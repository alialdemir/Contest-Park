using ContestPark.Core.Enums;
using ContestPark.Core.Extensions;
using Xunit;

namespace ContestPark.Core.Tests.Extensions
{
    public class LanguageExtensionTest
    {
        /// <summary>
        /// Eğer türkçe language enumunu lang code çevirirsek tr-TR dönmeli
        /// </summary>
        [Fact]
        public void Should_Language_Turkish_When_LangCode_trTR()
        {
            Languages language = Languages.Turkish;
            string langCode = language.ToLanguageCode();

            Assert.Equal("tr-TR", langCode);
        }

        /// <summary>
        /// Eğer ingilizce language enumunu lang code çevirirsek en-US dönmeli
        /// </summary>
        [Fact]
        public void Should_Language_English_When_LangCode_enUS()
        {
            Languages language = Languages.English;
            string langCode = language.ToLanguageCode();

            Assert.Equal("en-US", langCode);
        }

        /// <summary>
        /// String en-US enuma çevirince ingilizce gelmeli
        /// </summary>
        [Fact]
        public void Should_LangCode_enUS_When_Language_English_Enum()
        {
            Languages language = "en-US".ToLanguagesEnum();

            Assert.Equal(Languages.English, language);
        }

        /// <summary>
        /// String tr-TR enuma çevirince ingilizce gelmeli
        /// </summary>
        [Fact]
        public void Should_LangCode_trTR_When_Language_Turkish_Enum()
        {
            Languages language = "tr-TR".ToLanguagesEnum();

            Assert.Equal(Languages.Turkish, language);
        }

        /// <summary>
        /// Saçma bir yazıyı dil enuma çevirince ingilizce default gelmeli
        /// </summary>
        [Fact]
        public void Should_LangCode_frFR_When_Language_English_Enum()
        {
            Languages language = "fr-FR".ToLanguagesEnum();

            Assert.Equal(Languages.English, language);
        }
    }
}