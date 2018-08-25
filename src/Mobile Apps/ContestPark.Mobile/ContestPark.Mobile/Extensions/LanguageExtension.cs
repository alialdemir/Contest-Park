using ContestPark.Mobile.Enums;

namespace ContestPark.Mobile.Extensions
{
    public static class LanguageExtension
    {
        public static string ToLanguageCode(this Languages language)
        {
            return Languages.Turkish.HasFlag(language) ? "tr-TR" : "en-US";
        }

        public static string ToLanguageCode(this string langCode)
        {
            return ToLanguagesEnum(langCode).ToLanguageCode();
        }

        public static Languages ToLanguagesEnum(this string langCode)
        {
            return langCode == "tr-TR" ? Languages.Turkish : Languages.English;
        }
    }
}