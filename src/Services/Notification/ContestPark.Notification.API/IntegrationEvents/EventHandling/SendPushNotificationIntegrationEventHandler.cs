using ContestPark.Core.Enums;
using ContestPark.Core.Extensions;
using ContestPark.Core.Models;
using ContestPark.EventBus.Abstractions;
using ContestPark.Notification.API.Enums;
using ContestPark.Notification.API.IntegrationEvents.Events;
using ContestPark.Notification.API.Models;
using ContestPark.Notification.API.Resources;
using ContestPark.Notification.API.Services.PushNotification;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Threading.Tasks;

namespace ContestPark.Notification.API.IntegrationEvents.EventHandling
{
    public class SendPushNotificationIntegrationEventHandler : IIntegrationEventHandler<SendPushNotificationIntegrationEvent>
    {
        #region Private variables

        private readonly ILogger<SendPushNotificationIntegrationEventHandler> _logger;
        private readonly IPushNotification _pushNotification;

        #endregion Private variables

        #region Constructor

        public SendPushNotificationIntegrationEventHandler(ILogger<SendPushNotificationIntegrationEventHandler> logger,
                                                           IPushNotification pushNotification)
        {
            _logger = logger;
            _pushNotification = pushNotification;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Push notification gönderme işlemlerini handler eder
        /// </summary>
        /// <param name="event"Bildirim bilgileri></param>
        public async Task Handle(SendPushNotificationIntegrationEvent @event)
        {
            if (string.IsNullOrEmpty(@event.UserId) || !PushNotificationTypes.Reward.HasFlag(@event.PushNotificationType))
                return;

            PushNotificationMessageModel pushNotification = null;

            switch (@event.PushNotificationType)
            {
                case PushNotificationTypes.Reward:
                    pushNotification = RewardAsync(@event.CurrentUserLanguage);
                    break;
            }

            if (pushNotification == null)
                return;

            pushNotification.UserId = @event.UserId;

            if (@event.ScheduleDate.HasValue)
                pushNotification.ScheduleDate = @event.ScheduleDate;

            var isSuccess = await _pushNotification.SendPushAsync(pushNotification);
            if (!isSuccess)
            {
                _logger.LogInformation("Push notification gönderme başarısız oldu.");

                return;
            }
        }

        /// <summary>
        /// Günlük ödül kazanma süresi doldu bildirimi gönderir
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <param name="currentUserLanguage">Bildirimin gidece dil seçeneği</param>
        private PushNotificationMessageModel RewardAsync(Languages currentLanguages)
        {
            CultureInfo culture = CultureInfo.CreateSpecificCulture(currentLanguages.ToLanguageCode());
            string text = NotificationResource.ResourceManager.GetString(nameof(NotificationResource.CollectDailyGoldRewards), culture);

            return new PushNotificationMessageModel
            {
                Title = "ContestPark",
                Text = text,
                Icon = DefaultImages.Logo,
            };
        }

        #endregion Methods
    }
}
