using ContestPark.Core.Interfaces;
using ContestPark.Core.Model;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ContestPark.Core.DataSeed
{
    /// <summary>
    /// Database oluşturur ve otomatik olarak micro servisler içerisinde bulunan
    /// migration ve data seeder sınıflarını bulup gerekli methodları tetikler.
    /// </summary>
    public class DatabaseMigration
    {
        private ISettingsBase Settings => new SettingsBase();

        public ILogger<DatabaseMigration> Logger { get; set; }

        public static async Task MigrateUp(ILogger<DatabaseMigration> logger)
        {
            DatabaseMigration migration = new DatabaseMigration
            {
                Logger = logger
            };

            Assembly[] migrationAssemblies = migration.GetMigrationAssemblies("FluentMigrator.Migration");

            await migration.CreateDatabase();

            migration.Run(migration.SeenDatabase, migrationAssemblies);
        }

        /// <summary>
        /// Veri tabanı için gerekli dataların seed classlarını bulup çalıştırır
        /// </summary>
        public void SeenDatabase()
        {
            DatabaseMigration migration = new DatabaseMigration();

            const string seederClassName = "ContestPark.Core.DataSeed.ContextSeedBase";

            Assembly[] seederAssemblies = migration.GetMigrationAssemblies(seederClassName);

            Type[] types = GetSeederTypes(seederAssemblies, seederClassName);

            foreach (Type type in types)
            {
                if (!(Activator.CreateInstance(type) is ContextSeedBase runnable))
                    throw new ArgumentNullException(nameof(runnable));

                runnable.SeedAsync(new SettingsBase(), Logger);
            }
        }

        /// <summary>
        /// Update the database
        /// </summary>
        public void Run(Action databaseSeeder, params Assembly[] assemblies)
        {
            var retry = GetRetryPolicy();

            //var logger = services.GetRequiredService<ILogger<IWebHost>>();

            try
            {
                retry.Execute(() =>
                {
                    //   logger.LogInformation($"Migrating database associated with context {nameof(UpdateDatabaseAsync)}");

                    // Instantiate the runner
                    var runner = CreateServices(assemblies).GetRequiredService<IMigrationRunner>();

                    // Execute the migrations
                    runner.MigrateUp();
                    databaseSeeder?.Invoke();

                    //            logger.LogInformation($"Migrated database associated with context {nameof(UpdateDatabaseAsync)}");
                });
            }
            catch //(Exception ex)
            {
                //logger.LogError(ex, $"An error occurred while migrating the database used on context {nameof(UpdateDatabaseAsync)}");
            }
        }

        /// <summary>
        /// Fluent migration classından türmiş initial classlarını getirir
        /// </summary>
        /// <returns></returns>
        private Assembly[] GetMigrationAssemblies(string baseClassName)
        {
            Assembly[] assemblies = (from asm in AppDomain.CurrentDomain.GetAssemblies()
                                     from type in asm.GetTypes()
                                     where type.IsClass && type.BaseType != null && type.BaseType.FullName != null && type.BaseType.FullName.Contains(baseClassName)
                                     select asm).ToArray();

            return assemblies;
        }

        /// <summary>
        /// İlgili assemblies içindeki seeder classlarını döndürür
        /// </summary>
        /// <param name="seederAssemblies"></param>
        /// <param name="seederClassName"></param>
        /// <returns></returns>
        private Type[] GetSeederTypes(Assembly[] seederAssemblies, string seederClassName)
        {
            Type[] types = (from asm in seederAssemblies
                            from type in asm.GetTypes()
                            where type.IsClass && type.BaseType != null && type.BaseType.FullName != null && type.BaseType.FullName.Contains(seederClassName)
                            select type).ToArray();

            return types;
        }

        /// <summary>
        /// Database oluşturur
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <param name="databaseName">Db name</param>
        public async Task<bool> CreateDatabase(string databaseName = "ContestPark")
        {
            try
            {
                string connectionString = Settings.ConnectionString.Replace("Database=ContestPark", "Database=master");// master üzerinde database create işlemi yapıldı
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = $@"if db_id('{databaseName}') is null
                                      CREATE DATABASE {databaseName}";
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private RetryPolicy GetRetryPolicy()
        {
            return Policy.Handle<Exception>()
                .WaitAndRetry(new TimeSpan[]
                {
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(20),
                    TimeSpan.FromSeconds(30),
                });
        }

        /// <summary>
        /// Configure the dependency injection services
        /// </sumamry>
        private IServiceProvider CreateServices(params Assembly[] assemblies)
        {
            string connectionString = Settings.ConnectionString;

            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            return new ServiceCollection()
                // Add common FluentMigrator services
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    // Add SQLite support to FluentMigrator
                    //.AddSQLite()
                    .AddSqlServer()
                    // Set the connection string
                    .WithGlobalConnectionString(connectionString)
                    // Define the assembly containing the migrations
                    .ScanIn(assemblies).For.Migrations())
                // Enable logging to console in the FluentMigrator way
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                // Build the service provider
                .BuildServiceProvider(false);
        }
    }
}