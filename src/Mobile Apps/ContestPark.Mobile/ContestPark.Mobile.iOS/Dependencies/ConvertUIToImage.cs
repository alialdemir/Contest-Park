using ContestPark.Mobile.Dependencies;
using ContestPark.Mobile.iOS.Dependencies;
using CoreGraphics;
using Foundation;
using System;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: Dependency(typeof(ConvertUIToImage))]

namespace ContestPark.Mobile.iOS.Dependencies
{
    public class ConvertUIToImage : IConvertUIToImage
    {
        public UIImage UiImage { get; set; }

        public string GetImagePathByPage(ContentPage contentPage)
        {
            var rect = new CGRect(0, 0, 400, 400);
            var iOSView = ConvertFormsToNative(contentPage.Content, rect);

            UiImage = ConvertViewToImage(iOSView);

            var filePath = SaveImage(UiImage);
            UIGraphics.EndImageContext();

            return filePath;
        }

        private UIImage ConvertViewToImage(UIView iOSView)
        {
            UIGraphics.BeginImageContext(iOSView.Frame.Size);
            iOSView.Layer.RenderInContext(UIGraphics.GetCurrentContext());
            UiImage = UIGraphics.GetImageFromCurrentImageContext();
            return UiImage;
        }

        private string SaveImage(UIImage image)
        {
            var documentsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string pngFilename = System.IO.Path.Combine(documentsDirectory, DateTime.Now.ToFileTime() + ".png");
            NSData imgData = image.AsPNG();

            return pngFilename;
        }

        public UIView ConvertFormsToNative(Xamarin.Forms.View view, CGRect size)
        {
            var renderer = Xamarin.Forms.Platform.iOS.Platform.CreateRenderer(view);

            renderer.NativeView.Frame = size;

            renderer.NativeView.AutoresizingMask = UIViewAutoresizing.All;
            renderer.NativeView.ContentMode = UIViewContentMode.ScaleToFill;

            renderer.Element.Layout(size.ToRectangle());

            var nativeView = renderer.NativeView;

            nativeView.SetNeedsLayout();

            return nativeView;
        }
    }
}