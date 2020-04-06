using ContestPark.Core.Services.RequestProvider;
using ContestPark.Notification.API.Models;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace ContestPark.Notification.API.Services.FirebasePushNotification
{
    public class FirebasePushNotification : IFirebasePushNotification
    {
        #region Private variables

        private readonly IRequestProvider _requestProvider;

        private readonly NotificationSettings _notificationSettings;

        #endregion Private variables

        #region Constructor

        public FirebasePushNotification(IRequestProvider requestProvider,
                                        IOptions<NotificationSettings> settings)
        {
            _requestProvider = requestProvider ?? throw new ArgumentNullException(nameof(requestProvider));
            _notificationSettings = settings.Value;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Bildirim gönder
        /// </summary>
        /// <returns>Firebase response</returns>
        public Task<PushNotificationResponseModel> SendPushAsync(PushNotificationModel pushNotification)
        {
            return _requestProvider.PostAsync<PushNotificationResponseModel>(
                _notificationSettings.FirebaseUrl,
                pushNotification,
                _notificationSettings.FirebaseServerKey);
        }

        #endregion Methods
    }
}
