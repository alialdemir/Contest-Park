using ContestPark.Core.Dapper.Abctract;
using Dapper;
using FluentMigrator;
using FluentMigrator.Builders.Create.Table;
using FluentMigrator.Builders.Execute;
using FluentMigrator.Infrastructure;
using FluentMigrator.Runner;
using System;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ContestPark.Core.Dapper.Extensions
{
    public static class DatabaseExtension
    {
        public static IFluentSyntax CreateTableIfNotExists(this MigrationBase self, string tableName, Func<ICreateTableWithColumnOrSchemaOrDescriptionSyntax, IFluentSyntax> constructTableFunction, string schemaName = "dbo")
        {
            if (!self.Schema.Schema(schemaName).Table(tableName).Exists())
            {
                return constructTableFunction(self.Create.Table(tableName));
            }
            else
                return null;
        }

        private static DatabaseInfo GetDatabaseInfo(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            DbConnectionStringBuilder builder = new DbConnectionStringBuilder
            {
                ConnectionString = connectionString
            };

            return new DatabaseInfo
            {
                Server = builder["server"] as string,
                DatabaseName = builder["database"] as string,
                Password = builder["pwd"] as string,
                UserId = builder["uid"] as string
            };
        }

        private static DatabaseConnection GetConnection(DatabaseInfo databaseInfo)
        {
            return new DatabaseConnection($"server={databaseInfo.Server};uid={databaseInfo.UserId};pwd={databaseInfo.Password};");
        }

        public static async Task<bool> CreateDatabaseIfNotExistsAsync(this IMigrationRunner runner, string connectionString)
        {
            DatabaseInfo databaseInfo = GetDatabaseInfo(connectionString);

            DatabaseConnection databaseConnection = GetConnection(databaseInfo);

            var dbConnection = databaseConnection.Connection;

            string dbName = dbConnection.QuerySingleOrDefault<string>($"SHOW DATABASES LIKE '{databaseInfo.DatabaseName}'");
            int result = await dbConnection.ExecuteAsync($"CREATE DATABASE IF NOT EXISTS `{databaseInfo.DatabaseName}`;");

            dbConnection.Close();

            return result > 0 && string.IsNullOrEmpty(dbName);
        }

        /// <summary>
        /// Ebedded resource oalrak eklemiş sql script dosyalarını aseembly içerisinde bulup execute eder
        /// </summary>
        /// <param name="runner"></param>
        /// <param name="assembly"></param>
        /// <param name="scriptNames"></param>
        /// <returns></returns>
        public static void ExecuteScripts(this IExecuteExpressionRoot runner, Assembly assembly, params string[] scriptNames)
        {
            var scriptsPath = assembly.GetManifestResourceNames().Where(x => x.EndsWith(".sql"));

            foreach (var script in scriptsPath)
            {
                if (!scriptNames.Where(x => script.Contains(x)).Any())
                    continue;

                string sqlScript = GetEmbeddedResource(script, assembly);
                if (string.IsNullOrEmpty(sqlScript))
                    continue;

                runner.WithConnection(async (dbConnection, x) => await dbConnection.ExecuteAsync(sqlScript));
            }
        }

        /// <summary>
        /// Projenin içinde ebedded resource dosyalarını verir
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="assembly"></param>
        /// <returns>Embedded resource</returns>
        public static string GetEmbeddedResource(string resourceName, Assembly assembly)
        {
            using (Stream resourceStream = assembly.GetManifestResourceStream(resourceName))
            {
                if (resourceStream == null)
                    return null;

                using (StreamReader reader = new StreamReader(resourceStream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public static void DeleteDatabase(string connectionString)
        {
            DatabaseInfo databaseInfo = GetDatabaseInfo(connectionString);

            GetConnection(databaseInfo)
                .Connection.Execute($"DROP DATABASE {databaseInfo.DatabaseName};");
        }

        private class DatabaseInfo
        {
            public string Server { get; set; }
            public string UserId { get; set; }
            public string Password { get; set; }
            public string DatabaseName { get; set; }
        }
    }
}
