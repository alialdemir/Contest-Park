using Amazon.S3;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using ContestPark.Core.Middlewares;
using ContestPark.Core.Services.RequestProvider;
using ContestPark.EventBus.Abstractions;
using ContestPark.EventBus.IntegrationEventLogEF.Services;
using ContestPark.Identity.API.Data;
using ContestPark.Identity.API.Data.Repositories.DeviceInfo;
using ContestPark.Identity.API.Data.Repositories.Picture;
using ContestPark.Identity.API.Data.Repositories.Reference;
using ContestPark.Identity.API.Data.Repositories.ReferenceCode;
using ContestPark.Identity.API.Data.Repositories.User;
using ContestPark.Identity.API.Data.Tables;
using ContestPark.Identity.API.IntegrationEvents;
using ContestPark.Identity.API.IntegrationEvents.EventHandling;
using ContestPark.Identity.API.IntegrationEvents.Events;
using ContestPark.Identity.API.Resources;
using ContestPark.Identity.API.Services.BlobStorage;
using ContestPark.Identity.API.Services.Block;
using ContestPark.Identity.API.Services.Follow;
using ContestPark.Identity.API.Services.Login;
using ContestPark.Identity.API.Services.NumberFormat;
using ContestPark.Identity.API.Services.Picture;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Data.Common;
using System.Reflection;

namespace ContestPark.Identity.API
{
    public partial class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

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
                .AddCors()
                .UseMiddleware<ServiceAuthorizeMiddleware>()
                .UseCustomIdentityServer()
                .UseRequestLocalizationCustom()
                .UseMvc();

            ConfigureEventBus(app);
        }

        protected virtual void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            eventBus.Subscribe<ChangedGameCountIntegrationEvent, ChangedGameCountIntegrationEventHandler>();
            eventBus.Subscribe<DeleteFileIntegrationEvent, DeleteFileIntegrationEventHandler>();
            eventBus.Subscribe<FollowIntegrationEvent, FollowIntegrationEventHandler>();
            eventBus.Subscribe<UnFollowIntegrationEvent, UnFollowIntegrationEventHandler>();
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            string connectionString = Configuration["ConnectionString"];

            services.Configure<IdentitySettings>(Configuration);

            #region Ef Configuration

            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
             options.UseMySql(connectionString,
                                     mySqlOptionsAction: sqlOptions =>
                                     {
                                         sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                                         //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency

                                         //sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                                     }));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            #endregion Ef Configuration

            #region AddTransient

            services.AddTransient<ILoginService<ApplicationUser>, EFLoginService>();

            services.AddTransient<IReferenceRepository, ReferenceRepository>();

            services.AddTransient<IReferenceCodeRepostory, ReferenceCodeRepostory>();

            services.AddTransient<IUserRepository, UserRepository>();

            services.AddTransient<IPictureRepository, PictureRepository>();

            services.AddSingleton<IFileUploadService, S3FileUploadService>();

            services.AddTransient<IDeviceInfoRepository, DeviceInfoRepository>();

            services.AddSingleton<IRequestProvider, RequestProvider>();

            services.AddTransient<IBlockService, BlockService>();

            services.AddTransient<IFollowService, FollowService>();

            services.AddSingleton<INumberFormatService, NumberFormatService>();

            #endregion AddTransient

            #region S3 settings

            string awsAccessKeyId = Configuration["AwsAccessKeyId"];
            string awsSecretAccessKey = Configuration["AwsSecretAccessKey"];

            services.AddSingleton<IAmazonS3>(new AmazonS3Client(awsAccessKeyId, awsSecretAccessKey, Amazon.RegionEndpoint.EUCentral1));

            #endregion S3 settings

            services.AddLocalizationCustom();

            services.AddCustomIdentityServer(Configuration, connectionString)
                    .AddMvc(options => options.EnableEndpointRouting = false)
                    .AddJsonOptions()
                    .AddDataAnnotationsLocalization(typeof(IdentityResource).Name)
                    .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services
                .AddIntegrationEventLogEFDbContext(connectionString)
                .AddRabbitMq(Configuration)
                .AddCorsConfigure();

            services.AddTransient<Func<DbConnection, IIntegrationEventLogService>>(
            sp => (DbConnection c) => new IntegrationEventLogService(c));

            services.AddTransient<IIdentityIntegrationEventService, IdentityIntegrationEventService>();

            services.AddTransient<ChangedGameCountIntegrationEventHandler>();
            services.AddTransient<DeleteFileIntegrationEventHandler>();
            services.AddTransient<FollowIntegrationEventHandler>();
            services.AddTransient<UnFollowIntegrationEventHandler>();

            var container = new ContainerBuilder();
            container.Populate(services);
            return new AutofacServiceProvider(container.Build());
        }
    }
}
