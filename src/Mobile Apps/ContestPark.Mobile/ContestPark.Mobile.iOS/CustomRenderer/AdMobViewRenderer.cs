using ContestPark.Mobile.Components.AdMob;
using ContestPark.Mobile.iOS.CustomRenderer;
using CoreGraphics;
using Foundation;
using Google.MobileAds;
using System;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(AdMobView), typeof(AdMobViewRenderer))]

namespace ContestPark.Mobile.iOS.CustomRenderer
{
    public class AdMobViewRenderer : ViewRenderer<AdMobView, BannerView>
    {
        private string _adUnitId = string.Empty;
        private BannerView _adView;

        private void CreateNativeControl(UIViewController controller, AdMobView myMtAdView, string adsId, bool? personalizedAds, bool needToRefreshAdView)
        {
            if (_adView != null && !needToRefreshAdView)
                return;

            _adUnitId = adsId;

            if (string.IsNullOrEmpty(_adUnitId))
            {
                Console.WriteLine("You must set the adsID before using it");
            }

            _adView = new BannerView(AdSizeCons.SmartBannerPortrait,
                new CGPoint(0, UIScreen.MainScreen.Bounds.Size.Height - AdSizeCons.Banner.Size.Height))
            {
                AdUnitId = _adUnitId,
                RootViewController = controller
            };

            if ((personalizedAds.HasValue && personalizedAds.Value) || myMtAdView.UserPersonalizedAds)
            {
                _adView.LoadRequest(GetRequest());
            }
            else
            {
                var request = GetRequest();
                var extras = new Extras { AdditionalParameters = NSDictionary.FromObjectAndKey(new NSString("1"), new NSString("npa")) };
                request.RegisterAdNetworkExtras(extras);
                _adView.LoadRequest(request);
            }
        }

        private Request GetRequest()
        {
            var request = Request.GetDefaultRequest();

            return request;
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

        protected override void OnElementChanged(ElementChangedEventArgs<AdMobView> e)
        {
            base.OnElementChanged(e);

            if (_adView != null)
                return;

            if (Control == null)
            {
                UIViewController controller = GetVisibleViewController();
                if (controller != null)
                {
                    if (e.NewElement != null)
                        CreateNativeControl(controller, e.NewElement, e.NewElement.AdUnitId, e.NewElement.UserPersonalizedAds, false);
                    else if (e.OldElement != null)
                        CreateNativeControl(controller, e.OldElement, e.OldElement.AdUnitId, e.OldElement.UserPersonalizedAds, true);
                    else
                        return;

                    SetNativeControl(_adView);
                }
            }
        }
    }
}
