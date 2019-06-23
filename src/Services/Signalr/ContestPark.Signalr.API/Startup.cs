using Autofac;
using Autofac.Extensions.DependencyInjection;
using ContestPark.EventBus.Abstractions;
using ContestPark.Signalr.API.IntegrationEvents.EventHandling;
using ContestPark.Signalr.API.IntegrationEvents.Events;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace ContestPark.Signalr.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddAuth(Configuration)
                     .AddApplicationInsightsTelemetry(Configuration);

            string signalrStoreConnectionString = Configuration.GetValue<string>("SignalrStoreConnectionString");
            if (!string.IsNullOrEmpty(signalrStoreConnectionString))
            {
                services.AddSignalR()
                        .AddRedis(signalrStoreConnectionString);
            }
            else
            {
                services.AddSignalR();
            }

            services.AddRabbitMq(Configuration)
                    .AddCors(options =>
                    {
                        options.AddPolicy("CorsPolicy",
                            builder => builder
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .SetIsOriginAllowed((host) => true)
                            .AllowCredentials());
                    });

            services.AddAuth(Configuration);

            services.AddTransient<SendErrorMessageWithSignalrIntegrationEventHandler>();

            var container = new ContainerBuilder();
            container.Populate(services);
            return new AutofacServiceProvider(container.Build());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var pathBase = Configuration["PATH_BASE"];
            if (!string.IsNullOrEmpty(pathBase))
            {
                loggerFactory.CreateLogger<Startup>().LogDebug("Using PATH BASE '{pathBase}'", pathBase);
                app.UsePathBase(pathBase);
            }

            app.UseExceptionHandlerConfigure()
               .AddCors();

            ConfigureAuth(app);

            app.UseSignalR(routes =>
            {
                routes.MapHub<ContestParkHub>("/contestparkhub", options =>
                    options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransports.All);
            });

            ConfigureEventBus(app);
        }

        protected virtual void ConfigureAuth(IApplicationBuilder app)
        {
            app.UseAuth();
        }

        protected virtual void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            eventBus.Subscribe<SendErrorMessageWithSignalrIntegrationEvent, SendErrorMessageWithSignalrIntegrationEventHandler>();
        }
    }
}