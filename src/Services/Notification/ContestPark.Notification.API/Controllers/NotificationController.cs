using ContestPark.Core.Database.Models;
using ContestPark.Core.Models;
using ContestPark.Core.Services.Identity;
using ContestPark.Notification.API.Infrastructure.Repositories.Notification;
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
        private readonly IIdentityService _identityService;

        #endregion Private variables

        #region Constructor

        public NotificationController(ILogger<NotificationController> logger,
                                      ISmsService smsService,
                                      IIdentityService identityService,
                                      INotificationRepository notificationRepository) : base(logger)
        {
            _smsService = smsService;
            _identityService = identityService;
            _notificationRepository = notificationRepository;
        }

        #endregion Constructor

        #region Methods

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
        /// Sms ile login olma için kod üretip sms gönderir
        /// </summary>
        /// <param name="smsInfo">Telefon numarası bilgisi</param>
        /// <returns>Sms ile gönderilen kod</returns>
        [HttpPost("Sms")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(SmsModel), (int)HttpStatusCode.OK)]
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

        #endregion Methods
    }
}
