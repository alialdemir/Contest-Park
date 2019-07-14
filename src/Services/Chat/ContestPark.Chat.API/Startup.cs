using Autofac;
using Autofac.Extensions.DependencyInjection;
using ContestPark.Chat.API.Infrastructure.Repositories.Block;
using ContestPark.Chat.API.Infrastructure.Repositories.Conversation;
using ContestPark.Chat.API.Infrastructure.Repositories.Message;
using ContestPark.Chat.API.IntegrationEvents.EventHandling;
using ContestPark.Chat.API.IntegrationEvents.Events;
using ContestPark.Chat.API.Resources;
using ContestPark.Core.Services.Identity;
using ContestPark.Core.Services.RequestProvider;
using ContestPark.EventBus.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;

namespace ContestPark.Chat.API
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
            services.Configure<ChatSettings>(Configuration);

            services.AddAuth(Configuration)
                    .AddMySql()
                    .AddMvc()
                    .AddJsonOptions()
                    .AddDataAnnotationsLocalization(typeof(ChatResource).Name)
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //By connecting here we are making sure that our service
            //cannot start until redis is ready. This might slow down startup,
            //but given that there is a delay on resolving the ip address
            //and then creating the connection it seems reasonable to move
            //that cost to startup instead of having the first request pay the
            //penalty.
            services.AddSingleton<ConnectionMultiplexer>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<ChatSettings>>().Value;
                var configuration = ConfigurationOptions.Parse(settings.Redis, true);

                configuration.ResolveDns = true;

                return ConnectionMultiplexer.Connect(configuration);
            });

            services.AddLocalizationCustom();

            services
                    .AddRabbitMq(Configuration)
                    .AddCorsConfigure();

            ConfigureIdentityService(services);

            services.AddTransient<IConversationRepository, ConversationRepository>();
            services.AddTransient<IMessageRepository, MessageRepository>();
            services.AddTransient<IBlockRepository, BlockRepository>();

            services.AddTransient<SendMessageIntegrationEventHandler>();
            services.AddTransient<RemoveMessagesIntegrationEventHandler>();

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

            eventBus.Subscribe<SendMessageIntegrationEvent, SendMessageIntegrationEventHandler>();
            eventBus.Subscribe<RemoveMessagesIntegrationEvent, RemoveMessagesIntegrationEventHandler>();
        }
    }
}
