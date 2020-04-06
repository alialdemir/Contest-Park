using ContestPark.Core.Database.Interfaces;
using ContestPark.Notification.API.Models;
using System.Threading.Tasks;

namespace ContestPark.Notification.API.Infrastructure.Repositories.PushNotification
{
    public class PushNotificationRepository : IPushNotificationRepository
    {
        #region Private Variables

        private readonly IRepository<Tables.PushNotification> _pushNotificationRepsoitory;

        #endregion Private Variables

        #region Constructor

        public PushNotificationRepository(IRepository<Tables.PushNotification> pushNotificationRepsoitory)
        {
            _pushNotificationRepsoitory = pushNotificationRepsoitory;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Token bilgisini siler
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public void RemoveAsync(string userId)
        {
            _pushNotificationRepsoitory.RemoveAsync(userId);
        }

        /// <summary>
        /// Eğer push notification kaydı varsa günceller yoksa yeni kayıt ekler
        /// </summary>
        /// <param name="tokenModel">Firebase push notification token ve kullanıcı id</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public async Task<bool> UpdateTokenByUserIdAsync(PushNotificationTokenModel tokenModel)
        {
            Tables.PushNotification pushNotification = _pushNotificationRepsoitory.FindById(tokenModel.UserId);
            if (pushNotification == null)// Eğer daha önce push notification kaydı eklenmemiş ise insert yapsın
            {
                string newUserId = await _pushNotificationRepsoitory.AddAsync<string>(new Tables.PushNotification
                {
                    UserId = tokenModel.UserId,
                    Token = tokenModel.Token
                });

                return tokenModel.UserId == newUserId;
            }

            if (pushNotification.Token == tokenModel.Token)// zaten token kaydı varsa güncellemesine gerek yok
                return true;

            pushNotification.Token = tokenModel.Token;

            return await _pushNotificationRepsoitory.UpdateAsync(pushNotification);
        }

        /// <summary>
        /// Kullanıcı id'sine ait firebase push notification token verir
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <returns>Push  notification token</returns>
        public string GetTokenByUserId(string userId)
        {
            string sql = @"SELECT pn.Token FROM PushNotifications pn
                           WHERE pn.UserId = @userId";

            return _pushNotificationRepsoitory.QuerySingleOrDefault<string>(sql, new
            {
                userId
            });
        }

        #endregion Methods
    }
}
