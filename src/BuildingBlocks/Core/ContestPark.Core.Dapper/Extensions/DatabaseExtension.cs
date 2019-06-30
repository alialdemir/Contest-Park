using ContestPark.Core.Dapper.Abctract;
using Dapper;
using FluentMigrator.Builders.Execute;
using FluentMigrator.Runner;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ContestPark.Core.Dapper.Extensions
{
    public static class DatabaseExtension
    {
        private static DatabaseInfo GetDatabaseInfo(string connectionString)
        {
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

        public static IMigrationRunner CreateDatabaseIfNotExists(this IMigrationRunner runner, string connectionString)
        {
            DatabaseInfo databaseInfo = GetDatabaseInfo(connectionString);

            GetConnection(databaseInfo)
                .Connection.Execute($"CREATE DATABASE IF NOT EXISTS {databaseInfo.DatabaseName};");

            return runner;
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
