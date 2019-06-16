using ContestPark.Core.Enums;
using ContestPark.EventBus.Events;
using ContestPark.Identity.API.Data.Repositories.User;
using ContestPark.Identity.API.IntegrationEvents;
using ContestPark.Identity.API.IntegrationEvents.Events;
using ContestPark.Identity.API.Models;
using ContestPark.Identity.API.Resources;
using ContestPark.Identity.API.Services;
using ContestPark.Identity.API.Services.BlobStorage;
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

namespace ContestPark.Identity.API.Controllers
{
    public class AccountController : Core.Controllers.ControllerBase
    {
        #region Private variables

        private readonly IEmailService _emailService;
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IIdentityIntegrationEventService _identityIntegrationEventService;
        private readonly IBlobStorageService _blobStorageService;

        #endregion Private variables

        #region Constructor

        public AccountController(IEmailService emailService,
                                 ILogger<AccountController> logger,
                                 UserManager<ApplicationUser> userManager,
                                 IUserRepository userRepository,
                                 IIdentityIntegrationEventService identityIntegrationEventService,
                                 IBlobStorageService blobStorageService) : base(logger)
        {
            _emailService = emailService;
            _logger = logger;
            _userManager = userManager;
            _userRepository = userRepository;
            _identityIntegrationEventService = identityIntegrationEventService;
            _blobStorageService = blobStorageService;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Kapak resmi değiştir
        /// </summary>
        /// <param name="files">Yüklenen resim</param>
        /// <returns>Resim url</returns>
        [HttpPost]
        [Route("changeCoverPicture")]
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
        [Route("changeProfilePicture")]
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

            // Diğer servislere resmin değiştiğini bildir
            var profilePictureChangedIntegrationEvent = new ProfilePictureChangedIntegrationEvent(UserId, fileName, oldProfilePicturePath);
            await PublishEvent(profilePictureChangedIntegrationEvent);

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
        [Route("update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateUserInfo([FromBody]UpdateUserInfoModel updateUserInfo)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
                return NotFound();

            string oldUserName = user.UserName;
            string oldEmail = user.Email;
            string oldFullName = user.FullName;

            List<string> errors = new List<string>();

            if (oldEmail != updateUserInfo.Email)
            {
                IdentityResult result = await _userManager.SetEmailAsync(user, updateUserInfo.Email);
                if (!result.Succeeded && result.Errors.Count() > 0)
                    errors.AddRange(IdentityResultErrors(result.Errors));
            }

            if (oldUserName != updateUserInfo.UserName)
            {
                IdentityResult result = await _userManager.SetUserNameAsync(user, updateUserInfo.UserName);
                if (!result.Succeeded && result.Errors.Count() > 0)
                    errors.AddRange(IdentityResultErrors(result.Errors));
            }

            if (oldFullName != updateUserInfo.FullName)
            {
                user.FullName = updateUserInfo.FullName;
                IdentityResult result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded && result.Errors.Count() > 0)
                    errors.AddRange(IdentityResultErrors(result.Errors));
            }

            if (errors.Count > 0)
                return BadRequest(errors);

            // Create Integration Event to be published through the Event Bus
            var userInfoChangedIntegrationEvent = new UserInfoChangedIntegrationEvent(user.Id, user.FullName, user.UserName, oldFullName, oldUserName);

            await PublishEvent(userInfoChangedIntegrationEvent);

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

            // Create Integration Event to be published through the Event Bus
            var newUserRegisterIntegrationEvent = new NewUserRegisterIntegrationEvent(user.Id, user.FullName, user.UserName, user.ProfilePicturePath);

            await PublishEvent(newUserRegisterIntegrationEvent);

            _logger.LogInformation($"Register new user: {user.Id} userName: {user.UserName}");

            return Ok();
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

        private async Task PublishEvent(IntegrationEvent @event)
        {
            // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
            await _identityIntegrationEventService.SaveEventAndApplicationContextChangesAsync(@event);

            // Publish through the Event Bus and mark the saved event as published
            await _identityIntegrationEventService.PublishThroughEventBusAsync(@event);
        }

        /// <summary>
        /// Parametreden gelen kullanıcıların bilgilerini döner
        /// sadece bizim servislerden istek atılabilir dışarıdan erişilemez!
        /// </summary>
        /// <param name="userInfos">Kullanıcı id listesi</param>
        /// <returns>Parametreden gelen kullanıcıların bilgileri</returns>
        [HttpPost]
        [Route("UserInfos")]
        [ProducesResponseType(typeof(List<UserNotFoundModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetUserInfos([FromBody]List<string> userInfos)
        {
            if (userInfos == null || userInfos.Count == 0)
                return BadRequest();

            var users = _userRepository.GetUserInfos(userInfos);
            if (users == null || users.Count() == 0)
                return NotFound();

            return Ok(users);
        }

        #endregion Methods
    }
}