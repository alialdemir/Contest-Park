using Android.Content;
using Android.Graphics.Drawables;
using ContestPark.Mobile.Components;
using ContestPark.Mobile.Droid.CustomRenderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(LinearGradientButton), typeof(LinearGradientButtonRenderer))]

namespace ContestPark.Mobile.Droid.CustomRenderer
{
    public class LinearGradientButtonRenderer : ButtonRenderer
    {
        public LinearGradientButtonRenderer(Context context) : base(context)

        {
        }

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
            if (button.CornerRadiusPosition == LinearGradientButton.CornerRadiusPositions.Bottom)
            {
                float[] radius = new float[8];
                radius[0] = 0;   //Top Left corner
                radius[1] = 0;   //Top Left corner
                radius[2] = 0;     //Top Right corner
                radius[3] = 0;     //Top Right corner
                radius[4] = button.CornerRadius;     //Bottom Right corner
                radius[5] = button.CornerRadius;     //Bottom Right corner
                radius[6] = button.CornerRadius;   //Bottom Left corner
                radius[7] = button.CornerRadius;   //Bottom Left corner
                gd.SetCornerRadii(radius);
            }
            else if (button.CornerRadiusPosition == LinearGradientButton.CornerRadiusPositions.Top)
            {
                float[] radius = new float[8];
                radius[0] = button.CornerRadius;   //Top Left corner
                radius[1] = button.CornerRadius;   //Top Left corner
                radius[2] = button.CornerRadius;     //Top Right corner
                radius[3] = button.CornerRadius;     //Top Right corner
                radius[4] = 0;    //Bottom Right corner
                radius[5] = 0;     //Bottom Right corner
                radius[6] = 0;   //Bottom Left corner
                radius[7] = 0;   //Bottom Left corner
                gd.SetCornerRadii(radius);
            }
            else if (button.CornerRadiusPosition == LinearGradientButton.CornerRadiusPositions.None)
            {
                gd.SetCornerRadius(button.CornerRadius);
            }

            // gd.SetCornerRadius(button.CornerRadius);

            if (button.IsUpperCase)
            {
                e.NewElement.Text = button.Text.ToUpper();
            }

            //apply the button background to newly created drawable gradient
            Control.SetBackground(gd);
        }
    }
}
