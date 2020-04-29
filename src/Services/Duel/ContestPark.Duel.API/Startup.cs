using Amazon.S3;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using ContestPark.Core.Services.Identity;
using ContestPark.Core.Services.NumberFormat;
using ContestPark.Core.Services.RequestProvider;
using ContestPark.Duel.API.Infrastructure.Repositories.AskedQuestion;
using ContestPark.Duel.API.Infrastructure.Repositories.ContestDate;
using ContestPark.Duel.API.Infrastructure.Repositories.Duel;
using ContestPark.Duel.API.Infrastructure.Repositories.DuelDetail;
using ContestPark.Duel.API.Infrastructure.Repositories.Question;
using ContestPark.Duel.API.Infrastructure.Repositories.Redis.DuelUser;
using ContestPark.Duel.API.Infrastructure.Repositories.Redis.UserAnswer;
using ContestPark.Duel.API.Infrastructure.Repositories.ScoreRankingRepository;
using ContestPark.Duel.API.IntegrationEvents.EventHandling;
using ContestPark.Duel.API.IntegrationEvents.Events;
using ContestPark.Duel.API.Resources;
using ContestPark.Duel.API.Services.Balance;
using ContestPark.Duel.API.Services.Follow;
using ContestPark.Duel.API.Services.Picture;
using ContestPark.Duel.API.Services.ScoreCalculator;
using ContestPark.Duel.API.Services.SubCategory;
using ContestPark.EventBus.Abstractions;
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

namespace ContestPark.Duel.API
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
            services.AddAuthorization(options =>// Soru ekleme için admin policy oluşturuldu
            {
                options.AddPolicy("AdminPolicy", policy =>
                    policy.RequireClaim("role", "Admin, User"));
            });

            services.Configure<DuelSettings>(Configuration);

            services.AddSingleton<IRequestProvider, RequestProvider>();

            services.AddAuth(Configuration)
                    .AddMySql()
                    .AddMvc(options=> options.EnableEndpointRouting=false)
                    .AddJsonOptions()
                    .AddDataAnnotationsLocalization(typeof(DuelResource).Name)
                    .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddLocalizationCustom();

            services.AddTransient<IRedisClient>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<DuelSettings>>().Value;
                return new PooledRedisClientManager(settings.Redis).GetClient();
            });

            ConfigureOtherService(services);

            services
                    .AddRabbitMq(Configuration)
                    .AddCorsConfigure();

            services.AddTransient<IScoreRankingRepository, ScoreRankingRepository>();

            services.AddTransient<IContestDateRepository, ContestDateRepository>();

            services.AddTransient<IDuelUserRepository, DuelUserRepository>();

            services.AddTransient<IQuestionRepository, QuestionRepository>();

            services.AddTransient<IAskedQuestionRepository, AskedQuestionRepository>();

            services.AddTransient<IDuelRepository, DuelRepository>();

            services.AddTransient<IDuelDetailRepository, DuelDetailRepository>();

            services.AddTransient<INumberFormatService, NumberFormatService>();

            services.AddTransient<IUserAnswerRepository, UserAnswerRepository>();

            services.AddSingleton<IScoreCalculator, ScoreCalculator>();

            services.AddTransient<IFileUploadService, S3FileUploadService>();

            #region S3 settings

            string awsAccessKeyId = Configuration["AwsAccessKeyId"];
            string awsSecretAccessKey = Configuration["AwsSecretAccessKey"];

            services.AddSingleton<IAmazonS3>(new AmazonS3Client(awsAccessKeyId, awsSecretAccessKey, Amazon.RegionEndpoint.EUCentral1));

            #endregion S3 settings

            #region Event handler

            services.AddTransient<DeliverGoldToWinnersIntegrationEventHandler>();

            services.AddTransient<DuelEscapeIntegrationEventHandler>();

            services.AddTransient<DuelFinishIntegrationEventHandler>();

            services.AddTransient<DuelStartIntegrationEventHandler>();

            services.AddTransient<RemoveWaitingOpponentIntegrationEventHandler>();

            services.AddTransient<UserAnswerIntegrationEventHandler>();

            services.AddTransient<WaitingOpponentIntegrationEventHandler>();

            #endregion Event handler

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

        protected virtual void ConfigureOtherService(IServiceCollection services)
        {
            services.AddTransient<IRequestProvider, RequestProvider>();

            services.AddTransient<IIdentityService, IdentityService>();

            services.AddTransient<IFollowService, FollowService>();

            services.AddTransient<IBalanceService, BalanceService>();

            services.AddTransient<ISubCategoryService, SubCategoryService>();
        }

        protected virtual void ConfigureAuth(IApplicationBuilder app)
        {
            app.UseAuth();
        }

        protected virtual void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            eventBus.Subscribe<DeliverGoldToWinnersIntegrationEvent, DeliverGoldToWinnersIntegrationEventHandler>();

            eventBus.Subscribe<DuelEscapeIntegrationEvent, DuelEscapeIntegrationEventHandler>();

            eventBus.Subscribe<DuelFinishIntegrationEvent, DuelFinishIntegrationEventHandler>();

            eventBus.Subscribe<DuelStartIntegrationEvent, DuelStartIntegrationEventHandler>();

            eventBus.Subscribe<RemoveWaitingOpponentIntegrationEvent, RemoveWaitingOpponentIntegrationEventHandler>();

            eventBus.Subscribe<UserAnswerIntegrationEvent, UserAnswerIntegrationEventHandler>();

            eventBus.Subscribe<WaitingOpponentIntegrationEvent, WaitingOpponentIntegrationEventHandler>();
        }
    }
}
