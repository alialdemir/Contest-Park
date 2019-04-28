using Android.Content;
using Android.Support.Design.Widget;
using Android.Support.V4.Graphics.Drawable;
using ContestPark.Mobile.Droid.CustomRenderer;
using ContestPark.Mobile.Views;
using Plugin.Iconize;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(TabView), typeof(TabViewRenderer))]

namespace ContestPark.Mobile.Droid.CustomRenderer
{
    public class TabViewRenderer : IconTabbedPageRenderer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IconTabbedPageRenderer"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public TabViewRenderer(Context context)
            : base(context)
        {
            // Intentionally left blank
        }

        /// <inheritdoc />
        protected override void SetTabIcon(TabLayout.Tab tab, FileImageSource icon)
        {
            var iconize = Iconize.FindIconForKey(icon.File);
            if (iconize != null)
            {
                var drawable = new IconDrawable(Context, icon).SizeDp(20);

                int color = Color.FromHex("#66ffc107").ToAndroid();

                drawable.Color(color);

                DrawableCompat.SetTintList(drawable, GetItemIconTintColorState());
                tab.SetIcon(drawable);
                return;
            }

            base.SetTabIcon(tab, icon);
        }
    }
}