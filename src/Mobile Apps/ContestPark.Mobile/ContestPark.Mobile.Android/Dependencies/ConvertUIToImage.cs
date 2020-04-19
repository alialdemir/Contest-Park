using Android.Graphics;
using Android.Views;
using ContestPark.Mobile.Dependencies;
using ContestPark.Mobile.Droid.Dependencies;
using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using droid = Android;

[assembly: Dependency(typeof(ConvertUIToImage))]

namespace ContestPark.Mobile.Droid.Dependencies
{
    public class ConvertUIToImage : IConvertUIToImage
    {
        private double Height { get; set; } = 400;
        private double Width { get; set; } = 400;

        [Obsolete]
        public string GetImagePathByPage(ContentView contentPage)
        {
            if (contentPage == null)
                throw new ArgumentNullException(nameof(contentPage));

            if (contentPage.HeightRequest > 0)
                Height = contentPage.HeightRequest;

            if (contentPage.WidthRequest > 0)
                Width = contentPage.WidthRequest;

            //Converting forms page to native view
            ViewGroup androidView = ConvertFormsToNative(contentPage.Content, new Rectangle(0, 0, Width, Height));

            // Converting View to BitMap
            var bitmap = ConvertViewToBitMap(androidView);

            // Saving image in mobile local storage
            return SaveImage(bitmap);
        }

        [Obsolete]
        private ViewGroup ConvertFormsToNative(Xamarin.Forms.View view, Rectangle size)
        {
            var vRenderer = Platform.CreateRenderer(view);

            var viewGroup = vRenderer.ViewGroup;

            vRenderer.Tracker.UpdateLayout();
            var layoutParams = new ViewGroup.LayoutParams((int)Width, (int)Height);
            viewGroup.LayoutParameters = layoutParams;
            view.Layout(size);
            viewGroup.Layout(0, 0, (int)Width, (int)Height);

            return viewGroup;
        }

        private Bitmap ConvertViewToBitMap(ViewGroup view)
        {
            Bitmap bitmap = Bitmap.CreateBitmap((int)Width, (int)Height, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(bitmap);
            canvas.DrawColor(droid.Graphics.Color.Black);
            view.Draw(canvas);

            return bitmap;
        }

        private string SaveImage(Bitmap bitmap)
        {
            var sdCardPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            // droid.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            var fileName = System.IO.Path.Combine(sdCardPath, DateTime.Now.ToFileTime() + ".png");

            using (var os = new FileStream(fileName, FileMode.CreateNew))
            {
                bitmap.Compress(Bitmap.CompressFormat.Png, 95, os);
            }

            return fileName;
        }
    }
}
