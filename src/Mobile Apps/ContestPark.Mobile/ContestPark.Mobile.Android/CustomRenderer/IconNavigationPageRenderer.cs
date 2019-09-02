using Android.Support.V7.Graphics.Drawable;
using ContestPark.Mobile.Droid.CustomRenderer;
using ContestPark.Mobile.Views;
using Plugin.CurrentActivity;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android.AppCompat;

[assembly: ExportRenderer(typeof(MasterDetailView), typeof(IconNavigationPageRenderer))]

namespace ContestPark.Mobile.Droid.CustomRenderer
{
    [System.Obsolete]
    public class IconNavigationPageRenderer : MasterDetailPageRenderer
    {
        private static Android.Support.V7.Widget.Toolbar GetToolbar() => (CrossCurrentActivity.Current?.Activity as MainActivity)?.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);
            var toolbar = GetToolbar();
            if (toolbar != null)
            {
                for (var i = 0; i < toolbar.ChildCount; i++)
                {
                    var imageButton = toolbar.GetChildAt(i) as Android.Widget.ImageButton;
                    var drawerArrow = imageButton?.Drawable as DrawerArrowDrawable;
                    if (drawerArrow == null)
                        continue;

                    bool displayBack = false;
                    var app = Xamarin.Forms.Application.Current;

                    var detailPage = (app.MainPage as MasterDetailPage).Detail;

                    var navPageLevel = detailPage.Navigation.NavigationStack.Count;
                    if (navPageLevel > 1)
                        displayBack = true;

                    if (!displayBack)
                        ChangeIcon(imageButton, Resource.Drawable.menuicon);
                    if (displayBack)
                        ChangeIcon(imageButton, Resource.Drawable.left_arrow);
                }
            }
        }

        private void ChangeIcon(Android.Widget.ImageButton imageButton, int id)
        {
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Lollipop)
                imageButton.SetImageDrawable(Context.GetDrawable(id));

            imageButton.SetImageResource(id);
        }
    }
}
