using Autofac;
using Autofac.Extensions.DependencyInjection;
using ContestPark.EventBus.Abstractions;
using ContestPark.Post.API.Infrastructure.Repositories.Like;
using ContestPark.Post.API.Infrastructure.Repositories.Post;
using ContestPark.Post.API.IntegrationEvents.EventHandling;
using ContestPark.Post.API.IntegrationEvents.Events;
using ContestPark.Post.API.Resources;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace ContestPark.Post.API
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
            services.Configure<PostSettings>(Configuration);

            services.AddAuth(Configuration)
                    .AddCosmosDb(Configuration)
                    .AddMvc()
                    .AddJsonOptions()
                    .AddDataAnnotationsLocalization(typeof(PostResource).Name)
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddLocalizationCustom();

            services
                    .AddRabbitMq(Configuration)
                    .AddCorsConfigure();

            services.AddTransient<IPostRepository, PostRepository>();
            services.AddTransient<ILikeRepository, LikeRepository>();

            services.AddTransient<NewPostAddedIntegrationEventHandler>();
            services.AddTransient<PostLikeIntegrationEventHandler>();
            services.AddTransient<PostUnLikeIntegrationEventHandler>();
            services.AddTransient<NewUserRegisterIntegrationEventHandler>();
            services.AddTransient<UserInfoChangedIntegrationEventHandler>();
            services.AddTransient<ProfilePictureChangedIntegrationEventHandler>();
            services.AddTransient<UserNotFoundIntegrationEventHandler>();
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
            app.UseAuth();
        }

        protected virtual void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            eventBus.Subscribe<NewPostAddedIntegrationEvent, NewPostAddedIntegrationEventHandler>();
            eventBus.Subscribe<PostLikeIntegrationEvent, PostLikeIntegrationEventHandler>();
            eventBus.Subscribe<PostUnLikeIntegrationEvent, PostUnLikeIntegrationEventHandler>();
            eventBus.Subscribe<NewUserRegisterIntegrationEvent, NewUserRegisterIntegrationEventHandler>();
            eventBus.Subscribe<UserInfoChangedIntegrationEvent, UserInfoChangedIntegrationEventHandler>();
            eventBus.Subscribe<ProfilePictureChangedIntegrationEvent, ProfilePictureChangedIntegrationEventHandler>();
            eventBus.Subscribe<UserNotFoundIntegrationEvent, UserNotFoundIntegrationEventHandler>();
        }
    }
}
