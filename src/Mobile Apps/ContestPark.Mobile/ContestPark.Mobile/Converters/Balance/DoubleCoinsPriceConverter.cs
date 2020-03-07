using System;
using System.Globalization;
using Xamarin.Forms;

namespace ContestPark.Mobile.Converters
{
    public class DoubleCoinsPriceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((Enums.Coins)value == Enums.Coins.Negative ? "-" : "+");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }
    }
}
