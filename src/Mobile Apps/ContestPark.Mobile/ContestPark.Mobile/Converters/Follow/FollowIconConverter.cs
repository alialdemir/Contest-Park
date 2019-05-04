using System;
using System.Globalization;
using Xamarin.Forms;

namespace ContestPark.Mobile.Converters.Follow
{
    public class FollowIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "fas-user-check" : "fas-user-plus";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }
    }
}