using Autofac;
using Autofac.Extensions.DependencyInjection;
using ContestPark.Category.API.Infrastructure.Repositories.Category;
using ContestPark.Category.API.Infrastructure.Repositories.OpenCategory;
using ContestPark.Category.API.Resources;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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

            services.AddAuth(Configuration)
                    .AddCosmosDb(Configuration)
                    .AddMvc()
                    .AddJsonOptions()
                    .AddDataAnnotationsLocalization(typeof(CategoryResource).Name)
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddLocalizationCustom();

            services
                    .AddRabbitMq(Configuration)
                    .AddCorsConfigure();

            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddTransient<IOpenCategoryRepository, OpenCategoryRepository>();

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
               .AddCors()
               .UseAuth()
               .UseRequestLocalizationCustom()
               .UseMvc();
        }
    }
}