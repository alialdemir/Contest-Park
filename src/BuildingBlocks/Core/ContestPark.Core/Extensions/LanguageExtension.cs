using ContestPark.Core.Enums;

namespace ContestPark.Core.Extensions
{
    public static class LanguageExtension
    {
        /// <summary>
        /// Language enumunu ükle koduna çevirir
        /// </summary>
        /// <param name="language">Dil enumu</param>
        /// <returns>Ülke kodu</returns>
        public static string ToLanguageCode(this Languages language)
        {
            return Languages.Turkish == language ? "tr-TR" : "en-US";
        }

        ///// <summary>
        ///// üÜlke koduna göre
        ///// </summary>
        ///// <param name="langCode"></param>
        ///// <returns></returns>
        //public static string ToLanguageCode(this string langCode)
        //{
        //    return ToLanguagesEnum(langCode).ToLanguageCode();
        //}

        /// <summary>
        /// Ülke koduna göre dil enumu döndürür
        /// </summary>
        /// <param name="langCode">Ülke kodu</param>
        /// <returns>Dil kodu</returns>
        public static Languages ToLanguagesEnum(this string langCode)
        {
            return langCode == "tr-TR" ? Languages.Turkish : Languages.English;
        }
    }
}