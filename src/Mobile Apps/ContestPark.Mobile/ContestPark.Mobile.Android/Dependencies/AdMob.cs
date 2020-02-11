using Android.Gms.Ads;
using Android.Gms.Ads.Reward;
using ContestPark.Mobile.Dependencies;
using ContestPark.Mobile.Droid.Dependencies;
using ContestPark.Mobile.Events;
using System;
using Xamarin.Forms;

[assembly: Dependency(typeof(AdMob))]

namespace ContestPark.Mobile.Droid.Dependencies
{
    public class AdMob : IAdmob
    {
        #region Private variables

        private IRewardedVideoAd _rewardedVideoAd;

        private InterstitialAd _interstitialAd;

        #endregion Private variables

        #region Constructor

        public AdMob()
        {
        }

        #endregion Constructor

        #region Properties

        public bool UserPersonalizedAds { get; set; }

        #endregion Properties

        #region Events

        public event EventHandler OnRewardedVideoAdLoaded;

        public event EventHandler OnRewardedVideoAdClosed;

        public event EventHandler<AdMobEventArgs> OnRewarded;

        public event EventHandler<AdMobEventArgs> OnRewardedVideoAdFailedToLoad;

        #endregion Events

        #region Methods

        public bool IsInterstitialLoaded()
        {
            return _interstitialAd != null && _interstitialAd.IsLoaded;
        }

        public bool IsRewardedVideoLoaded()
        {
            return _rewardedVideoAd != null && _rewardedVideoAd.IsLoaded;
        }

        public void LoadInterstitial(string adUnit)
        {
            if (_interstitialAd == null || _interstitialAd?.AdUnitId != adUnit)
                CreateInterstitialAd(adUnit);

            if (!_interstitialAd.IsLoaded && !_interstitialAd.IsLoading)
            {
                _interstitialAd.LoadAd(GetRequest());
            }
            else
            {
                Console.WriteLine("Interstitial already loaded");
            }
        }

        public void LoadRewardedVideo(string adUnit)
        {
            if (_rewardedVideoAd == null)
                CreateRewardedVideo();

            if (!_rewardedVideoAd.IsLoaded)
            {
                _rewardedVideoAd.LoadAd(adUnit, GetRequest());
            }
            else
            {
                Console.WriteLine("Rewarded Video already loaded");
            }
        }

        private AdRequest GetRequest()
        {
            var request = new AdRequest.Builder();

            return request.Build();
        }

        public void ShowInterstitial()
        {
            if (IsInterstitialLoaded())
            {
                _interstitialAd.Show();
            }
            else
            {
                Console.WriteLine("Interstitial not loaded");
            }
        }

        public void ShowRewardedVideo()
        {
            if (IsRewardedVideoLoaded())
            {
                _rewardedVideoAd.Show();
            }
            else
            {
                Console.WriteLine("Rewarded Video not loaded");
            }
        }

        private void CreateInterstitialAd(string adUnit)
        {
            var context = Android.App.Application.Context;
            _interstitialAd = new InterstitialAd(context) { AdUnitId = adUnit };
        }

        private void CreateRewardedVideo()
        {
            var context = Android.App.Application.Context;
            _rewardedVideoAd = MobileAds.GetRewardedVideoAdInstance(context);

            var rewardListener = new MyRewardedVideoAdListener();
            _rewardedVideoAd.RewardedVideoAdListener = rewardListener;

            rewardListener.OnRewardedVideoAdLoadedEvent += (sender, e) => OnRewardedVideoAdLoaded?.Invoke(sender, e);
            rewardListener.OnRewardedVideoAdClosedEvent += (sender, e) => OnRewardedVideoAdClosed?.Invoke(sender, e); ;
            rewardListener.OnRewardedEvent += (sender, e) => OnRewarded?.Invoke(null, e);
            rewardListener.OnRewardedVideoAdFailedToLoadEvent += (sender, e) => OnRewardedVideoAdFailedToLoad?.Invoke(sender, e);
        }

        #endregion Methods
    }

    public class MyRewardedVideoAdListener : Java.Lang.Object, IRewardedVideoAdListener
    {
        public event EventHandler OnRewardedVideoAdLoadedEvent;

        public event EventHandler OnRewardedVideoAdClosedEvent;

        public event EventHandler<AdMobEventArgs> OnRewardedEvent;

        public event EventHandler<AdMobEventArgs> OnRewardedVideoAdFailedToLoadEvent;

        public void OnRewarded(IRewardItem reward)
        {
            OnRewardedEvent?.Invoke(null, new AdMobEventArgs() { RewardAmount = reward.Amount, RewardType = reward.Type });
        }

        public void OnRewardedVideoAdClosed()
        {
            OnRewardedVideoAdClosedEvent?.Invoke(null, null);
        }

        public void OnRewardedVideoAdFailedToLoad(int errorCode)
        {
            OnRewardedVideoAdFailedToLoadEvent?.Invoke(null, new AdMobEventArgs() { ErrorCode = errorCode });
        }

        public void OnRewardedVideoAdLeftApplication()
        {
        }

        public void OnRewardedVideoAdLoaded()
        {
            OnRewardedVideoAdLoadedEvent?.Invoke(null, null);
        }

        public void OnRewardedVideoAdOpened()
        {
        }

        public void OnRewardedVideoStarted()
        {
        }

        public void OnRewardedVideoCompleted()
        {
        }
    }
}
