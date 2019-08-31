using System;
using System.Globalization;
using Xamarin.Forms;

namespace ContestPark.Mobile.Converters.Quiz.StylishFrameTextColor
{
    public class StylishFrameTextColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((Color)value).ToHex() == "#FFFFFFFF" ? "#3B3B3B" : "#FFFFFF";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }
    }
}
