using System.Reflection;

namespace ContestPark.Mobile
{
    public static class ImageSourceExtension
    {
        public static Xamarin.Forms.ImageSource ToResourceImage(this string resource)
        {
            return Xamarin.Forms.ImageSource.FromResource($"ContestPark.Mobile.Common.Images.{resource}", typeof(ImageSourceExtension).GetTypeInfo().Assembly);
        }
    }
}
