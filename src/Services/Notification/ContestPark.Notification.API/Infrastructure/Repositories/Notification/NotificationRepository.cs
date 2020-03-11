using ContestPark.Core.Database.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Notification.API.Infrastructure.Repositories.Notification
{
    public class NotificationRepository : INotificationRepository
    {
        #region Private Variables

        private readonly IRepository<Tables.Notification> _notificationRepsoitory;

        #endregion Private Variables

        #region Constructor

        public NotificationRepository(IRepository<Tables.Notification> notificationRepsoitory)
        {
            _notificationRepsoitory = notificationRepsoitory;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Bildirim ekle
        /// </summary>
        /// <param name="notification">Bildirim</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public async Task<bool> AddRangeAsync(IEnumerable<Tables.Notification> notifications)
        {
            return await _notificationRepsoitory.AddRangeAsync(notifications);
        }

        #endregion Methods
    }
}
