using ContestPark.Mobile.Dependencies;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.iOS.Dependencies;
using Foundation;
using Google.MobileAds;
using System;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(AdMob))]

namespace ContestPark.Mobile.iOS.Dependencies
{
    public class AdMob : RewardBasedVideoAdDelegate, IAdmob
    {
        #region Private variables

        private Interstitial _adInterstitial;

        #endregion Private variables

        #region Constructor

        public AdMob()
        {
            RewardBasedVideoAd.SharedInstance.Delegate = this;
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
            return _adInterstitial != null && _adInterstitial.IsReady;
        }

        public bool IsRewardedVideoLoaded()
        {
            return RewardBasedVideoAd.SharedInstance.IsReady;
        }

        public void LoadInterstitial(string adUnit)
        {
            CreateInterstitialAd(adUnit);

            var request = Request.GetDefaultRequest();
            _adInterstitial.LoadRequest(request);
        }

        public void LoadRewardedVideo(string adUnit)
        {
            if (RewardBasedVideoAd.SharedInstance.IsReady)
            {
                OnRewardedVideoAdLoaded?.Invoke(null, null);
                return;
            }

            var request = Request.GetDefaultRequest();
            RewardBasedVideoAd.SharedInstance.LoadRequest(request, adUnit);
        }

        private UIViewController GetVisibleViewController()
        {
            var rootController = UIApplication.SharedApplication.Delegate?.GetWindow()?.RootViewController;
            if (rootController == null)
                return null;

            if (rootController.PresentedViewController == null)
                return rootController;

            if (rootController.PresentedViewController is UINavigationController controller)
            {
                return controller.VisibleViewController;
            }

            if (rootController.PresentedViewController is UITabBarController barController)
            {
                return barController.SelectedViewController;
            }

            return rootController.PresentedViewController;
        }

        public void ShowInterstitial()
        {
            if (_adInterstitial != null && _adInterstitial.IsReady)
            {
                var rootController = GetVisibleViewController();
                if (rootController != null)
                {
                    _adInterstitial.Present(rootController);
                }
            }
        }

        public void ShowRewardedVideo()
        {
            if (RewardBasedVideoAd.SharedInstance.IsReady)
            {
                var rootController = GetVisibleViewController();
                if (rootController != null)
                {
                    RewardBasedVideoAd.SharedInstance.Present(rootController);
                }
            }
        }

        private void CreateInterstitialAd(string adUnit)
        {
            try
            {
                if (_adInterstitial != null)
                {
                    _adInterstitial = null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            _adInterstitial = new Interstitial(adUnit);
        }

        #endregion Methods

        #region Event hander

        public override void DidFailToLoad(RewardBasedVideoAd rewardBasedVideoAd, NSError error)
        {
            OnRewardedVideoAdFailedToLoad?.Invoke(rewardBasedVideoAd, new AdMobEventArgs() { ErrorCode = (int)error.Code });
        }

        public override void DidClose(RewardBasedVideoAd rewardBasedVideoAd)
        {
            OnRewardedVideoAdClosed?.Invoke(rewardBasedVideoAd, new EventArgs());
        }

        public override void DidRewardUser(RewardBasedVideoAd rewardBasedVideoAd, AdReward reward)
        {
            OnRewarded?.Invoke(rewardBasedVideoAd, new AdMobEventArgs() { RewardAmount = (int)reward.Amount, RewardType = reward.Type });
        }

        public override void DidReceiveAd(RewardBasedVideoAd rewardBasedVideoAd)
        {
            OnRewardedVideoAdLoaded?.Invoke(rewardBasedVideoAd, new EventArgs());
        }

        #endregion Event hander
    }
}
