using ContestPark.Mobile.iOS.CustomRenderer;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ContentPage), typeof(BackButtonRenderer))]
[assembly: ExportRenderer(typeof(NavigationPage), typeof(BackButtonNav))]

namespace ContestPark.Mobile.iOS.CustomRenderer
{
    public class BackButtonRenderer : PageRenderer
    {
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (NavigationController != null)
                NavigationController.TopViewController.NavigationItem.BackBarButtonItem = new UIBarButtonItem("", UIBarButtonItemStyle.Plain, null);
        }
    }

    public class BackButtonNav : NavigationRenderer
    {
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            NavigationBar.TopItem.BackBarButtonItem = new UIBarButtonItem("", UIBarButtonItemStyle.Plain, null);
        }
    }
}
