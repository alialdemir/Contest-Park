using ContestPark.Core.Database.Models;
using ContestPark.Core.Models;
using ContestPark.Core.Services.Identity;
using ContestPark.EventBus.Abstractions;
using ContestPark.Notification.API.Enums;
using ContestPark.Notification.API.Infrastructure.Repositories.Notification;
using ContestPark.Notification.API.Infrastructure.Repositories.PushNotification;
using ContestPark.Notification.API.IntegrationEvents.Events;
using ContestPark.Notification.API.Models;
using ContestPark.Notification.API.Resources;
using ContestPark.Notification.API.Services.Sms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ContestPark.Notification.API.Controllers
{
    public class NotificationController : Core.Controllers.ControllerBase
    {
        #region Private variables

        private readonly INotificationRepository _notificationRepository;
        private readonly ISmsService _smsService;
        private readonly IEventBus _eventBus;
        private readonly IPushNotificationRepository _pushNotificationRepository;
        private readonly IIdentityService _identityService;

        #endregion Private variables

        #region Constructor

        public NotificationController(ILogger<NotificationController> logger,
                                      ISmsService smsService,
                                      IEventBus eventBus,
                                      IPushNotificationRepository pushNotificationRepository,
                                      IIdentityService identityService,
                                      INotificationRepository notificationRepository) : base(logger)
        {
            _smsService = smsService;
            _eventBus = eventBus;
            _pushNotificationRepository = pushNotificationRepository;
            _identityService = identityService;
            _notificationRepository = notificationRepository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Firebase push notification token güncelle
        /// </summary>
        /// <param name="tokenModel">Token</param>
        [HttpPost("Push/Token")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdatePushToken([FromBody]PushNotificationTokenModel tokenModel)
        {
            if (tokenModel == null || string.IsNullOrEmpty(tokenModel.Token))
                return BadRequest();

            // TODO: Burayı queue alınalı

            tokenModel.UserId = UserId;

            bool isSuccess = await _pushNotificationRepository.UpdateTokenByUserIdAsync(tokenModel);
            if (!isSuccess)
                BadRequest();

            return Ok();
        }

        /// <summary>
        /// Push notification gönderir
        /// </summary>
        /// <param name="pushNotificationType">Gönderilecek push notification tipi</param>
        [HttpPost("Push/Send")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IActionResult PushSend([FromQuery]PushNotificationTypes pushNotificationType)
        {
            Logger.LogInformation("Push isteği geldi {push} {UserId}", pushNotificationType, UserId);

            var @event = new SendPushNotificationIntegrationEvent(pushNotificationType,
                                                              CurrentUserLanguage,
                                                              UserId);

            _eventBus.Publish(@event);

            return Ok();
        }

        /// <summary>
        /// Kullanıcının bildirim listesi
        /// </summary>
        /// <param name="pagingModel">Sayfalama</param>
        /// <returns>Bildirimler</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ServiceModel<NotificationModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Notifications([FromQuery]PagingModel pagingModel)
        {
            ServiceModel<NotificationModel> result = _notificationRepository.Notifications(UserId, CurrentUserLanguage, pagingModel);
            if (result == null || !result.Items.Any())
                return NotFound();

            IEnumerable<string> userIds = result
                                            .Items
                                            .Select(notification => notification.UserId)
                                            .ToList();

            IEnumerable<UserModel> userInfos = await _identityService.GetUserInfosAsync(userIds);
            if (userInfos != null && userInfos.Any())
            {
                result.Items = (from notification in result.Items
                                let user = userInfos.FirstOrDefault(user => user.UserId == notification.UserId)
                                select new NotificationModel
                                {
                                    Date = notification.Date,
                                    Description = string.Format(notification.Description, user.UserName),
                                    IsNotificationSeen = notification.IsNotificationSeen,
                                    Link = notification.Link,
                                    NotificationId = notification.NotificationId,
                                    NotificationType = notification.NotificationType,
                                    UserId = notification.UserId,
                                    ProfilePicturePath = user.ProfilePicturePath,
                                    FullName = user.FullName,
                                    UserName = user.UserName,
                                    IsFollowing = notification.IsFollowing,
                                    PostId = notification.PostId,
                                }).ToList();
            }

            return Ok(result);
        }

        /// <summary>
        /// Ülke kodu ve telefon numarasına sms ile doğrulama kodu gönderir
        /// Kodu doğrulamak için "CheckSmsCode" api'sini kullanın
        /// </summary>
        /// <param name="smsInfo">Telefon numarası bilgisi</param>
        /// <returns>Sms ile gönderilen kod</returns>
        [HttpPost("SendSms")]
        [AllowAnonymous]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> SendSms([FromBody]SmsInfoModel smsInfo)
        {
            if (smsInfo == null
                || string.IsNullOrEmpty(smsInfo.PhoneNumber)
                || string.IsNullOrEmpty(smsInfo.CountryCode)
                || string.IsNullOrEmpty(smsInfo.PhoneNumberWithCountryCode))
                return BadRequest();

            bool isSmsSend = smsInfo.PhoneNumber.StartsWith("5454");// Eğer numaranın başı 5454 ile başlıyorsa sms göndermeden login olmalı özel durumlar için ekledim
            if (isSmsSend)
            {
                smsInfo.PhoneNumber = smsInfo.PhoneNumber.Substring(4, smsInfo.PhoneNumber.Length - 4);
            }

            var match = Regex.Match(smsInfo.PhoneNumber, @"^5(0[5-7]|[3-5]\d) ?\d{3} ?\d{4}$", RegexOptions.IgnoreCase);// Globale çıkınca burayı kaldıralım
            if (!match.Success)
            {
                return BadRequest(NotificationResource.InvalidPhoneNumber);
            }

            int code = !isSmsSend
                ? new Random().Next(1000, 9999)
                : 5454;// 5454 özel durum için

            string message = $"{NotificationResource.ContestParkAccessCode}{code}";

            if (!isSmsSend)
            {
                bool isSuccess = await _smsService.SendSms(message, smsInfo.PhoneNumberWithCountryCode);
                if (!isSuccess)
                {
                    Logger.LogError(
                        "{phoneNumber} numaralı telefona {code} doğrulama kodu gönderilemedi!",
                        smsInfo.PhoneNumberWithCountryCode, code);

                    return BadRequest();
                }
            }

            Logger.LogInformation(
                "{phoneNumber} numaralı telefona {code} doğrulama kodu gönderildi. {PhoneNumber}", smsInfo.PhoneNumberWithCountryCode,
                code,
              smsInfo.PhoneNumber);

            Logger.LogInformation("redis key set {key}", smsInfo.PhoneNumber);

            bool isSmsSuccess = _smsService.Insert(new SmsRedisModel
            {
                Code = code,
                PhoneNumber = smsInfo.PhoneNumber
            });
            if (!isSmsSuccess)
                return BadRequest();

            return Ok();
        }

        /// <summary>
        /// Sms ile gönderilen kod doğru girilmiş mi kontrol eder
        /// Sms ile gelen kod ve telefon numarası doğru ise kullanıcı adı döner eğer kullanıcı kayıtlı değilse kullanıcı adı boş gelir
        /// </summary>
        /// <param name="smsModel">Sms ile girilen kod</param>
        [HttpPost("CheckSmsCode")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(SmsRedisModel), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CheckSmsCode([FromBody]SmsModel smsModel)
        {
            if (smsModel == null
                || string.IsNullOrEmpty(smsModel.PhoneNumber)
                || smsModel.Code < 1000// code 1000 den büyük olmalı
                || smsModel.Code > 9999// code 9999 küçük olmalı
                )
                return BadRequest();

            bool isSmsSend = smsModel.PhoneNumber.StartsWith("5454");// Eğer numaranın başı 5454 ile başlıyorsa sms göndermeden login olmalı özel durumlar için ekledim
            if (isSmsSend)
            {
                smsModel.PhoneNumber = smsModel.PhoneNumber.Substring(4, smsModel.PhoneNumber.Length - 4);
            }

            Logger.LogInformation("Sms kodu doğrulama isteği geldi {PhoneNumber} {Code}", smsModel.PhoneNumber, smsModel.Code);

            Logger.LogInformation("redis key get {key}", smsModel.PhoneNumber);

            SmsRedisModel redisCode = _smsService.GetSmsCode(smsModel.PhoneNumber);
            if (redisCode == null || redisCode.Code != smsModel.Code)
                return BadRequest();

            _smsService.Delete(smsModel.PhoneNumber);

            UserNameModel userNameModel = await _identityService.GetUserNameByPhoneNumber(smsModel.PhoneNumber);
            if (userNameModel == null)
                return BadRequest();

            Logger.LogInformation(
                "{phoneNumber} telefon numarası ile {code} doğrulama kodu doğru girildi ve {userName} kullanıcı adı ile eşleşti.",
                smsModel.PhoneNumber,
                smsModel.Code,
                userNameModel.UserName);

            return Ok(userNameModel);
        }

        #region Eski sms login

        /// <summary>
        /// Sms ile login olma için kod üretip sms gönderir
        /// </summary>
        /// <param name="smsInfo">Telefon numarası bilgisi</param>
        /// <returns>Sms ile gönderilen kod</returns>
        [HttpPost("Sms")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(SmsModel), (int)HttpStatusCode.OK)]
        [System.Obsolete]
        public async Task<IActionResult> LogInSms([FromBody]SmsInfoModel smsInfo)
        {
            if (smsInfo == null
                || string.IsNullOrEmpty(smsInfo.PhoneNumber)
                || string.IsNullOrEmpty(smsInfo.CountryCode)
                || string.IsNullOrEmpty(smsInfo.PhoneNumberWithCountryCode))
                return BadRequest();

            var match = Regex.Match(smsInfo.PhoneNumber, @"^5(0[5-7]|[3-5]\d) ?\d{3} ?\d{4}$", RegexOptions.IgnoreCase);// Globale çıkınca burayı kaldıralım
            if (!match.Success)
            {
                return BadRequest(NotificationResource.InvalidPhoneNumber);
            }

            SmsModel result = new SmsModel
            {
                Code = new Random().Next(1000, 9999),
            };

            string message = $"{NotificationResource.ContestParkAccessCode}{result.Code}";

            bool isSmsSuccess = await _smsService.SendSms(message, smsInfo.PhoneNumberWithCountryCode);
            if (!isSmsSuccess)
                return BadRequest();

            return Ok(result);
        }

        #endregion Eski sms login

        #endregion Methods
    }
}
