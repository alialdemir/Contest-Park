//using ContestPark.Mobile.Dependencies;
//using ContestPark.Mobile.Events;
//using ContestPark.Mobile.iOS.Dependencies;
//using Foundation;
//using Google.MobileAds;
//using System;
//using UIKit;
//using Xamarin.Forms;

//[assembly: Dependency(typeof(AdMob))]

//namespace ContestPark.Mobile.iOS.Dependencies
//{
//    public class AdMob : RewardBasedVideoAdDelegate, IAdmob
//    {
//        #region Private variables

//        private Interstitial _adInterstitial;

//        #endregion Private variables

//        #region Constructor

//        public AdMob()
//        {
//            RewardBasedVideoAd.SharedInstance.Delegate = this;
//        }

//        #endregion Constructor

//        #region Properties

//        public bool UserPersonalizedAds { get; set; }

//        #endregion Properties

//        #region Events

//        public event EventHandler OnRewardedVideoAdLoaded;

//        public event EventHandler OnRewardedVideoAdClosed;

//        public event EventHandler<AdMobEventArgs> OnRewarded;

//        public event EventHandler<AdMobEventArgs> OnRewardedVideoAdFailedToLoad;

//        #endregion Events

//        #region Methods

//        public bool IsInterstitialLoaded()
//        {
//            return _adInterstitial != null && _adInterstitial.IsReady;
//        }

//        public bool IsRewardedVideoLoaded()
//        {
//            return RewardBasedVideoAd.SharedInstance.IsReady;
//        }

//        public void LoadInterstitial(string adUnit)
//        {
//            CreateInterstitialAd(adUnit);

//            var request = Request.GetDefaultRequest();
//            _adInterstitial.LoadRequest(request);
//        }

//        public void LoadRewardedVideo(string adUnit)
//        {
//            if (RewardBasedVideoAd.SharedInstance.IsReady)
//            {
//                OnRewardedVideoAdLoaded?.Invoke(null, null);
//                return;
//            }

//            var request = Request.GetDefaultRequest();
//            RewardBasedVideoAd.SharedInstance.LoadRequest(request, adUnit);
//        }

//        public void ShowInterstitial()
//        {
//            if (_adInterstitial != null && _adInterstitial.IsReady)
//            {
//                var window = UIApplication.SharedApplication.KeyWindow;
//                var vc = UIApplication.SharedApplication.Delegate?.GetWindow()?.RootViewController;
//                while (vc.PresentedViewController != null)
//                {
//                    vc = vc.PresentedViewController;
//                }

//                _adInterstitial.PresentFromRootViewController(vc);
//            }
//        }

//        public void ShowRewardedVideo()
//        {
//            if (RewardBasedVideoAd.SharedInstance.IsReady)
//            {
//                var window = UIApplication.SharedApplication.KeyWindow;
//                var vc = UIApplication.SharedApplication.Delegate?.GetWindow()?.RootViewController;
//                while (vc.PresentedViewController != null)
//                {
//                    vc = vc.PresentedViewController;
//                }

//                RewardBasedVideoAd.SharedInstance.PresentFromRootViewController(vc);
//            }
//        }

//        private void CreateInterstitialAd(string adUnit)
//        {
//            try
//            {
//                if (_adInterstitial != null)
//                {
//                    _adInterstitial = null;
//                }
//            }
//            catch (Exception e)
//            {
//                Console.WriteLine(e);
//            }

//            _adInterstitial = new Interstitial(adUnit);
//        }

//        #endregion Methods

//        #region Event hander

//        public override void DidFailToLoad(RewardBasedVideoAd rewardBasedVideoAd, NSError error)
//        {
//            OnRewardedVideoAdFailedToLoad?.Invoke(rewardBasedVideoAd, new AdMobEventArgs() { ErrorCode = (int)error.Code });
//        }

//        public override void DidClose(RewardBasedVideoAd rewardBasedVideoAd)
//        {
//            OnRewardedVideoAdClosed?.Invoke(rewardBasedVideoAd, new EventArgs());
//        }

//        public override void DidRewardUser(RewardBasedVideoAd rewardBasedVideoAd, AdReward reward)
//        {
//            OnRewarded?.Invoke(rewardBasedVideoAd, new AdMobEventArgs() { RewardAmount = (int)reward.Amount, RewardType = reward.Type });
//        }

//        public override void DidReceiveAd(RewardBasedVideoAd rewardBasedVideoAd)
//        {
//            OnRewardedVideoAdLoaded?.Invoke(rewardBasedVideoAd, new EventArgs());
//        }

//        #endregion Event hander
//    }
//}
