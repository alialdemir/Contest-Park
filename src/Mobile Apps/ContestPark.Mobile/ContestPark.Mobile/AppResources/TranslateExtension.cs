using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Extensions;
using ContestPark.Mobile.Services.Settings;
using Prism.Ioc;
using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.AppResources
{
    [ContentProperty("Text")]
    public class TranslateExtension : IMarkupExtension
    {
        public static readonly Lazy<ResourceManager> resmgr = new Lazy<ResourceManager>(() => new ResourceManager(ResourceId, typeof(TranslateExtension).GetTypeInfo().Assembly));
        private const string ResourceId = "ContestPark.Mobile.AppResources.ContestParkResources";
        private static CultureInfo _cultureInfo;

        public static CultureInfo CultureInfo
        {
            get
            {
                if (_cultureInfo == null)
                {
                    ISettingsService settingsService = RegisterTypesConfig.Container.Resolve<ISettingsService>();
                    if (settingsService != null && settingsService.CurrentUser != null)
                    {
                        _cultureInfo = new CultureInfo(settingsService.CurrentUser.Language.ToLanguageCode());
                    }
                }

                return _cultureInfo;
            }

            set
            {
                _cultureInfo = value;
            }
        }

        public string Text { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Text == null)
                return "";

            var translation = resmgr.Value.GetString(Text, CultureInfo);

            if (translation == null)
            {
#if DEBUG
                throw new ArgumentException(
                    String.Format("Key '{0}' was not found in resources '{1}' for culture '{2}'.", Text, ResourceId, CultureInfo.Name),
                    "Text");
#else
                translation = Text; // returns the key, which GETS DISPLAYED TO THE USER
#endif
            }
            return translation;
        }
    }
}
