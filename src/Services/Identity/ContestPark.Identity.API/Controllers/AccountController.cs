using ContestPark.Core.Enums;
using ContestPark.Core.Models;
using ContestPark.Identity.API.Data.Repositories.User;
using ContestPark.Identity.API.IntegrationEvents;
using ContestPark.Identity.API.IntegrationEvents.Events;
using ContestPark.Identity.API.Models;
using ContestPark.Identity.API.Resources;
using ContestPark.Identity.API.Services;
using ContestPark.Identity.API.Services.BlobStorage;
using ContestPark.Identity.API.Services.Block;
using ContestPark.Identity.API.Services.Follow;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ContestPark.Identity.API.ControllersIdentityResource
{
    public class AccountController : Core.Controllers.ControllerBase
    {
        #region Private variables

        private readonly IEmailService _emailService;
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBlockService _blockService;
        private readonly IFollowService _followService;
        private readonly IUserRepository _userRepository;
        private readonly IIdentityIntegrationEventService _identityIntegrationEventService;
        private readonly IBlobStorageService _blobStorageService;

        #endregion Private variables

        #region Constructor

        public AccountController(IEmailService emailService,
                                 ILogger<AccountController> logger,
                                 UserManager<ApplicationUser> userManager,
                                 IBlockService blockService,
                                 IFollowService followService,
                                 IUserRepository userRepository,
                                 IIdentityIntegrationEventService identityIntegrationEventService,
                                 IBlobStorageService blobStorageService) : base(logger)
        {
            _emailService = emailService;
            _logger = logger;
            _userManager = userManager;
            _blockService = blockService;
            _followService = followService;
            _userRepository = userRepository;
            _identityIntegrationEventService = identityIntegrationEventService;
            _blobStorageService = blobStorageService;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Kullanıcı adına göre profil bilgilerini verir
        /// </summary>
        /// <param name="userName">Kullanıcı adı</param>
        /// <returns>Profil bilgileri</returns>
        [HttpGet]
        [Route("Profile/{userName}")]
        [ProducesResponseType(typeof(ProfileInfoModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationResult), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetProfile([FromRoute] string userName)
        {
            UserProfileModel userProfile = _userRepository.GetUserInfoByUserName(userName);
            if (userProfile == null)
                return NotFound();

            bool? isBlocked = null;
            bool? isFollowing = null;
            if (!string.IsNullOrEmpty(UserId) && userProfile.UserId != UserId)
            {
                isBlocked = await _blockService.BlockedStatusAsync(UserId, userProfile.UserId);
                isFollowing = await _followService.FollowStatusAsync(UserId, userProfile.UserId);
            }

            return Ok(new ProfileInfoModel
            {
                CoverPicture = isBlocked == false || UserId == userProfile.UserId ? userProfile.CoverPicture : DefaultImages.DefaultCoverPicture,
                ProfilePicturePath = isBlocked == false || UserId == userProfile.UserId ? userProfile.ProfilePicturePath : DefaultImages.DefaultProfilePicture,
                FullName = userProfile.FullName,
                UserId = userProfile.UserId,
                FollowersCount = userProfile.FollowersCount,
                FollowUpCount = userProfile.FollowUpCount,
                GameCount = userProfile.GameCount,
                IsBlocked = isBlocked,
                IsFollowing = isFollowing,
                IsPrivateProfile = userProfile.IsPrivateProfile
            });
        }

        /// <summary>
        /// Random bot kullanıcı profil resmi  verir
        /// </summary>
        /// <returns>Kullanıcı profil resimleri</returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("GetRandomProfilePictures")]
        [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetRandomProfilePictures()
        {
            IEnumerable<string> randomProfilePictures = _userRepository.GetRandomProfilePictures();
            if (randomProfilePictures == null || randomProfilePictures.Count() == 0)
                return NotFound();

            return Ok(randomProfilePictures);
        }

        /// <summary>
        /// Kapak resmi değiştir
        /// </summary>
        /// <param name="files">Yüklenen resim</param>
        /// <returns>Resim url</returns>
        [HttpPost]
        [Route("ChangeCoverPicture")]
        [ProducesResponseType(typeof(ChangePictureModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        public async Task<IActionResult> ChangeCoverPicture(IList<IFormFile> files)// Burda tek list olarak alınmaması lazım ama tek alınca yüklenmiyor
        {
            if (files.Count == 0)
                return BadRequest();

            IFormFile file = files.First();
            if (file == null)
                return NotFound();

            if (_blobStorageService.CheckFileSize(file.Length))// 4 mb'den büyük ise dosya boyutu  geçersizdir
            {
                return BadRequest(IdentityResource.UnsupportedImageExtension);
            }

            string extension = Path.GetExtension(file.FileName);
            if (!_blobStorageService.CheckPictureExtension(extension))
            {
                return StatusCode((int)HttpStatusCode.UnsupportedMediaType, new ValidationResult(IdentityResource.UnsupportedImageExtension));
            }

            Stream pictureStream = file.OpenReadStream();
            if (pictureStream == null || pictureStream.Length == 0)
                return NotFound();

            string fileName = await _blobStorageService.UploadFileToStorageAsync(pictureStream, file.FileName, UserId);
            if (string.IsNullOrEmpty(fileName))
                return BadRequest();

            ApplicationUser user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
                return NotFound();

            if (!string.IsNullOrEmpty(user.CoverPicturePath))// Eğer varsa eski resmi sil
            {
                await _blobStorageService.DeleteFileAsync(user.CoverPicturePath);
            }

            // Profil resmi db güncelle
            user.CoverPicturePath = fileName;

            IdentityResult result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded && result.Errors.Count() > 0)
                return BadRequest(IdentityResultErrors(result.Errors));

            // Publish add post
            var @event = new NewPostAddedIntegrationEvent(NewPostAddedIntegrationEvent.PostTypes.Image,
                                                          NewPostAddedIntegrationEvent.PostImageTypes.CoverImage,
                                                          UserId,
                                                          fileName);
            _identityIntegrationEventService.NewPostAdd(@event);

            return Ok(new ChangePictureModel
            {
                PicturePath = user.CoverPicturePath
            });
        }

        /// <summary>
        /// Profil resmi değiştir
        /// </summary>
        /// <param name="files">Yüklenen resim</param>
        /// <returns>Resim url</returns>
        [HttpPost]
        [Route("ChangeProfilePicture")]
        [ProducesResponseType(typeof(ChangePictureModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        public async Task<IActionResult> ChangeProfilePicture(IList<IFormFile> files)// Burda tek list olarak alınmaması lazım ama tek alınca yüklenmiyor
        {
            if (files.Count == 0)
                return BadRequest();

            IFormFile file = files.First();
            if (file == null)
                return NotFound();

            if (_blobStorageService.CheckFileSize(file.Length))// 4 mb'den büyük ise dosya boyutu  geçersizdir
            {
                return BadRequest(IdentityResource.UnsupportedImageExtension);
            }

            string extension = Path.GetExtension(file.FileName);
            if (!_blobStorageService.CheckPictureExtension(extension))
            {
                return StatusCode((int)HttpStatusCode.UnsupportedMediaType, new ValidationResult(IdentityResource.UnsupportedImageExtension));
            }

            Stream pictureStream = file.OpenReadStream();
            if (pictureStream == null || pictureStream.Length == 0)
                return NotFound();

            string fileName = await _blobStorageService.UploadFileToStorageAsync(pictureStream, file.FileName, UserId);
            if (string.IsNullOrEmpty(fileName))
                return BadRequest();

            ApplicationUser user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
                return NotFound();

            if (!string.IsNullOrEmpty(user.ProfilePicturePath))// Eğer varsa eski resmi sil
            {
                await _blobStorageService.DeleteFileAsync(user.ProfilePicturePath);
            }

            // Profil resmi db güncelle
            string oldProfilePicturePath = user.ProfilePicturePath;
            user.ProfilePicturePath = fileName;

            IdentityResult result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded && result.Errors.Count() > 0)
                return BadRequest(IdentityResultErrors(result.Errors));

            // Publish add post
            var @event = new NewPostAddedIntegrationEvent(NewPostAddedIntegrationEvent.PostTypes.Image,
                                                          NewPostAddedIntegrationEvent.PostImageTypes.ProfileImage,
                                                          UserId,
                                                          fileName);
            _identityIntegrationEventService.NewPostAdd(@event);

            return Ok(new ChangePictureModel
            {
                PicturePath = user.ProfilePicturePath
            });
        }

        /// <summary>
        /// Kullanıcı bilgilerini güncelleme
        /// </summary>
        /// <param name="updateUserInfo">Güncellenecek bilgiler</param>
        /// <returns>Başarılı ise 200 ok değilse hata nedeni döner</returns>
        [HttpPost]
        [Route("Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateUserInfo([FromBody]UpdateUserInfoModel updateUserInfo)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
                return NotFound();

            string oldUserName = user.UserName;
            //  string oldEmail = user.Email;
            string oldFullName = user.FullName;

            bool oldIsPrivateProfile = user.IsPrivateProfile;

            List<string> errors = new List<string>();

            //if (oldEmail != updateUserInfo.Email)
            //{
            //    IdentityResult result = await _userManager.SetEmailAsync(user, updateUserInfo.Email);
            //    if (!result.Succeeded && result.Errors.Count() > 0)
            //        errors.AddRange(IdentityResultErrors(result.Errors));
            //}

            if (updateUserInfo.IsPrivateProfile.HasValue && oldIsPrivateProfile != updateUserInfo.IsPrivateProfile)
            {
                user.IsPrivateProfile = updateUserInfo.IsPrivateProfile.Value;
                IdentityResult result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded && result.Errors.Count() > 0)
                    errors.AddRange(IdentityResultErrors(result.Errors));
            }

            if (!string.IsNullOrEmpty(updateUserInfo.UserName) && oldUserName != updateUserInfo.UserName)
            {
                IdentityResult result = await _userManager.SetUserNameAsync(user, updateUserInfo.UserName);
                if (!result.Succeeded && result.Errors.Count() > 0)
                    errors.AddRange(IdentityResultErrors(result.Errors));
            }

            if (!string.IsNullOrEmpty(updateUserInfo.FullName) && oldFullName != updateUserInfo.FullName)
            {
                user.FullName = updateUserInfo.FullName;
                IdentityResult result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded && result.Errors.Count() > 0)
                    errors.AddRange(IdentityResultErrors(result.Errors));
            }

            if (errors.Count > 0)
                return BadRequest(errors);

            return Ok();
        }

        /// <summary>
        ///  Eski şifre ile şifre değiştir
        /// </summary>
        /// <param name="changePasswordModel">Şuanki şifre ve yeni şifre</param>
        /// <returns>
        /// Güncelleme başarılı olduysa başarılı yanıtı döndürür,
        /// aksi takdirde hatanın hata nedenlerini döndürür
        /// </returns>
        [HttpPost]
        [Route("ChangePassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordModel changePasswordModel)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
                return NotFound();

            bool isPasswordSuccess = await _userManager.CheckPasswordAsync(user, changePasswordModel.OldPassword);

            if (!isPasswordSuccess)
                return BadRequest(IdentityResource.YourCurrentPasswordIsInvalid);

            IdentityResult result = await ResetPasswordAsync(user, changePasswordModel.NewPassword);
            if (!result.Succeeded)
                return BadRequest(IdentityResource.ErrorChangingPasswordPleaseTryAgain);

            return Ok();
        }

        /// <summary>
        /// Şifremi unuttum code kontrol ise şifre değiştir
        /// </summary>
        /// <param name="changePasswordModel">Kullanıcı adı veya eposta adresi</param>
        /// <returns>
        /// Güncelleme başarılı olduysa başarılı yanıtı döndürür,
        /// aksi takdirde hatanın hata nedenlerini döndürür
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("ChangePassword/code")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangePasswordWithCode([FromBody]ChangePasswordWithCodeModel changePasswordModel)
        {
            ApplicationUser user = _userRepository.GetUserByCode(changePasswordModel.Code);
            if (user == null)
                return NotFound();

            IdentityResult result = await ResetPasswordAsync(user, changePasswordModel.Password);
            if (!result.Succeeded)
                return BadRequest(IdentityResource.ErrorChangingPasswordPleaseTryAgain);

            _userRepository.RemoveCode(user.Id);

            return Ok();
        }

        /// <summary>
        /// Şifremi unuttum
        /// </summary>
        /// <param name="model">Kullanıcı adı veya eposta adresi</param>
        /// <returns>
        /// Güncelleme başarılı olduysa başarılı yanıtı döndürür,
           /// aksi takdirde hatanın hata nedenlerini döndürür
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("ForgotYourPassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ForgetPasswordAsync([FromBody]ForgetPasswordModel model)
        {
            if (string.IsNullOrEmpty(model.UserNameOrEmail))
                return BadRequest();

            ApplicationUser user = null;

            if (new EmailAddressAttribute().IsValid(model.UserNameOrEmail))
            {
                user = await _userManager.FindByEmailAsync(model.UserNameOrEmail.ToLower());
            }
            else
            {
                user = await _userManager.FindByNameAsync(model.UserNameOrEmail.ToLower());
            }

            if (user == null)
                return BadRequest(IdentityResource.UserNotFound);

            int code = _userRepository.InsertCode(user.Id);
            if (code == 0)
            {
                _logger.LogError("Şifre değiştirme sırasında code üretilemedi.");
                return BadRequest(IdentityResource.ThereWasAnErrorInSendingPassword);
            }

            string message = ForgotPasswordEmailMessageHtml(user.FullName, code);
            bool isSendMail = await _emailService.SendEmailAsync(user.Email, "Forgot your password?", message);
            if (!isSendMail)
            {
                _logger.LogError("Şifre değiştir maili gönderilemedi.");
                return BadRequest(IdentityResource.ThereWasAnErrorInSendingPassword);
            }

            return Ok();
        }

        /// <summary>
        /// Şifremi unuttum code kontrol
        /// </summary>
        /// <param name="model">Kullanıcı adı veya eposta adresi</param>
        /// <returns>
        /// Güncelleme başarılı olduysa başarılı yanıtı döndürür,
           /// aksi takdirde hatanın hata nedenlerini döndürür
        /// </returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("ForgotYourPassword/codecheck")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult ForgetPasswordCodeCheckAsync([FromQuery]int code)
        {
            bool isSuccess = _userRepository.CodeCheck(code);
            if (isSuccess)
                return Ok();

            return NotFound();
        }

        /// <summary>
        /// Üye ol
        /// </summary>
        /// <param name="signUpModel">Üye olunacak model</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationResult), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SignUp([FromBody]SignUpModel signUpModel)
        {
            if (signUpModel == null)
                return BadRequest();

            ApplicationUser user = new ApplicationUser
            {
                UserName = signUpModel.UserName.ToLower(),
                FullName = signUpModel.FullName.ToLowerInvariant(),
                Email = signUpModel.Email.ToLower(),
                LanguageCode = signUpModel.LanguageCode.ToLower(),
            };

            var result = await _userManager.CreateAsync(user, signUpModel.Password);
            if (!result.Succeeded && result.Errors.Count() > 0)
                return BadRequest(IdentityResultErrors(result.Errors));

            _logger.LogInformation($"Register new user: {user.Id} userName: {user.UserName}");

            return Ok();
        }

        /// <summary>
        /// Parametreden gelen kullanıcıların bilgilerini döner
        /// sadece bizim servislerden istek atılabilir dışarıdan erişilemez!
        /// </summary>
        /// <param name="userInfos">Kullanıcı id listesi</param>
        /// <returns>Parametreden gelen kullanıcıların bilgileri</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("UserInfos")]
        [ProducesResponseType(typeof(List<UserModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetUserInfos([FromBody]List<string> userInfos, [FromQuery]bool includeCoverPicturePath = false)
        {
            if (userInfos == null || userInfos.Count == 0)
                return BadRequest();

            var users = _userRepository.GetUserInfos(userInfos, includeCoverPicturePath);
            if (users == null || users.Count() == 0)
                return NotFound();

            return Ok(users);
        }

        /// <summary>
        /// Rastgele kullanıcı id verir
        /// </summary>
        /// <returns>Kullanıcı id</returns>
        [HttpGet("RandomUserId")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult GetRandomUserId()
        {
            return Ok(new
            {
                userId = _userRepository.GetRandomBotUserId()
            });
        }

        /// <summary>
        /// Şifremi unuttum email içeriği
        /// </summary>
        /// <param name="fullname"></param>
        /// <param name="code"></param>
        /// <returns>Html mesaj</returns>
        private string ForgotPasswordEmailMessageHtml(string fullname, int code)
        {
            return $@"
                    {IdentityResource.Hello} {fullname},<br/><br/>
                    {IdentityResource.WeHaveReceivedARequestToRenewYourContestParkPassword}</br>
                    {IdentityResource.EnterTheCodeBelowToRefreshThePassword}<br/><br/>

                    <div style='code-box'>
                          {code}
                    </div><br/><br/>
                   ";
        }

        /// <summary>
        /// Kullanıcının şifresini değiştir
        /// </summary>
        /// <param name="user">Şifresi değişecek kullanıcı</param>
        /// <param name="newPassword">Yeni şifre</param>
        /// <returns>İşem sonucu</returns>
        private async Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string newPassword)
        {
            string code = await _userManager.GeneratePasswordResetTokenAsync(user);// şifre değiştirme için kod oluşturuldu
            IdentityResult result = await _userManager.ResetPasswordAsync(user, code, newPassword);// şifre değiştirildi edildi

            return result;
        }

        /// <summary>
        /// Identity result hatalarını translate edip Bad request olarak döndürür
        /// </summary>
        /// <param name="result">Errors</param>
        /// <returns></returns>
        private List<string> IdentityResultErrors(IEnumerable<IdentityError> identityErrors)
        {
            var errors = identityErrors.Select(e =>
                    CurrentUserLanguage.HasFlag(Languages.Turkish) ? IdentityResource.ResourceManager.GetString(e.Code) : e.Description
                    ).ToList();

            return errors;
        }

        #endregion Methods
    }
}
