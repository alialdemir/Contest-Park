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
            if (e.OldElement != null || e.NewElement == null)
                return;

            if (_version > new Version(6, 0))
            {   //n iOS 6 and earlier, the image displayed when the switch is in the on position.
                Control.OnImage = UIImage.FromFile("switchactive.png");
                //n iOS 6 and earlier, the image displayed when the switch is in the off position.
                Control.OffImage = UIImage.FromFile("switchpassive.png");
            }
            else
            {
                Control.ThumbTintColor = Color.Default.ToUIColor();
            }

            //The color used to tint the appearance of the thumb.
            Control.ThumbTintColor = Color.Default.ToUIColor();
            //Control.BackgroundColor = view.SwitchBGColor.ToUIColor();
            //The color used to tint the appearance of the switch when it is turned on.
            Control.OnTintColor = Color.Default.ToUIColor();
            //The color used to tint the outline of the switch when it is turned off.
            Control.TintColor = Color.Default.ToUIColor();
        }
    }
}
