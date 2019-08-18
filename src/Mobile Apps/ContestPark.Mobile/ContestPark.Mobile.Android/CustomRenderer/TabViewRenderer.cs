// tabbar custom yazılaacağı için kaldırdık

//using Android.Content;
//using Android.Graphics.Drawables;
//using Android.Support.Design.Widget;
//using Android.Support.V4.Graphics.Drawable;
//using ContestPark.Mobile.AppResources;
//using ContestPark.Mobile.Droid.CustomRenderer;
//using ContestPark.Mobile.Views;
//using Plugin.Iconize;
//using Xamarin.Forms;
//using Xamarin.Forms.Platform.Android;

//[assembly: ExportRenderer(typeof(TabView), typeof(TabViewRenderer))]

//namespace ContestPark.Mobile.Droid.CustomRenderer
//{
//    public class TabViewRenderer : IconTabbedPageRenderer
//    {
//        /// <summary>
//        /// Initializes a new instance of the <see cref="IconTabbedPageRenderer"/> class.
//        /// </summary>
//        /// <param name="context">The context.</param>
//        public TabViewRenderer(Context context)
//            : base(context)
//        {
//            SetBackgroundColor(Color.FromHex("#171717").ToAndroid());
//            // Intentionally left blank
//        }

//        protected override void SetTabIconImageSource(TabLayout.Tab tab, Drawable icon)
//        {
//            string iconKey = GetIconKey(tab.Text);
//            var iconize = Iconize.FindIconForKey(iconKey);
//            if (iconize != null)
//            {
//                var drawable = new IconDrawable(Context, iconKey).SizeDp(20);

//                int color = Color.FromHex("#66ffc107").ToAndroid();

//                drawable.Color(color);

//                DrawableCompat.SetTintList(drawable, GetItemIconTintColorState());
//                tab.SetIcon(drawable);
//                return;
//            }
//            base.SetTabIconImageSource(tab, icon);
//        }

//        private string GetIconKey(string pageName)
//        {
//            string categories = ContestParkResources.Categories;

//            if (ContestParkResources.Categories.Equals(pageName))
//            {
//                return "fas-gamepad";
//            }
//            else if (ContestParkResources.Chat.Equals(pageName))
//            {
//                return "fas-comments";
//            }
//            else if (ContestParkResources.Notifications.Equals(pageName))
//            {
//                return "fas-bell";
//            }
//            else if (ContestParkResources.Profile.Equals(pageName))
//            {
//                return "fas-user-circle";
//            }

//            return "";
//        }
//    }
//}