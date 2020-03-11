﻿using ContestPark.Core.Database.Models;
using ContestPark.Core.Models;
using ContestPark.Core.Services.Identity;
using ContestPark.Notification.API.Infrastructure.Repositories.Notification;
using ContestPark.Notification.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ContestPark.Notification.API.Controllers
{
    public class NotificationController : Core.Controllers.ControllerBase
    {
        #region Private variables

        private readonly INotificationRepository _notificationRepository;

        private readonly IIdentityService _identityService;

        #endregion Private variables

        #region Constructor

        public NotificationController(ILogger<NotificationController> logger,
                                      IIdentityService identityService,
                                      INotificationRepository notificationRepository) : base(logger)
        {
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
                                            .Select(notification => notification.WhoUserId)
                                            .ToList();

            IEnumerable<UserModel> userInfos = await _identityService.GetUserInfosAsync(userIds);
            if (userInfos != null && userInfos.Any())
            {
                result.Items = (from notification in result.Items
                                let user = userInfos.FirstOrDefault(user => user.UserId == notification.WhoUserId)
                                select new NotificationModel
                                {
                                    Date = notification.Date,
                                    Description = string.Format(notification.Description, user.UserName),
                                    IsNotificationSeen = notification.IsNotificationSeen,
                                    Link = notification.Link,
                                    NotificationId = notification.NotificationId,
                                    NotificationType = notification.NotificationType,
                                    WhoUserId = notification.WhoUserId,
                                    ProfilePicturePath = user.ProfilePicturePath,
                                    WhoFullName = user.FullName,
                                    WhoUserName = user.UserName,
                                }).ToList();
            }

            return Ok(result);
        }

        #endregion Methods
    }
}
