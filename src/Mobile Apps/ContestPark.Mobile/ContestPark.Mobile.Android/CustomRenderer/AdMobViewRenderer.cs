using Android.Content;
using Android.Gms.Ads;
using Android.OS;
using Android.Widget;
using ContestPark.Mobile.Components.AdMob;
using ContestPark.Mobile.Droid.CustomRenderer;
using Google.Ads.Mediation.Admob;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(AdMobView), typeof(AdMobViewRenderer))]

namespace ContestPark.Mobile.Droid.CustomRenderer
{
    public class AdMobViewRenderer : ViewRenderer<AdMobView, AdView>
    {
        private string _adUnitId = string.Empty;
        private readonly AdSize _adSize = AdSize.SmartBanner;
        private AdView _adView;

        public AdMobViewRenderer(Context context) : base(context)
        {
        }

        private void CreateNativeControl(AdMobView myMtAdView, string adsId, bool? personalizedAds)
        {
            if (_adView != null)
                return;

            _adUnitId = adsId;

            if (string.IsNullOrEmpty(_adUnitId))
            {
                Console.WriteLine("You must set the adsID before using it");
            }

            _adView = new AdView(Context)
            {
                AdSize = _adSize,
                AdUnitId = _adUnitId,
                LayoutParameters = new LinearLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent)
            };

            var requestBuilder = new AdRequest.Builder();

            if ((personalizedAds.HasValue && personalizedAds.Value) || myMtAdView.UserPersonalizedAds)
            {
                _adView.LoadAd(requestBuilder.Build());
            }
            else
            {
                Bundle bundleExtra = new Bundle();
                bundleExtra.PutString("npa", "1");

                _adView.LoadAd(requestBuilder
                    .AddNetworkExtrasBundle(Java.Lang.Class.FromType(typeof(AdMobAdapter)), bundleExtra)
                    .Build());
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<AdMobView> e)
        {
            base.OnElementChanged(e);
            if (Control == null)
            {
                CreateNativeControl(e.NewElement, e.NewElement.AdUnitId, e.NewElement.UserPersonalizedAds);
                SetNativeControl(_adView);
            }
        }
    }
}
