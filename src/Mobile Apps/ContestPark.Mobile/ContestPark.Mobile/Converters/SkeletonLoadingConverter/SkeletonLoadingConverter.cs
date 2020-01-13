using System;
using System.Globalization;
using Xamarin.Forms;

namespace ContestPark.Mobile.Converters.SkeletonLoadingConverter
{
    public class SkeletonLoadingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }
    }
}
