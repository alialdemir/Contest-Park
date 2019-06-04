using Autofac;
using Autofac.Extensions.DependencyInjection;
using ContestPark.Identity.API.Data;
using ContestPark.Identity.API.Data.Repositories.User;
using ContestPark.Identity.API.Models;
using ContestPark.Identity.API.Resources;
using ContestPark.Identity.API.Services;
using ContestPark.Identity.API.Services.BlobStorage;
using ContestPark.Identity.API.Services.Login;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
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
                .UseCustomIdentityServer()
                .UseRequestLocalizationCustom()
                .UseMvc();
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

            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IUserRepository, UserRepository>();

            services.AddTransient<IBlobStorageService, BlobStorageService>();

            #endregion AddTransient

            services.AddCustomIdentityServer(Configuration, connectionString)
                    .AddMvc()
                    .AddJsonOptions()
                    .AddDataAnnotationsLocalization(typeof(IdentityResource).Name)
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddLocalizationCustom();

            services
                .AddIntegrationEventLogEFDbContext(connectionString)
                .AddRabbitMq(Configuration);

            var container = new ContainerBuilder();
            container.Populate(services);
            return new AutofacServiceProvider(container.Build());
        }
    }
}