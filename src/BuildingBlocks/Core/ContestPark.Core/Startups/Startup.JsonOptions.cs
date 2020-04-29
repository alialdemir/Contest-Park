using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class JsonOptionsStartup
    {
        public static IMvcBuilder AddJsonOptions(this IMvcBuilder mvcBuilder)
        {
            mvcBuilder.AddNewtonsoftJson(options =>
             {
                 options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                 options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                 options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                 options.SerializerSettings.Converters.Add(new StringEnumConverter());
             });

            return mvcBuilder;
        }
    }
}