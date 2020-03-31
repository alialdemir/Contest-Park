using ContestPark.Core.Enums;
using ContestPark.Core.Models;
using ContestPark.Identity.API.Data.Repositories.DeviceInfo;
using ContestPark.Identity.API.Data.Repositories.Reference;
using ContestPark.Identity.API.Data.Repositories.ReferenceCode;
using ContestPark.Identity.API.Data.Repositories.User;
using ContestPark.Identity.API.Extensions;
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
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
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
        private readonly IReferenceCodeRepostory _referenceCodeRepostory;
        private readonly IReferenceRepository _referenceRepository;
        private readonly IdentitySettings _identitySettings;
        private readonly IFollowService _followService;
        private readonly IDeviceInfoRepository _deviceInfoRepository;
        private readonly IUserRepository _userRepository;
        private readonly IIdentityIntegrationEventService _identityIntegrationEventService;
        private readonly IFileUploadService _fileUploadService;

        #endregion Private variables

        #region Constructor

        public AccountController(IEmailService emailService,
                                 ILogger<AccountController> logger,
                                 UserManager<ApplicationUser> userManager,
                                 IBlockService blockService,
                                 IReferenceCodeRepostory referenceCodeRepostory,
                                 IReferenceRepository referenceRepository,
                                 IOptions<IdentitySettings> settings,
                                 IFollowService followService,
                                 IDeviceInfoRepository deviceInfoRepository,
                                 IUserRepository userRepository,
                                 IIdentityIntegrationEventService identityIntegrationEventService,
                                 IFileUploadService fileUploadService) : base(logger)
        {
            _emailService = emailService;
            _logger = logger;
            _userManager = userManager;
            _blockService = blockService;
            _referenceCodeRepostory = referenceCodeRepostory;
            _referenceRepository = referenceRepository;
            _identitySettings = settings.Value;
            _followService = followService;
            _deviceInfoRepository = deviceInfoRepository;
            _userRepository = userRepository;
            _identityIntegrationEventService = identityIntegrationEventService;
            _fileUploadService = fileUploadService;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Kullanıcıya ait telefon numarası
        /// </summary>
        /// <returns>Telefon numarası</returns>
        [HttpGet]
        [Route("PhoneNumber")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult GetPhoneNumber()
        {
            string phoneNumber = _userRepository.GetPhoneNumber(UserId);
            if (string.IsNullOrEmpty(phoneNumber))
                return NotFound();

            return Ok(new
            {
                phoneNumber
            });
        }

        /// <summary>
        /// Kullanıcı adına göre profil bilgilerini verir
        /// </summary>
        /// <param name="userName">Kullanıcı adı</param>
        /// <returns>Profil bilgileri</returns>
        [HttpGet]
        [Route("Profile/{userName}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProfileInfoModel), StatusCodes.Status200OK)]
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
            if (files == null || files.Count == 0)
                return BadRequest();

            IFormFile file = files.First();
            if (file == null)
                return NotFound();

            if (_fileUploadService.CheckFileSize(file.Length))// 4 mb'den büyük ise dosya boyutu  geçersizdir
            {
                return BadRequest(IdentityResource.UnsupportedImageExtension);
            }

            if (!_fileUploadService.CheckPictureExtension(file.ContentType))
            {
                return StatusCode((int)HttpStatusCode.UnsupportedMediaType, new ValidationResult(IdentityResource.UnsupportedImageExtension));
            }

            Stream pictureStream = file.OpenReadStream();
            if (pictureStream == null || pictureStream.Length == 0)
                return NotFound();

            string fileName = await _fileUploadService.UploadFileToStorageAsync(pictureStream,
                                                                                 file.FileName,
                                                                                 UserId,
                                                                                 file.ContentType,
                                                                                 Enums.PictureTypes.Cover);
            if (string.IsNullOrEmpty(fileName))
                return BadRequest();

            ApplicationUser user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
                return NotFound();

            if (!string.IsNullOrEmpty(user.CoverPicturePath))// Eğer varsa eski resmi sil
            {
                _fileUploadService.DeleteFile(UserId, user.CoverPicturePath, Enums.PictureTypes.Cover);
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
            _identityIntegrationEventService.PublishEvent(@event);

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
            if (files == null || files.Count == 0)
                return BadRequest();

            IFormFile file = files.First();
            if (file == null)
                return NotFound();

            if (_fileUploadService.CheckFileSize(file.Length))// 4 mb'den büyük ise dosya boyutu  geçersizdir
            {
                return BadRequest(IdentityResource.UnsupportedImageExtension);
            }

            if (!_fileUploadService.CheckPictureExtension(file.ContentType))
            {
                return StatusCode((int)HttpStatusCode.UnsupportedMediaType, new ValidationResult(IdentityResource.UnsupportedImageExtension));
            }

            Stream pictureStream = file.OpenReadStream();
            if (pictureStream == null || pictureStream.Length == 0)
                return NotFound();

            string fileName = await _fileUploadService.UploadFileToStorageAsync(pictureStream,
                                                                                 file.FileName,
                                                                                 UserId,
                                                                                 file.ContentType,
                                                                                 Enums.PictureTypes.Profile);
            if (string.IsNullOrEmpty(fileName))
                return BadRequest();

            ApplicationUser user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
                return NotFound();

            if (!string.IsNullOrEmpty(user.ProfilePicturePath))// Eğer varsa eski resmi sil
            {
                _fileUploadService.DeleteFile(UserId, user.ProfilePicturePath, Enums.PictureTypes.Profile);
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
            _identityIntegrationEventService.PublishEvent(@event);

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
        /// Kullanıcının dil ayarını günceller
        /// </summary>
        /// <param name="language">Dil ayarı</param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateLanguage")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateLanguage([FromQuery]Languages language)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
                return NotFound();

            user.Language = language;
            IdentityResult result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded && result.Errors.Count() > 0)
                return BadRequest(result.Errors);

            return Ok();
        }

        /// <summary>
        /// Kullanıcının temel bilgilerini döndürür
        /// </summary>
        /// <returns>Kullanıcı bilgileri</returns>
        [HttpGet]
        [Route("UserInfo")]
        [ProducesResponseType(typeof(UserInfoModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UserInfo()
        {
            UserInfoModel userInfo = _userRepository.UserInfo(UserId);
            if (userInfo != null)
                return Ok(userInfo);

            return NotFound();
        }

        /// <summary>
        /// Telefon numarasına ait kullanıcı adını verir
        /// </summary>
        /// <param name="phoneNumber">Telefon numarası</param>
        /// <returns>Kullanıcı adı</returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("GetUserName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [System.Obsolete]
        public IActionResult GetUserNameByPhoneNumber([FromQuery]string phoneNumber)
        {
            string userName = _userRepository.GetUserNameByPhoneNumber(phoneNumber);
            if (!string.IsNullOrEmpty(userName))
                return Ok(new { userName });

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

            //if (!string.IsNullOrEmpty(signUpModel.DeviceIdentifier) && _deviceInfoRepository.CheckDeviceIdentifier(signUpModel.DeviceIdentifier))
            //{
            //    return BadRequest(IdentityResource.GlobalErrorMessage);
            //}

            string isPhoneNumberRegistered = _userRepository.GetUserNameByPhoneNumber(signUpModel.Password);
            if (!string.IsNullOrEmpty(isPhoneNumberRegistered))// telefon numarası kayıtlı mı kontrol ettik
            {
                return BadRequest(IdentityResource.ThisPhoneNumberIsRegistered);
            }

            ApplicationUser user = new ApplicationUser
            {
                UserName = signUpModel.UserName.ToLower(),
                FullName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(signUpModel.FullName),
                LanguageCode = signUpModel.LanguageCode.ToLower(),
                Language = signUpModel.LanguageCode.ToLanguagesEnum(),
                PhoneNumber = signUpModel.Password// Sms ile login olma sisteminde kullanıcıdan şifre almamak için şifresini telefon numarası yaptık
            };

            var result = await _userManager.CreateAsync(user, signUpModel.Password);
            if (!result.Succeeded && result.Errors.Count() > 0)
                return BadRequest(IdentityResultErrors(result.Errors));

            // Kullanıcı yeni login olduğu için  belirli bir miktar altın ekledim
            PublishChangeBalanceIntegrationEvent(120.00m, BalanceTypes.Gold, user.Id);

            try
            {
                AddReferenceBalance(user.Id, signUpModel.ReferenceCode);

                bool isInRole = await _userManager.IsInRoleAsync(user, "user");
                if (!isInRole)
                    await _userManager.AddToRoleAsync(user, "user");// Role eklendi

                if (!string.IsNullOrEmpty(signUpModel.DeviceIdentifier))
                {
                    // Bu kısım üye olmayı etkilemesin diye try-catch bloklarına aldım
                    _deviceInfoRepository.Insert(signUpModel.DeviceIdentifier);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Device info kayıt edilirken hata oluştu. device info id: {signUpModel.DeviceIdentifier}", ex);
            }

            _logger.LogInformation($"Register new user: {user.Id} userName: {user.UserName}");

            return Ok();
        }

        /// <summary>
        /// Referans kodu ile bakiye ekle
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <param name="referenceCode">Referans kodu</param>
        private void AddReferenceBalance(string userId, string referenceCode)
        {
            if (!string.IsNullOrEmpty(referenceCode))
            {
                _logger.LogInformation($@"Referans kodu ile üye olma işlemi gerçekleşti. Reference Code:{referenceCode}
                                                                                         New User Id: {userId}
                                                                                         GiftMoneyAmount: {_identitySettings.GiftMoneyAmount}");

                string referenceUserId = _userRepository.GetUserIdByUserName(referenceCode);
                if (!string.IsNullOrEmpty(referenceUserId))// Kullanıcı adı ile referans kodu var mı diye kontrol ettik
                {
                    PublishChangeBalanceIntegrationEvent(_identitySettings.GiftMoneyAmount, BalanceTypes.Money, userId);

                    PublishChangeBalanceIntegrationEvent(_identitySettings.GiftMoneyAmount, BalanceTypes.Money, referenceUserId);

                    _referenceCodeRepostory.Insert(referenceCode, referenceUserId, userId);
                }
                else
                {
                    ReferenceModel referenceModel = _referenceRepository.IsCodeActive(referenceCode);
                    if (referenceModel != null)// Eğer bizim tanımlaadığımız referans kodu ile geliyorsa referans kodundaki para biriminde ve para değeri kadar bakiye ekledik
                    {
                        PublishChangeBalanceIntegrationEvent(referenceModel.Amount, referenceModel.BalanceType, userId);

                        _referenceCodeRepostory.Insert(referenceCode, null, userId);
                    }
                }
            }
        }

        /// <summary>
        /// Eğer referans kodu ile üye oluyorsa belirli birr miktar para verdik
        /// </summary>
        private void PublishChangeBalanceIntegrationEvent(decimal amount, BalanceTypes balanceType, string userId)
        {
            Task.Factory.StartNew(() =>
          {
              var @eventBalanceMoney = new ChangeBalanceIntegrationEvent(amount, userId, balanceType, BalanceHistoryTypes.DailyChip);

              _identityIntegrationEventService.PublishEvent(@eventBalanceMoney);
          });
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
        [HttpGet("RandomUser")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(RandomUserModel), StatusCodes.Status200OK)]
        public IActionResult GetRandomUser()
        {
            var user = _userRepository.GetRandomBotUser();
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        /// <summary>
        /// Identity result hatalarını translate edip Bad request olarak döndürür
        /// </summary>
        /// <param name="result">Errors</param>
        /// <returns></returns>
        private IEnumerable<string> IdentityResultErrors(IEnumerable<IdentityError> identityErrors)
        {
            var errors = identityErrors.Select(e =>
                    CurrentUserLanguage.HasFlag(Languages.Turkish) ? IdentityResource.ResourceManager.GetString(e.Code) : e.Description
                    ).AsEnumerable();

            return errors;
        }

        #endregion Methods
    }
}
