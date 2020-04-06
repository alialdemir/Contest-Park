using ContestPark.Mobile.Services.Notification;
using Matcha.BackgroundService;
using System;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Jobs
{
    public class RewardJob : IPeriodicTask
    {
        #region Private variables

        private readonly INotificationService _notificationService;

        #endregion Private variables

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
        public bool IsInitialize { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// En son aldığı günlük ödülden birsonraki ödülün süresi gelince push notification göndermesi için servere istek atar
        /// </summary>
        public async Task<bool> StartJob()
        {
            if (IsInitialize)
            {
                IsInitialize = false;

                await _notificationService.PushSendAsync(Enums.PushNotificationTypes.Reward);
            }

            IsInitialize = true;

            return IsInitialize; //return false when you want to stop or trigger only once
        }

        #endregion Methods
    }
}
