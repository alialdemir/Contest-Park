using ContestPark.Mobile.Services.Notification;
using Matcha.BackgroundService;
using System;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Jobs
{
    public class RewardJob : IPeriodicTask
    {
        #region Private Variables

        private readonly INotificationService _notificationService;

        #endregion Private Variables

        #region Constructor

        public RewardJob(TimeSpan nextRewardTime,
                         INotificationService notificationService)
        {
            Interval = nextRewardTime;
            _notificationService = notificationService;
        }

        #endregion Constructor

        #region Properties

        public TimeSpan Interval { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// En son aldığı günlük ödülden birsonraki ödülün süresi gelince push notification göndermesi için servere istek atar
        /// </summary>
        public async Task<bool> StartJob()
        {
            await _notificationService.PushSendAsync(Enums.PushNotificationTypes.Reward);

            return false; //return false when you want to stop or trigger only once
        }

        #endregion Methods
    }
}
