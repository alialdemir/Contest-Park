using ContestPark.Mobile.Enums;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace ContestPark.Mobile.Converters
{
    public class TabBorderColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return Color.Transparent;

            return (BalanceTypes)value == ((BalanceTypes)System.Convert.ToByte(parameter)) ? Color.FromHex("#7CF5BA") : Color.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }
    }
}
