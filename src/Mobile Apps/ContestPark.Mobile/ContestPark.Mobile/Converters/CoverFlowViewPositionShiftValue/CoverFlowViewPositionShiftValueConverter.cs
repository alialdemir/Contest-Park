using System;
using System.Globalization;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ContestPark.Mobile.Converters
{
    public class CoverFlowViewPositionShiftValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DeviceDisplay.MainDisplayInfo.Width < 780 ? 0 : 60;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConvertBack(value, targetType, parameter, culture);
        }
    }
}
