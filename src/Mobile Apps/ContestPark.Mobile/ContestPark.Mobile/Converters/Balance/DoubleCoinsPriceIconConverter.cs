using ContestPark.Mobile.Enums;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace ContestPark.Mobile.Converters
{
    public class DoubleCoinsPriceIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (BalanceTypes)value == BalanceTypes.Gold
                ? "doublecoins.png".ToResourceImage()
                : "doublecoinstl.png".ToResourceImage();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }
    }
}
