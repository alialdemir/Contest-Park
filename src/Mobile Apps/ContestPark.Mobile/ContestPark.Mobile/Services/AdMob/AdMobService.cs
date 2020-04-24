using Acr.UserDialogs;
using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Dependencies;
using Microsoft.AppCenter.Crashes;
using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace ContestPark.Mobile.Services.AdMob
{
    public class AdMobService : IAdMobService
    {
        #region Constructor

        public AdMobService()
        {
            if (Admob != null)
            {
                Admob.UserPersonalizedAds = true;
            }
            LoadEvents();
        }

        #endregion Constructor

        #region Properties

        private IAdmob Admob
        {
            get
            {
                return DependencyService.Get<IAdmob>();
            }
        }

        #endregion Properties

        #region Events

        public event EventHandler OnRewardedVideoAdClosed;

        private void LoadEvents()
        {
            if (Admob == null)
                return;

            Admob.OnRewarded += (o, e) =>
              {
                  Debug.WriteLine("OnRewarded");
              };

            Admob.OnRewardedVideoAdLoaded += (o, e) =>
            {
                Debug.WriteLine("OnRewardedVideoAdLoaded");
                Admob.ShowRewardedVideo();

                UserDialogs.Instance.HideLoading();
            };

            Admob.OnRewardedVideoAdFailedToLoad += (o, e) =>
            {
                Debug.WriteLine("OnRewardedVideoAdFailedToLoad");
                UserDialogs.Instance.HideLoading();
            };

            Admob.OnRewardedVideoAdClosed += (o, e) => OnRewardedVideoAdClosed?.Invoke(o, e);
        }

        #endregion Events

        #region Methods

        /// <summary>
        /// Ödüllü reklam yükler
        /// </summary>
        public void ShowOrLoadRewardedVideo()
        {
            if (Admob == null)
                return;
            try
            {
                if (!Admob.IsRewardedVideoLoaded())
                    Admob.LoadRewardedVideo(GlobalSetting.RewardedVideoUnitId);
                else
                    Admob.ShowRewardedVideo();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        /// <summary>
        /// Tam ekran reklma yükle
        /// </summary>
        public void LoadInterstitialVideo()
        {
            if (Admob == null)
                return;
            try
            {
                if (!Admob.IsInterstitialLoaded())
                    Admob.LoadInterstitial(GlobalSetting.InterstitialUnitId);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        /// <summary>
        /// Tam ekran reklam göster
        /// </summary>
        public void ShowInterstitial()
        {
            if (Admob == null)
                return;
            try
            {
                Admob.ShowInterstitial();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        #endregion Methods
    }
}
