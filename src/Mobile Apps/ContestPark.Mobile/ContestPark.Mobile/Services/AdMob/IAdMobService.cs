using System;

namespace ContestPark.Mobile.Services.AdMob
{
    public interface IAdMobService
    {
        event EventHandler OnRewardedVideoAdClosed;

        void LoadInterstitialVideo();

        void ShowInterstitial();

        void ShowOrLoadRewardedVideo();
    }
}
