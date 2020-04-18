using Autofac;
using Autofac.Extensions.DependencyInjection;
using ContestPark.Balance.API.Infrastructure.Repositories.Balance;
using ContestPark.Balance.API.Infrastructure.Repositories.BalanceHistory;
using ContestPark.Balance.API.Infrastructure.Repositories.MoneyWithdrawRequest;
using ContestPark.Balance.API.Infrastructure.Repositories.PurchaseHistory;
using ContestPark.Balance.API.Infrastructure.Repositories.Reference;
using ContestPark.Balance.API.Infrastructure.Repositories.ReferenceCode;
using ContestPark.Balance.API.IntegrationEvents.EventHandling;
using ContestPark.Balance.API.IntegrationEvents.Events;
using ContestPark.Balance.API.Resources;
using ContestPark.Balance.API.Services.VerifyReceiptIos;
using ContestPark.Core.Middlewares;
using ContestPark.Core.Services.RequestProvider;
using ContestPark.EventBus.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace ContestPark.Balance.API
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
            services.Configure<BalanceSettings>(Configuration);

            services.AddSingleton<IRequestProvider, RequestProvider>();

            services.AddAuth(Configuration)
                    .AddMySql()
                    .AddMvc()
                    .AddJsonOptions()
                    .AddDataAnnotationsLocalization(typeof(BalanceResource).Name)
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddLocalizationCustom();

            services
                    .AddRabbitMq(Configuration)
                    .AddCorsConfigure();

            services.AddTransient<IBalanceRepository, BalanceRepository>();
            services.AddTransient<IPurchaseHistoryRepository, PurchaseHistoryRepository>();
            services.AddTransient<IReferenceRepository, ReferenceRepository>();
            services.AddTransient<IReferenceCodeRepostory, ReferenceCodeRepostory>();
            services.AddTransient<IBalanceHistoryRepository, BalanceHistoryRepository>();

            services.AddTransient<IMoneyWithdrawRequestRepository, MoneyWithdrawRequestRepository>();

            services.AddTransient<IVerifyReceiptIos, VerifyReceiptIos>();

            services.AddTransient<ChangeBalanceIntegrationEventHandler>();
            services.AddTransient<MultiChangeBalanceIntegrationEventHandler>();

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

        protected virtual void ConfigureAuth(IApplicationBuilder app)
        {
            app
                .UseMiddleware<ServiceAuthorizeMiddleware>()
                .UseAuth();
        }

        protected virtual void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            eventBus.Subscribe<ChangeBalanceIntegrationEvent, ChangeBalanceIntegrationEventHandler>();
            eventBus.Subscribe<MultiChangeBalanceIntegrationEvent, MultiChangeBalanceIntegrationEventHandler>();
        }
    }
}
