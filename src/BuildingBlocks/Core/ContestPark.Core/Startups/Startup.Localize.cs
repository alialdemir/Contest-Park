using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class LocalizeStartup
    {
        public static IServiceCollection AddLocalizationCustom(this IServiceCollection services)
        {
            // Kayıt işlemimizi gerçekleştiriyoruz, AddMvc() den önce eklediğinizden emin olunuz.
            services
                .AddLocalization(options =>
                {
                    // Resource (kaynak) dosyalarımızı ana dizin altında "Resources" klasorü içerisinde tutacağımızı belirtiyoruz.
                    options.ResourcesPath = "Resources";
                });

            return services;
        }

        public static IApplicationBuilder UseRequestLocalizationCustom(this IApplicationBuilder app)
        {
            // Bu bölüm UseMvc()' den önce eklenecektir.
            // Uygulamamız içerisinde destek vermemizi istediğimiz dilleri tutan bir liste oluşturuyoruz.
            var supportedCultures = new List<CultureInfo>
                     {
                           new CultureInfo("tr-TR"),
                           new CultureInfo("en-US"),
                     };
            // SupportedCultures ve SupportedUICultures'a yukarıda oluşturduğumuz dil listesini tanımlıyoruz.
            // DefaultRequestCulture'a varsayılan olarak uygulamamızın hangi dil ile çalışması gerektiğini tanımlıyoruz.
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures,
                DefaultRequestCulture = new RequestCulture("en-US")// Varsayılan dil ayarı
            });

            return app;
        }
    }
}