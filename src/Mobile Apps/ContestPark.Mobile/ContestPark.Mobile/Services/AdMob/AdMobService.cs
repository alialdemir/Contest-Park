using Acr.UserDialogs;
using ContestPark.Mobile.Configs;
using MarcTron.Plugin;
using MarcTron.Plugin.Interfaces;
using System;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.AdMob
{
    public class AdMobService : IAdMobService
    {
        #region Constructor

        public AdMobService()
        {
            Admob.UserPersonalizedAds = true;

            LoadEvents();
        }

        #endregion Constructor

        #region Properties

        private IMTAdmob Admob
        {
            get
            {
                return CrossMTAdmob.Current;
            }
        }

        #endregion Properties

        #region Events

        public event EventHandler OnRewardedVideoAdClosed;

        private void LoadEvents()
        {
            Admob.OnRewarded += (o, e) =>
              {
                  UserDialogs.Instance.ShowLoading("", MaskType.Black);
              };

            Admob.OnRewardedVideoAdLoaded += (o, e) =>
            {
                Admob.ShowRewardedVideo();

                UserDialogs.Instance.HideLoading();
            };

            Admob.OnRewardedVideoAdFailedToLoad += (o, e) =>
              {
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
            if (!CrossMTAdmob.IsSupported)
                return;

            Task.Factory.StartNew(() =>
            {
                if (!Admob.IsRewardedVideoLoaded())
                    Admob.LoadRewardedVideo(GlobalSetting.RewardedVideoUnitId);
                else
                    Admob.ShowRewardedVideo();
            });
        }

        /// <summary>
        /// Tam ekran reklma yükle
        /// </summary>
        public void LoadInterstitialVideo()
        {
            if (!CrossMTAdmob.IsSupported)
                return;

            Task.Factory.StartNew(() =>
            {
                if (!Admob.IsInterstitialLoaded())
                    Admob.LoadInterstitial(GlobalSetting.InterstitialUnitId);
            });
        }

        /// <summary>
        /// Tam ekran reklam göster
        /// </summary>
        public void ShowInterstitial()
        {
            if (!CrossMTAdmob.IsSupported)
                return;

            Task.Factory.StartNew(() =>
            {
                Admob.ShowInterstitial();
            });
        }

        #endregion Methods
    }
}
