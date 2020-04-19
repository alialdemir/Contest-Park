using ContestPark.Mobile.Jobs;
using ContestPark.Mobile.Services.Notification;
using Shiny.Jobs;
using System;

namespace ContestPark.Mobile.Services.BackgroundAggregator
{
    public class BackgroundAggregatorService : IBackgroundAggregatorService
    {
        #region Private variables

        private readonly INotificationService _notificationService;
        private readonly IJobManager _jobManager;

        #endregion Private variables

        #region Constructor

        public BackgroundAggregatorService(INotificationService notificationService,
                                           IJobManager jobManager)
        {
            _notificationService = notificationService;
            _jobManager = jobManager;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Bir sonraki ödül zamanı gelince push notification atması için background task çalıştırır
        /// </summary>
        /// <param name="nextRewardTime">Job çalışma zamanı</param>
        public void StartRewardJob(TimeSpan nextRewardTime)
        {
            JobInfo job = new JobInfo(typeof(RewardJob), nameof(RewardJob))
            {
                PeriodicTime = nextRewardTime,
                BatteryNotLow = false,
                DeviceCharging = false,
                RequiredInternetAccess = InternetAccess.Any,
            };

            _jobManager.Schedule(job);
        }

        #endregion Methods
    }
}
