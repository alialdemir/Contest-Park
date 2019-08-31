using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Models.Ranking;
using ContestPark.Mobile.Services.Settings;
using Prism.Ioc;
using System;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;

namespace ContestPark.Mobile.Converters
{
    public class RankBackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return Color.White;

            int rank = ((ListView)parameter).ItemsSource.Cast<object>().ToList().IndexOf(value);

            ISettingsService settingsService = RegisterTypesConfig.Container.Resolve<ISettingsService>();

            return settingsService.CurrentUser.UserName == ((RankingModel)value).UserName ?
                (Color)Color.FromHex("#F46F48") : rank % 2 == 0 ?
                Color.FromHex("#F9F9F9") :
                (Color)Application.Current.Resources["White"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }
    }
}
