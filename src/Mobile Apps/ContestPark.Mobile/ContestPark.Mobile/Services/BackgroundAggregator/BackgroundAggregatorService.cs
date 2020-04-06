using ContestPark.Mobile.Jobs;
using ContestPark.Mobile.Services.Notification;
using System;

namespace ContestPark.Mobile.Services.BackgroundAggregator
{
    public class BackgroundAggregatorService : IBackgroundAggregatorService
    {
        #region Private variables

        private readonly INotificationService _notificationService;

        #endregion Private variables

        #region Constructor

        public BackgroundAggregatorService(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Bir sonraki ödül zamanı gelince push notification atması için background task çalıştırır
        /// </summary>
        /// <param name="nextRewardTime">Job çalışma zamanı</param>
        public void StartRewardJob(TimeSpan nextRewardTime)
        {
            Matcha.BackgroundService.BackgroundAggregatorService.Add(() => new RewardJob(nextRewardTime, _notificationService));
        }

        #endregion Methods
    }
}
