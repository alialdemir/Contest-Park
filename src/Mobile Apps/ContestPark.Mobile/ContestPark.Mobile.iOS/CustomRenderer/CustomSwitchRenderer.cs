using ContestPark.Mobile.iOS.CustomRenderer;
using System;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Switch), typeof(CustomSwitchRenderer))]

namespace ContestPark.Mobile.iOS.CustomRenderer
{
    internal class CustomSwitchRenderer : SwitchRenderer
    {
        private readonly Version _version = new Version(ObjCRuntime.Constants.Version);

        protected override void OnElementChanged(ElementChangedEventArgs<Switch> e)
        {
            base.OnElementChanged(e);
            if (Control == null || e.OldElement != null || e.NewElement == null)
                return;

            //n iOS 6 and earlier, the image displayed when the switch is in the on position.
            Control.OnImage = UIImage.FromFile("switchactive.png");
            //n iOS 6 and earlier, the image displayed when the switch is in the off position.
            Control.OffImage = UIImage.FromFile("switchpassive.png");
        }
    }
}
