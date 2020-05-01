using System;
using System.Reflection;
using Xamarin.Forms;

namespace ContestPark.Mobile
{
    public abstract class ImageSource : Element
    {
        public static Xamarin.Forms.ImageSource FromResource(string resource)
        {
            return Xamarin.Forms.ImageSource.FromResource($"ContestPark.Mobile.Common.Images.{resource}", typeof(ImageSource).GetTypeInfo().Assembly);
        }

        public static Xamarin.Forms.ImageSource FromUri(Uri uri)
        {
            return Xamarin.Forms.ImageSource.FromUri(uri);
        }
    }
}
