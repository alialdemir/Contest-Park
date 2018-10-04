using System;
using System.Globalization;
using Xamarin.Forms;

namespace ContestPark.Mobile.Converters
{
    public class ImageSourceConterter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || String.IsNullOrEmpty(value.ToString())) return value;
            string picturePath = value.ToString();
            if (!String.IsNullOrEmpty(picturePath) && picturePath.IndexOf("htttp://") >= 0)
            {
                return new UriImageSource
                {
                    Uri = new Uri(picturePath),
                    CachingEnabled = true,
                    CacheValidity = new TimeSpan(0, 0, 30, 0)
                };
            }
            else if (!String.IsNullOrEmpty(picturePath)) return ImageSource.FromFile(picturePath);
            return picturePath;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}