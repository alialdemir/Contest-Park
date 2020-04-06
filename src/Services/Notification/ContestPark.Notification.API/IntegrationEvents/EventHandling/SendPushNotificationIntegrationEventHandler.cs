using ContestPark.Core.Enums;
using ContestPark.Core.Extensions;
using ContestPark.Core.Models;
using ContestPark.EventBus.Abstractions;
using ContestPark.Notification.API.Enums;
using ContestPark.Notification.API.Infrastructure.Repositories.PushNotification;
using ContestPark.Notification.API.IntegrationEvents.Events;
using ContestPark.Notification.API.Models;
using ContestPark.Notification.API.Resources;
using ContestPark.Notification.API.Services.FirebasePushNotification;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Notification.API.IntegrationEvents.EventHandling
{
    public class SendPushNotificationIntegrationEventHandler : IIntegrationEventHandler<SendPushNotificationIntegrationEvent>
    {
        #region Private variables

        private readonly ILogger<SendPushNotificationIntegrationEventHandler> _logger;
        private readonly IFirebasePushNotification _firebasePushNotification;
        private readonly IPushNotificationRepository _pushNotificationRepository;

        #endregion Private variables

        #region Constructor

        public SendPushNotificationIntegrationEventHandler(ILogger<SendPushNotificationIntegrationEventHandler> logger,
                                                           IFirebasePushNotification firebasePushNotification,
                                                           IPushNotificationRepository pushNotificationRepository)
        {
            _logger = logger;
            _firebasePushNotification = firebasePushNotification;
            _pushNotificationRepository = pushNotificationRepository;
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

            string pushNotificationToken = _pushNotificationRepository.GetTokenByUserId(@event.UserId);
            if (string.IsNullOrEmpty(pushNotificationToken))// Eğer token bilgisi boş ise bildirimler kapalıdır
                return;

            PushNotificationMessageModel notification = null;

            switch (@event.PushNotificationType)
            {
                case PushNotificationTypes.Reward:
                    notification = RewardAsync(@event.CurrentUserLanguage);
                    break;
            }
            if (notification == null)
                return;

            var response = await _firebasePushNotification.SendPushAsync(new PushNotificationModel
            {
                To = pushNotificationToken,
                Notification = notification
            });
            if (response == null)
            {
                _logger.LogInformation("Firebase push notification gönderme response null döndü");

                return;
            }

            if (response.Failure && response.Results.Any(x => x.Error == "NotRegistered"))// Eğer NotRegistered dönerse firebase token geçerli değildir o yüzden databaseden sildik
            {
                _pushNotificationRepository.RemoveAsync(@event.UserId);
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
                Icon = DefaultImages.Logo
            };
        }

        #endregion Methods
    }
}
