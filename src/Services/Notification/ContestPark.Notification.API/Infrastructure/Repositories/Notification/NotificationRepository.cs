using ContestPark.Core.Database.Interfaces;
using ContestPark.Core.Database.Models;
using ContestPark.Core.Enums;
using ContestPark.Notification.API.Enums;
using ContestPark.Notification.API.Models;
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

        /// <summary>
        /// Eğer 120 saniye içinde bildirim eklenmemiş ise true eklenmiş ise false döner
        /// </summary>
        /// <param name="notificationId">Bildirim</param>
        /// <returns>Bildirim eklenebilir mi true/false</returns>
        public bool IsNotificationBeAdded(NotificationTypes notificationType, int postId, string whoId)
        {
            string sql = @"SELECT ISNULL((
                           SELECT case when
                           SECOND(TIMEDIFF(NOW(), n.CreatedDate))  >= 30
                           then NULL
                           ELSE 1
                           END
                           FROM Notifications n
                           WHERE (n.NotificationType = @notificationType
                           AND n.PostId = @postId
                           AND n.WhoId = @whoId)
                           ORDER BY n.CreatedDate DESC
                           LIMIT 0,1))";

            return _notificationRepsoitory.QuerySingleOrDefault<bool>(sql, new
            {
                notificationType = (byte)notificationType,
                postId,
                whoId,
            });
        }

        /// <summary>
        /// Kullanıcı id'ye ait bildirimler
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <param name="langId">Dil seçimi</param>
        /// <param name="pagingModel">Sayfalama</param>
        /// <returns>Bildirim listesi</returns>
        public ServiceModel<NotificationModel> Notifications(string userId, Languages langId, PagingModel pagingModel)
        {
            return _notificationRepsoitory.ToSpServiceModel<NotificationModel>("SP_Notifications", new
            {
                userId,
                langId
            }, pagingModel);
        }

        #endregion Methods
    }
}
