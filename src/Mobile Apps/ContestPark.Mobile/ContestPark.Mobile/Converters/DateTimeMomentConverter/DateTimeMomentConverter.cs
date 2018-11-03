using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Services.Settings;
using Prism.Ioc;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace ContestPark.Mobile.Converters
{
    public class DateTimeMomentConverter : IValueConverter
    {
        /// <summary>
        /// Default dil ingilizce olarak ayarlandı
        /// </summary>
        /// <param name="dateTtime"></param>
        /// <returns></returns>
        public static string GetPrettyDate(DateTime dateTtime)
        {
            // TODO: Moment js kullanılması lazım

            TimeSpan s = DateTime.Now.Subtract(dateTtime);
            int dayDiff = (int)s.TotalDays;
            int secDiff = (int)s.TotalSeconds;

            ISettingsService settingsService = RegisterTypesConfig.Container.Resolve<ISettingsService>();

            Languages language = settingsService.CurrentUser.Language;

            if (dayDiff == 0)
            {
                if (secDiff < 60)
                {
                    if (language.HasFlag(Languages.Turkish)) return "şuanda";
                    else return "just now";
                }
                if (secDiff < 120)
                {
                    if (language.HasFlag(Languages.Turkish)) return "1 dakika önce";
                    else return "a minute ago";
                }
                if (secDiff < 3600)
                {
                    double minutes = Math.Floor((double)secDiff / 60);
                    if (language.HasFlag(Languages.Turkish)) return string.Format("{0} dakika önce", minutes);
                    else return string.Format("{0} minutes ago", minutes);
                }
                if (secDiff < 7200)
                {
                    if (language.HasFlag(Languages.Turkish)) return "1 saat önce";
                    else return "a hour ago";
                }

                if (secDiff < 86400)
                {
                    double hours = Math.Floor((double)secDiff / 3600);
                    if (language.HasFlag(Languages.Turkish)) return string.Format("{0} saat önce", hours);
                    else return string.Format("{0} hours ago", hours);
                }
            }
            if (dayDiff == 1)
            {
                if (language.HasFlag(Languages.Turkish)) return "dün";
                else return "yesterday";
            }
            if (dayDiff < 7)
            {
                if (language.HasFlag(Languages.Turkish)) return string.Format("{0} gün önce", dayDiff);
                else return string.Format("{0} days ago", dayDiff);
            }
            if (dayDiff < 31)
            {
                double weeks = Math.Ceiling((double)dayDiff / 7);
                if (language.HasFlag(Languages.Turkish)) return string.Format("{0} hafta önce", weeks);
                else return string.Format("{0} weeks ago", weeks);
            }
            if (dayDiff == 30)
            {
                if (language.HasFlag(Languages.Turkish)) return "1 ay önce";
                else return "a month ago";
            }
            if (dayDiff < 360)
            {
                double months = Math.Floor((double)dayDiff / 30);
                if (language.HasFlag(Languages.Turkish)) return string.Format("{0} ay önce", months);
                else return string.Format("{0} months ago", months);
            }
            if (dayDiff == 360)
            {
                if (language.HasFlag(Languages.Turkish)) return "1 yıl önce";
                else return "a year ago";
            }
            if (dayDiff >= 360)
            {
                double years = Math.Floor((double)dayDiff / 360);
                if (language.HasFlag(Languages.Turkish)) return string.Format("{0} yıl önce", years);
                else return string.Format("{0} years ago", years);
            }
            return string.Empty;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return GetPrettyDate((DateTime)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return GetPrettyDate((DateTime)value);
        }
    }
}