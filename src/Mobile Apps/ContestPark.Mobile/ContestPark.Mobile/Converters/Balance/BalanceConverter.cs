using System;
using System.Globalization;
using Xamarin.Forms;

namespace ContestPark.Mobile.Converters.Balance
{
    public class BalanceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || System.Convert.ToDecimal(value) == ((decimal)0.00))
                return "+ 0 TL";

            return string.Format("+ {0:##.##}", value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConvertBack(value, targetType, parameter, culture);
        }
    }
}
