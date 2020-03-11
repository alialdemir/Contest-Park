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
        public bool IsNotificationBeAdded(NotificationTypes notificationType, int postId, string whoId, string link)
        {
            string sql = @"SELECT 1 FROM Notifications n
                           WHERE TIME_TO_SEC(TIMEDIFF(NOW(), n.CreatedDate)) < 120
                           AND n.NotificationId = @notificationType
                           AND n.PostId = @postId
                           AND n.WhoId = @whoId
                           AND n.Link = @link";

            return _notificationRepsoitory.QuerySingleOrDefault<bool>(sql, new
            {
                notificationType,
                postId,
                whoId,
                link
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
