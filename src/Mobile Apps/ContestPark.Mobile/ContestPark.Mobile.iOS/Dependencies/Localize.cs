using ContestPark.Mobile.Dependencies;
using ContestPark.Mobile.iOS.Dependencies;
using Foundation;
using System.Globalization;
using System.Threading;
using Xamarin.Forms;

[assembly: Dependency(typeof(Localize))]

namespace ContestPark.Mobile.iOS.Dependencies
{
    public class Localize : ILocalize
    {
        public CultureInfo GetCurrentCultureInfo()
        {
            var netLanguage = "en";
            var prefLang = "en";

            if (NSLocale.PreferredLanguages.Length > 0)
            {
                var pref = NSLocale.PreferredLanguages[0];
                prefLang = pref.Substring(0, 2);
                netLanguage = pref.Replace("_", "-");
            }

            CultureInfo ci = null;
            try
            {
                ci = new CultureInfo(netLanguage);
            }
            catch
            {
                ci = new CultureInfo(prefLang);
            }

            return ci;
        }

        public void SetCultureInfo(CultureInfo cultureInfo)
        {
        }

        public void SetDefaultLocale()
        {
        }

        public void SetLocale()
        {
            var iosLocaleAuto = NSLocale.AutoUpdatingCurrentLocale.LocaleIdentifier;
            var netLocale = iosLocaleAuto.Replace("_", "-");
            CultureInfo ci;
            try
            {
                ci = new CultureInfo(netLocale);
            }
            catch
            {
                ci = GetCurrentCultureInfo();
            }

            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }
    }
}
