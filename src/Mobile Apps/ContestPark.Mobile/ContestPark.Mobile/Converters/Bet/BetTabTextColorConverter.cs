using ContestPark.Mobile.Enums;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace ContestPark.Mobile.Converters
{
    public class BetTabTextColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((BalanceTypes)value) == ((BalanceTypes)System.Convert.ToByte(parameter))
                ? (Color)ContestParkApp.Current.Resources["Black"]
                : (Color)ContestParkApp.Current.Resources["White"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }
    }
}
