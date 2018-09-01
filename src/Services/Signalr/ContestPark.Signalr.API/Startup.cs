using ContestPark.Core.OrleansClient;
using ContestPark.Core.Service.Startup;
using ContestPark.Signalr.API.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ContestPark.Signalr.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSignalR()
                 .AddJsonProtocol(options =>
                 {
                     JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
                     {
                         //ContractResolver = new CamelCasePropertyNamesContractResolver(),
                         //DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                         NullValueHandling = NullValueHandling.Ignore
                     };
                     jsonSerializerSettings.Converters.Add(new StringEnumConverter());

                     options.PayloadSerializerSettings = jsonSerializerSettings;
                 })
                .AddOrleans();

            services.UseOrleansClient(Configuration);

            services.AddAuth(Configuration)
                .AddOptions()
                .AddCorsConfigure();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.LoggerConfigure(env, loggerFactory, Configuration);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler();
            }

            app.AddCors()
                .AddAuth()
                .UseSignalR(routes =>
                {
                    routes.MapHub<ContestParkHub>("/contestpark", options =>
                        options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransports.All);
                })
                .UseMvc();
        }
    }
}