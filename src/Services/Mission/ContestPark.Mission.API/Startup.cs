using Autofac;
using Autofac.Extensions.DependencyInjection;
using ContestPark.Core.Startups;
using ContestPark.EventBus.Abstractions;
using ContestPark.Mission.API.Infrastructure.Repositories.CompletedMission;
using ContestPark.Mission.API.Infrastructure.Repositories.Mission;
using ContestPark.Mission.API.Resources;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace ContestPark.Mission.API
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
            services.Configure<MissionSettings>(Configuration);

            services.AddAuth(Configuration)
                    .AddMySql()
                    .AddMvc(options => options.EnableEndpointRouting = false)
                    .AddJsonOptions()
                    .AddDataAnnotationsLocalization(typeof(MissionResource).Name)
                    .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddLocalizationCustom();

            services
                    .AddRabbitMq(Configuration)
                    .AddCorsConfigure();

            services.AddTransient<IMissionRepository, MissionRepository>();
            services.AddTransient<ICompletedMissionRepository, CompletedMissionRepository>();

            // services.AddTransient<OpenSubCategoryAndFollowIntegrationEventHandler>();

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

            app.UseSwaggerGen("Mission V1");

            ConfigureEventBus(app);
        }

        protected virtual void ConfigureAuth(IApplicationBuilder app)
        {
            app.UseAuth();
        }

        protected virtual void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            //eventBus.Subscribe<NewUserRegisterIntegrationEvent, NewUserRegisterIntegrationEventHandler>();
        }
    }
}
