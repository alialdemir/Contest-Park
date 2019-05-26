using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DataAnnotationsLocalizationStartup
    {
        public static IMvcBuilder AddDataAnnotationsLocalization(this IMvcBuilder mvcBuilder, string resourceName)
        {
            string serviceAssemblyName = Assembly.GetEntryAssembly().GetName().Name;

            mvcBuilder.AddDataAnnotationsLocalization((options) =>// Bunu ekleyince direk dataanotions localize oluyor
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                    {
                        return factory.Create(resourceName, serviceAssemblyName);
                    };
                });

            return mvcBuilder;
        }
    }
}