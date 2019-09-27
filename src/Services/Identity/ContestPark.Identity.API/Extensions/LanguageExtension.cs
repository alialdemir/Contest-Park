using ContestPark.Core.Enums;

namespace ContestPark.Identity.API.Extensions
{
    public static class LanguageExtension
    {
        public static string ToLanguageCode(this Languages language)
        {
            return Languages.Turkish == language ? "tr-TR" : "en-US";
        }

        public static string ToLanguageCode(this string langCode)
        {
            return ToLanguagesEnum(langCode).ToLanguageCode();
        }

        public static Languages ToLanguagesEnum(this string langCode)
        {
            return langCode.ToLower() == "tr-tr" ? Languages.Turkish : Languages.English;
        }
    }
}
