using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using ContestPark.Category.API.Infrastructure.ElasticSearch;
using ContestPark.Category.API.Infrastructure.Repositories.FollowSubCategory;
using ContestPark.Category.API.Infrastructure.Repositories.OpenSubCategory;
using ContestPark.Category.API.Infrastructure.Repositories.Search;
using ContestPark.Category.API.Infrastructure.Repositories.SubCategory;
using ContestPark.Category.API.Infrastructure.Repositories.UserLevel;
using ContestPark.Category.API.IntegrationEvents.EventHandling;
using ContestPark.Category.API.IntegrationEvents.Events;
using ContestPark.Category.API.Resources;
using ContestPark.Category.API.Services.Balance;
using ContestPark.Core.Services.RequestProvider;
using ContestPark.EventBus.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nest;
using System;

namespace ContestPark.Category.API
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
            services.Configure<CategorySettings>(Configuration);

            services.AddSingleton<IRequestProvider, RequestProvider>();

            services.AddAuth(Configuration)
                    .AddMySql()
                    .AddMvc(options => options.EnableEndpointRouting = false)
                    .AddJsonOptions()
                    .AddDataAnnotationsLocalization(typeof(CategoryResource).Name)
                    .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddLocalizationCustom();

            services
                    .AddRabbitMq(Configuration)
                    .AddCorsConfigure();

            services.AddTransient<ISubCategoryRepository, SubCategoryRepository>();
            services.AddTransient<IOpenCategoryRepository, OpenCategoryRepository>();
            services.AddTransient<IFollowSubCategoryRepository, FollowSubCategoryRepository>();
            services.AddTransient<IUserLevelRepository, UserLevelRepository>();

            AddTransient(services);

            #region ElasticSearch

            services.AddSingleton((_) =>
            {
                var connectionSettings = new ConnectionSettings(new Uri(Configuration["ElasticSearchURI"]));
                connectionSettings.DefaultIndex("defaultindex");

                return connectionSettings;
            });

            services.AddTransient<IElasticContext, ElasticContext>();
            services.AddTransient<ISearchRepository, SearchRepository>();

            #endregion ElasticSearch

            services.AddTransient<NewUserRegisterIntegrationEventHandler>();
            services.AddTransient<UserInfoChangedIntegrationEventHandler>();
            services.AddTransient<ProfilePictureChangedIntegrationEventHandler>();
            services.AddTransient<NewSubCategoryAddedIntegrationEventHandler>();
            services.AddTransient<OpenSubCategoryAndFollowIntegrationEventHandler>();
            services.AddTransient<UpdateLevelIntegrationEventHandler>();

            services.AddAutoMapper(typeof(AutoMapperProfile));

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

        protected virtual void ConfigureAuth(IApplicationBuilder app)
        {
            app.UseAuth();
        }

        protected virtual void AddTransient(IServiceCollection services)
        {
            services.AddSingleton<IBalanceService, BalanceService>();
        }

        protected virtual void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            eventBus.Subscribe<NewUserRegisterIntegrationEvent, NewUserRegisterIntegrationEventHandler>();
            eventBus.Subscribe<UserInfoChangedIntegrationEvent, UserInfoChangedIntegrationEventHandler>();
            eventBus.Subscribe<ProfilePictureChangedIntegrationEvent, ProfilePictureChangedIntegrationEventHandler>();
            eventBus.Subscribe<NewSubCategoryAddedIntegrationEvent, NewSubCategoryAddedIntegrationEventHandler>();
            eventBus.Subscribe<OpenSubCategoryAndFollowIntegrationEvent, OpenSubCategoryAndFollowIntegrationEventHandler>();
            eventBus.Subscribe<UpdateLevelIntegrationEvent, UpdateLevelIntegrationEventHandler>();
        }
    }
}
