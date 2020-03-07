using ContestPark.Mobile.AppResources;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace ContestPark.Mobile.Converters
{
    public class AcceptDuelDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Format(ContestParkResources.InvitedYouToADuel, value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConvertBack(value, targetType, parameter, culture);
        }
    }
}
