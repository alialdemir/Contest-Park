using ContestPark.Mobile.Dependencies;
using System.Globalization;
using System.Threading;

[assembly: Xamarin.Forms.Dependency(typeof(ContestPark.Mobile.Droid.Dependencies.Localize))]

namespace ContestPark.Mobile.Droid.Dependencies
{
    public class Localize : ILocalize
    {
        public CultureInfo GetCurrentCultureInfo()
        {
            return CultureInfo();
        }

        public void SetDefaultLocale()
        {
            var cultureInfo = CultureInfo();
            SetCultureInfo(cultureInfo);
        }

        public void SetCultureInfo(CultureInfo cultureInfo)
        {
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
        }

        private CultureInfo CultureInfo()
        {
            var androidLocale = Java.Util.Locale.Default;
            var netLocale = androidLocale.ToString().Replace("_", "-");
            return new CultureInfo(netLocale);
        }
    }
}