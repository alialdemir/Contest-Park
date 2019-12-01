using Amazon.S3;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using ContestPark.Admin.API.Infrastructure.Repositories.AnswerLocalized;
using ContestPark.Admin.API.Infrastructure.Repositories.Category;
using ContestPark.Admin.API.Infrastructure.Repositories.QuestionLocalized;
using ContestPark.Admin.API.Infrastructure.Repositories.QuestionOfQuestionLocalized;
using ContestPark.Admin.API.Infrastructure.Repositories.SubCategory;
using ContestPark.Admin.API.IntegrationEvents.EventHandling;
using ContestPark.Admin.API.IntegrationEvents.Events;
using ContestPark.Admin.API.Providers;
using ContestPark.Admin.API.Resources;
using ContestPark.Admin.API.Services.Ffmpeg;
using ContestPark.Admin.API.Services.Picture;
using ContestPark.Admin.API.Services.Spotify;
using ContestPark.Core.Middlewares;
using ContestPark.Core.Services.NumberFormat;
using ContestPark.EventBus.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace ContestPark.Admin.API
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
            services.AddAuthorization(options =>// Soru ekleme için admin policy oluşturuldu
            {
                options.AddPolicy("AdminPolicy", policy =>
                    policy.RequireClaim("role", "Admin, User"));
            });

            services.Configure<AdminSettings>(Configuration);

            services.AddAuth(Configuration)
                    .AddMySql()
                    .AddMvc(options =>
                    {
                        // add custom model binders to beginning of collection
                        options.ModelBinderProviders.Insert(0, new FormDataJsonBinderProvider());
                    })
                    .AddJsonOptions()
                    .AddDataAnnotationsLocalization(typeof(AdminResource).Name)
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddLocalizationCustom();

            services
                    .AddRabbitMq(Configuration)
                    .AddCorsConfigure();

            services.AddTransient<ISpotifyService, SpotifyService>();
            services.AddTransient<IFfmpegService, FfmpegService>();

            #region s3

            string awsAccessKeyId = Configuration["AwsAccessKeyId"];
            string awsSecretAccessKey = Configuration["AwsSecretAccessKey"];
            services.AddSingleton<IAmazonS3>(new AmazonS3Client(awsAccessKeyId, awsSecretAccessKey, Amazon.RegionEndpoint.EUCentral1));

            #endregion s3

            #region Repositories

            services.AddTransient<INumberFormatService, NumberFormatService>();

            services.AddTransient<ISubCategoryRepository, SubCategoryRepository>();

            services.AddTransient<ICategoryRepository, CategoryRepository>();

            services.AddSingleton<IFileUploadService, S3FileUploadService>();

            services.AddTransient<IQuestionLocalizedRepository, QuestionLocalizedRepository>();

            services.AddTransient<IQuestionOfQuestionLocalizedRepository, QuestionOfQuestionLocalizedRepository>();

            services.AddTransient<IAnswerLocalizedRepository, AnswerLocalizedRepository>();

            #endregion Repositories

            #region Event handler

            services.AddTransient<CreateQuestionIntegrationEventHandler>();

            services.AddTransient<CreateSpotifyQuestionIntegrationEventHandler>();

            #endregion Event handler

            var container = new ContainerBuilder();
            container.Populate(services);
            return new AutofacServiceProvider(container.Build());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // app.UseStaticFiles();

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

#if !DEBUG
                ConfigureEventBus(app);
#endif
        }

        protected virtual void ConfigureAuth(IApplicationBuilder app)
        {
            app
                .UseMiddleware<ServiceAuthorizeMiddleware>()
                .UseAuth();
        }

        protected virtual void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            eventBus.Subscribe<CreateQuestionIntegrationEvent, CreateQuestionIntegrationEventHandler>();
            eventBus.Subscribe<CreateSpotifyQuestionIntegrationEvent, CreateSpotifyQuestionIntegrationEventHandler>();
        }
    }
}
