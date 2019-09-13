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
using System.Reflection;

namespace ContestPark.Signalr.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddAuth(Configuration);

            //string signalrStoreConnectionString = Configuration.GetValue<string>("Redis");
            //if (!string.IsNullOrEmpty(signalrStoreConnectionString))
            //{
            //    services.AddSignalR()
            //        .AddRedis(signalrStoreConnectionString);
            //}
            //else
            //{
            services.AddSignalR();
            //}

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

            #region Event handler

            services.AddTransient<DuelCreatedIntegrationEventHandler>();

            services.AddTransient<DuelStartingModelIntegrationEventHandler>();

            services.AddTransient<NextQuestionIntegrationEventHandler>();

            services.AddTransient<SendErrorMessageWithSignalrIntegrationEventHandler>();

            services.AddTransient<SendMessageWithSignalrIntegrationEventHandler>();

            #endregion Event handler

            var container = new ContainerBuilder();
            container.RegisterModule(new ApplicationModule());
            container.Populate(services);
            return new AutofacServiceProvider(container.Build());
        }

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

            eventBus.Subscribe<DuelCreatedIntegrationEvent, DuelCreatedIntegrationEventHandler>();

            eventBus.Subscribe<DuelStartingModelIntegrationEvent, DuelStartingModelIntegrationEventHandler>();

            eventBus.Subscribe<NextQuestionIntegrationEvent, NextQuestionIntegrationEventHandler>();

            eventBus.Subscribe<SendErrorMessageWithSignalrIntegrationEvent, SendErrorMessageWithSignalrIntegrationEventHandler>();

            eventBus.Subscribe<SendMessageWithSignalrIntegrationEvent, SendMessageWithSignalrIntegrationEventHandler>();
        }
    }

    public class ApplicationModule
      : Autofac.Module
    {
        public string QueriesConnectionString { get; }

        public ApplicationModule()
        {
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(SendErrorMessageWithSignalrIntegrationEvent).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IIntegrationEventHandler<>));
        }
    }
}
