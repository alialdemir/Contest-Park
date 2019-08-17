using Android.Graphics.Drawables;
using ContestPark.Mobile.Components;
using ContestPark.Mobile.Droid.CustomRenderer;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomEntry), typeof(CustomEntryRenderer))]

namespace ContestPark.Mobile.Droid.CustomRenderer
{
    [Obsolete]
    public class CustomEntryRenderer : EntryRenderer
    {
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
