using Autofac;
using Autofac.Extensions.DependencyInjection;
using ContestPark.Core.Services.Identity;
using ContestPark.Core.Services.RequestProvider;
using ContestPark.Duel.API.Infrastructure.Repositories.AskedQuestion;
using ContestPark.Duel.API.Infrastructure.Repositories.ContestDate;
using ContestPark.Duel.API.Infrastructure.Repositories.Duel;
using ContestPark.Duel.API.Infrastructure.Repositories.DuelDetail;
using ContestPark.Duel.API.Infrastructure.Repositories.Question;
using ContestPark.Duel.API.Infrastructure.Repositories.Redis.DuelUser;
using ContestPark.Duel.API.Infrastructure.Repositories.ScoreRankingRepository;
using ContestPark.Duel.API.IntegrationEvents.EventHandling;
using ContestPark.Duel.API.IntegrationEvents.Events;
using ContestPark.Duel.API.Resources;
using ContestPark.Duel.API.Services.Follow;
using ContestPark.Duel.API.Services.NumberFormat;
using ContestPark.Duel.API.Services.SubCategory;
using ContestPark.EventBus.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            services.Configure<DuelSettings>(Configuration);

            services.AddSingleton<IRequestProvider, RequestProvider>();

            services.AddAuth(Configuration)
                    .AddMySql()
                    .AddMvc()
                    .AddJsonOptions()
                    .AddDataAnnotationsLocalization(typeof(DuelResource).Name)
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddLocalizationCustom();

            services.AddSingleton<IRedisClient>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<DuelSettings>>().Value;

                return new RedisClient(settings.Redis);
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

            services.AddTransient<WaitingOpponentIntegrationEventHandler>();

            services.AddTransient<RemoveWaitingOpponentIntegrationEventHandler>();

            services.AddTransient<DuelFinishIntegrationEventHandler>();

            services.AddTransient<DuelStartIntegrationEventHandler>();

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
            services.AddSingleton<IRequestProvider, RequestProvider>();

            services.AddSingleton<IIdentityService, IdentityService>();

            services.AddSingleton<IFollowService, FollowService>();

            services.AddSingleton<ISubCategoryService, SubCategoryService>();
        }

        protected virtual void ConfigureAuth(IApplicationBuilder app)
        {
            app.UseAuth();
        }

        protected virtual void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            eventBus.Subscribe<WaitingOpponentIntegrationEvent, WaitingOpponentIntegrationEventHandler>();

            eventBus.Subscribe<RemoveWaitingOpponentIntegrationEvent, RemoveWaitingOpponentIntegrationEventHandler>();

            eventBus.Subscribe<DuelFinishIntegrationEvent, DuelFinishIntegrationEventHandler>();

            eventBus.Subscribe<DuelStartIntegrationEvent, DuelStartIntegrationEventHandler>();
        }
    }
}
