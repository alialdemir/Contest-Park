using System;
using System.Globalization;
using Xamarin.Forms;

namespace ContestPark.Mobile.Converters.CategoryDetail
{
    public class ZeroFalseConventer : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return int.TryParse(value.ToString(), out int zero) && !(zero == 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConvertBack(value, targetType, parameter, culture);
        }
    }
}