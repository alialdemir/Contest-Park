using ContestPark.EventBus.Abstractions;
using ContestPark.Notification.API.Infrastructure.Repositories.Notification;
using ContestPark.Notification.API.IntegrationEvents.Events;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Notification.API.IntegrationEvents.EventHandling
{
    public class AddNotificationIntegrationEventHandler : IIntegrationEventHandler<AddNotificationIntegrationEvent>
    {
        #region Private variables

        private readonly ILogger<AddNotificationIntegrationEventHandler> _logger;
        private readonly INotificationRepository _notificationRepository;

        #endregion Private variables

        #region Constructor

        public AddNotificationIntegrationEventHandler(ILogger<AddNotificationIntegrationEventHandler> logger,
                                                      INotificationRepository notificationRepository)
        {
            _logger = logger;
            _notificationRepository = notificationRepository;
        }

        #endregion Constructor

        #region Method

        /// <summary>
        /// Bildirim ekle
        /// </summary>
        /// <param name="event">Bildirim bilgileri</param>
        public async Task Handle(AddNotificationIntegrationEvent @event)
        {
            bool isNotificationBeAdded = _notificationRepository.IsNotificationBeAdded(@event.NotificationType,
                                                                                       @event.PostId ?? 0,
                                                                                       @event.WhoId);
            if (!isNotificationBeAdded)// Hızlı hızlı beğenme comment vs yapıp sürekli bildirim göndermesin diye kontrol koyduk
            {
                _logger.LogInformation("Seri bildirim gönderme işlemi yapılıyor.",
                                       @event.NotificationType,
                                       @event.PostId ?? 0,
                                       @event.WhoId,
                                       @event.Link);
                return;
            }

            var notifications = @event
                                     .WhonIds
                                     .Where(whonId => !string.IsNullOrEmpty(whonId) && whonId != @event.WhoId && !whonId.EndsWith("-bot"))
                                     .Select(whonId => new Infrastructure.Tables.Notification
                                     {
                                         NotificationType = @event.NotificationType,
                                         PostId = @event.PostId,
                                         WhoId = @event.WhoId,
                                         WhonId = whonId,
                                         Link = @event.Link,
                                     }).ToList();

            if (notifications == null && !notifications.Any())
                return;

            bool isSuccess = await _notificationRepository.AddRangeAsync(notifications);
            if (!isSuccess)
            {
                _logger.LogError(
                    "Biildirim ekleme işlemi başarısız oldu.",
                    @event.NotificationType,
                    @event.PostId,
                    @event.WhoId,
                    @event.WhonIds.Join(", "),
                    @event.Link);
            }
        }

        #endregion Method
    }
}
