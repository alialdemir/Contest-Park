using Android.Graphics.Drawables;
using ContestPark.Mobile.Components;
using ContestPark.Mobile.Droid.CustomRenderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(LinearGradientButton), typeof(LinearGradientButtonRenderer))]

namespace ContestPark.Mobile.Droid.CustomRenderer
{
    [System.Obsolete]
    public class LinearGradientButtonRenderer : ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || Element == null || Control == null)
                return;

            LinearGradientButton button = (LinearGradientButton)Element;

            //Color.parseColor() method allow us to convert
            // a hexadecimal color string to an integer value (int color)
            int[] colors = { button.StartColor.ToAndroid(), button.EndColor.ToAndroid() };

            //create a new gradient color
            GradientDrawable gd = new GradientDrawable(
              GradientDrawable.Orientation.TopBottom, colors);

            gd.SetCornerRadius(button.BorderRadius);

            //apply the button background to newly created drawable gradient
            Control.SetBackgroundDrawable(gd);
        }
    }
}
