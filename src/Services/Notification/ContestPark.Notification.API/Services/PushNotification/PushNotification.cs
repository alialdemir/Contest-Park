using ContestPark.Core.Services.RequestProvider;
using ContestPark.Notification.API.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace ContestPark.Notification.API.Services.PushNotification
{
    public class PushNotification : IPushNotification
    {
        #region Private variables

        private readonly IRequestProvider _requestProvider;
        private readonly ILogger<PushNotification> _logger;
        private readonly NotificationSettings _notificationSettings;

        #endregion Private variables

        #region Constructor

        public PushNotification(IRequestProvider requestProvider,
                                ILogger<PushNotification> logger,
                                IOptions<NotificationSettings> settings)
        {
            _requestProvider = requestProvider;
            _logger = logger;
            _notificationSettings = settings.Value;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Bildirim gönder
        /// </summary>
        /// <returns>Gönderilmiş ise true gönderilmemiş ise false</returns>
        public async Task<bool> SendPushAsync(PushNotificationMessageModel pushNotification)
        {
            OneSignalPushNotificationModel data = new OneSignalPushNotificationModel
            {
                AppId = _notificationSettings.OneSignalAppId,
                // uygulama tarafına data göndermek için bunu kullan Data = new { params = "data" }
                Headings = new
                {
                    en = pushNotification.Title
                },
                Contents = new
                {
                    en = pushNotification.Text
                },
            };

            if (pushNotification.ScheduleDate.HasValue)
            {
                string scheduleDate = $"{pushNotification.ScheduleDate.Value:yyyy-MM-dd hh-mm-ss tt} UTC+3:00";// türkiye için utc +3 kullanıldı

                data.SendAfter = scheduleDate;
            }

            if (!string.IsNullOrEmpty(pushNotification.UserId))
            {
                data.Filters = new object[]
                {
                    new {
                        field = "tag",
                        key = "UserId",
                        value = pushNotification.UserId
                    },
                };
            }

            PushNotificationResponseModel response = await _requestProvider.PostAsync<PushNotificationResponseModel>(_notificationSettings.OneSignalSendNotificationUrl,
                                                                                                                     data,
                                                                                                                     _notificationSettings.OneSignalApiKey);

            return response != null && response.Recipients > 0;
        }

        #endregion Methods
    }
}
