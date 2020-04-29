using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Services.Settings;
using Prism.Ioc;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace ContestPark.Mobile.Converters
{
    internal class FollowButtonVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ISettingsService settingsService = ContestParkApp.Current.Container.Resolve<ISettingsService>();

            return value != null && value.ToString() != settingsService.CurrentUser.UserId;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConvertBack(value, targetType, parameter, culture);
        }
    }
}
