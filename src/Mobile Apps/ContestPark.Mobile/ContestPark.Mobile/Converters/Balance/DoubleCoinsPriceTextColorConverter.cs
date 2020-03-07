using System;
using System.Globalization;
using Xamarin.Forms;

namespace ContestPark.Mobile.Converters
{
    public class DoubleCoinsPriceTextColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Color.FromHex((Enums.Coins)value == Enums.Coins.Negative ? "#FB1A1A" : "#8EF0A7");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }
    }
}
