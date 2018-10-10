using System.Globalization;

namespace ContestPark.Mobile.Dependencies
{
    public interface ILocalize
    {
        void SetDefaultLocale();

        void SetCultureInfo(CultureInfo cultureInfo);

        CultureInfo GetCurrentCultureInfo();
    }
}