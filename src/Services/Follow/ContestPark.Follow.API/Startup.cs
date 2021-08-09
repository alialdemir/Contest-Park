using Autofac;
using Autofac.Extensions.DependencyInjection;
using ContestPark.Core.Services.Identity;
using ContestPark.Core.Services.RequestProvider;
using ContestPark.Core.Startups;
using ContestPark.EventBus.Abstractions;
using ContestPark.Follow.API.Infrastructure.MySql.Repositories;
using ContestPark.Follow.API.Resources;
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

namespace ContestPark.Follow.API
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
            services.Configure<FollowSettings>(Configuration);

            services.AddMemoryCache();

            services.AddAuth(Configuration)
                    .AddMySql()
                    .AddMvc(options=> options.EnableEndpointRouting=false)
                    .AddJsonOptions()
                    .AddDataAnnotationsLocalization(typeof(FollowResource).Name)
                    .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddLocalizationCustom();

            services.AddSingleton<IRedisClient>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<FollowSettings>>().Value;

                return new RedisClient(settings.Redis);
            });

            ConfigureOtherService(services);

            services
                    .AddRabbitMq(Configuration)
                    .AddCorsConfigure();

            services.AddTransient<IFollowRepository, FollowRepository>();

            //services.AddTransient<NewUserRegisterIntegrationEventHandler>();

            services.AddSwagger();

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

            app.UseSwaggerGen("Follow v1");

            ConfigureEventBus(app);
        }

        protected virtual void ConfigureAuth(IApplicationBuilder app)
        {
            app.UseAuth();
        }

        protected virtual void ConfigureOtherService(IServiceCollection services)
        {
            services.AddSingleton<IRequestProvider, RequestProvider>();
            services.AddSingleton<IIdentityService, IdentityService>();
        }

        protected virtual void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            // eventBus.Subscribe<NewUserRegisterIntegrationEvent, NewUserRegisterIntegrationEventHandler>();
        }
    }
}
