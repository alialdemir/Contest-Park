using Autofac;
using Autofac.Extensions.DependencyInjection;
using ContestPark.BackgroundTasks.Services.Duel;
using ContestPark.BackgroundTasks.Tasks;
using ContestPark.Core.Services.RequestProvider;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace ContestPark.BackgroundTasks
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public virtual IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services
                .Configure<BackgroundTaskSettings>(this.Configuration)
                .AddOptions()
                .AddHostedService<NewContestDateTask>()
                .AddRabbitMq(Configuration);

            services.AddSingleton<IRequestProvider, RequestProvider>();

            services.AddSingleton<IDuelService, DuelService>();

            services.AddHostedService<NewContestDateTask>();

            var container = new ContainerBuilder();
            container.Populate(services);

            return new AutofacServiceProvider(container.Build());
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            var pathBase = Configuration["PATH_BASE"];
            if (!string.IsNullOrEmpty(pathBase))
            {
                loggerFactory.CreateLogger("init").LogDebug($"Using PATH BASE '{pathBase}'");
                app.UsePathBase(pathBase);
            }
        }
    }
}
