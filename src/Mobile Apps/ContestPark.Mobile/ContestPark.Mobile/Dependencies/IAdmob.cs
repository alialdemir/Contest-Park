using ContestPark.Mobile.Events;
using System;

namespace ContestPark.Mobile.Dependencies
{
    public interface IAdmob
    {
        bool UserPersonalizedAds { get; set; }

        void ShowInterstitial();

        void LoadInterstitial(string adUnit);

        bool IsInterstitialLoaded();

        void LoadRewardedVideo(string adUnit);

        void ShowRewardedVideo();

        bool IsRewardedVideoLoaded();

        event EventHandler OnRewardedVideoAdLoaded;

        event EventHandler OnRewardedVideoAdClosed;

        event EventHandler<AdMobEventArgs> OnRewarded;

        event EventHandler<AdMobEventArgs> OnRewardedVideoAdFailedToLoad;
    }
}
