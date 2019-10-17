using Android.Content;
using Android.Graphics.Drawables;
using ContestPark.Mobile.Components;
using ContestPark.Mobile.Droid.CustomRenderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomEntry), typeof(CustomEntryRenderer))]

namespace ContestPark.Mobile.Droid.CustomRenderer
{
    public class CustomEntryRenderer : EntryRenderer
    {
        public CustomEntryRenderer(Context context) : base(context)

        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {
                CustomEntry entry = (CustomEntry)Element;

                //Control.SetBackgroundResource(Resource.Layout.rounded_shape);
                var gradientDrawable = new GradientDrawable();
                gradientDrawable.SetCornerRadius(entry.CornerRadius);
                gradientDrawable.SetStroke(entry.BorderWidth, entry.BorderColor.ToAndroid());
                gradientDrawable.SetColor(entry.BackgroundColor.ToAndroid());
                Control.SetBackground(gradientDrawable);

                Control.SetPadding(Control.PaddingLeft, Control.PaddingTop, Control.PaddingRight,
                    Control.PaddingBottom);
            }
        }
    }
}
