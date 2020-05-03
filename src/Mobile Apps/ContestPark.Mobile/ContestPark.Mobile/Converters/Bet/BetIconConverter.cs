using System;
using System.Globalization;
using Xamarin.Forms;

namespace ContestPark.Mobile.Converters.Bet
{
    public class BetIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            return value.ToString().ToResourceImage();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }
    }
}
