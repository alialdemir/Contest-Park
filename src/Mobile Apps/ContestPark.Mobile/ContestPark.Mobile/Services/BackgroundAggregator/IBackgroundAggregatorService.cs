using System;

namespace ContestPark.Mobile.Services.BackgroundAggregator
{
    public interface IBackgroundAggregatorService
    {
        void StartRewardJob(TimeSpan nextRewardTime);
    }
}
