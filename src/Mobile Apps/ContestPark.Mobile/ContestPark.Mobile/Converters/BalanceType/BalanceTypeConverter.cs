using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Enums;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace ContestPark.Mobile.Converters.BalanceType
{
    public class BalanceTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((BalanceTypes)value) == BalanceTypes.Gold ? ContestParkResources.Gold : ContestParkResources.Dolar;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }
    }
}
