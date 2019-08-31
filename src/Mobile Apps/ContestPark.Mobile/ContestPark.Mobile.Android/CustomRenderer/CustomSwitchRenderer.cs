﻿using Android.Graphics;
using ContestPark.Mobile.Droid.CustomRenderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
[assembly: ExportRenderer(typeof(Xamarin.Forms.Switch), typeof(CustomSwitchRendererd))]

namespace ContestPark.Mobile.Droid.CustomRenderer
{
    [System.Obsolete]
    public class CustomSwitchRendererd : SwitchRenderer
    {
        private Xamarin.Forms.Switch view;
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Switch> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null || e.NewElement == null)
                return;
            view = Element;
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.JellyBean)
            {
                if (this.Control != null)
                {
                    if (this.Control.Checked)
                    {
                        this.Control.TrackDrawable.SetColorFilter(Xamarin.Forms.Color.Default.ToAndroid(), PorterDuff.Mode.SrcAtop);
                    }
                    else
                    {
                        this.Control.TrackDrawable.SetColorFilter(Xamarin.Forms.Color.Default.ToAndroid(), PorterDuff.Mode.SrcAtop);
                    }

                    view.Toggled += View_Toggled;
                    UpdateSwitchThumbImage(view);
                }
            }
        }

        private void View_Toggled(object sender, ToggledEventArgs e)
        {
            if (this.Control.Checked)
            {
                this.Control.TrackDrawable.SetColorFilter(Xamarin.Forms.Color.Default.ToAndroid(), PorterDuff.Mode.SrcAtop);
            }
            else
            {
                this.Control.TrackDrawable.SetColorFilter(Xamarin.Forms.Color.Default.ToAndroid(), PorterDuff.Mode.SrcAtop);
            }
            UpdateSwitchThumbImage(view);
        }

        private void UpdateSwitchThumbImage(Xamarin.Forms.Switch view)
        {

            if (this.Control.Checked)
            {
                Control.SetThumbResource(Resource.Drawable.switchactive);
            }
            else
            {
                Control.SetThumbResource(Resource.Drawable.switchpassive);
            }

        }

        protected override void Dispose(bool disposing)
        {
            view.Toggled -= this.View_Toggled;
            base.Dispose(disposing);
        }
    }
}