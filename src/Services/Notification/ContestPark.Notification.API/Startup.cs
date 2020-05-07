using Autofac;
using Autofac.Extensions.DependencyInjection;
using ContestPark.Core.Services.Identity;
using ContestPark.Core.Services.RequestProvider;
using ContestPark.EventBus.Abstractions;
using ContestPark.Notification.API.Infrastructure.Repositories.Notice;
using ContestPark.Notification.API.Infrastructure.Repositories.Notification;
using ContestPark.Notification.API.IntegrationEvents.EventHandling;
using ContestPark.Notification.API.IntegrationEvents.Events;
using ContestPark.Notification.API.Resources;
using ContestPark.Notification.API.Services.PushNotification;
using ContestPark.Notification.API.Services.Sms;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServiceStack.Redis;
using System;

namespace ContestPark.Notification.API
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
            services.Configure<NotificationSettings>(Configuration);

            services.AddSingleton<IRequestProvider, RequestProvider>();

            services.AddAuth(Configuration)
                    .AddMySql()
                    .AddMvc(options => options.EnableEndpointRouting = false)
                    .AddJsonOptions()
                    .AddDataAnnotationsLocalization(typeof(NotificationResource).Name)
                    .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddSingleton<IRedisClient>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<NotificationSettings>>().Value;

                return new RedisClient(settings.Redis);
            });

            services.AddLocalizationCustom();

            services
                    .AddRabbitMq(Configuration)
                    .AddCorsConfigure();

            services.AddTransient<INotificationRepository, NotificationRepository>();
            services.AddTransient<INoticeRepository, NoticeRepository>();

            services.AddTransient<ISmsService, AwsSmsService>();

            services.AddTransient<IPushNotification, PushNotification>();

            services.AddTransient<AddNotificationIntegrationEventHandler>();
            services.AddTransient<SendPushNotificationIntegrationEventHandler>();

            ConfigureIdentityService(services);

            var container = new ContainerBuilder();
            container.Populate(services);
            return new AutofacServiceProvider(container.Build());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var pathBase = Configuration["PATH_BASE"];
            if (!string.IsNullOrEmpty(pathBase))
            {
                loggerFactory.CreateLogger("init").LogDebug($"Using PATH BASE '{pathBase}'");
                app.UsePathBase(pathBase);
            }

            app.UseExceptionHandlerConfigure()
               .AddCors();

            ConfigureAuth(app);

            app.UseRequestLocalizationCustom()
               .UseMvc();

            ConfigureEventBus(app);
        }

        protected virtual void ConfigureIdentityService(IServiceCollection services)
        {
            services.AddSingleton<IRequestProvider, RequestProvider>();
            services.AddSingleton<IIdentityService, IdentityService>();
        }

        protected virtual void ConfigureAuth(IApplicationBuilder app)
        {
            app.UseAuth();
        }

        protected virtual void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            eventBus.Subscribe<AddNotificationIntegrationEvent, AddNotificationIntegrationEventHandler>();
            eventBus.Subscribe<SendPushNotificationIntegrationEvent, SendPushNotificationIntegrationEventHandler>();
        }
    }
}
