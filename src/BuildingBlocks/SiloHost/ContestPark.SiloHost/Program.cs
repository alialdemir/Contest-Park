using ContestPark.Core.DataSeed;
using ContestPark.SiloHost.Migration;
using System;
using System.Threading.Tasks;

namespace ContestPark.SiloHost
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Task.Factory.StartNew(async () => await Migration());

            SiloHostWrapper.Instance.Init();
        }

        private static async Task Migration()
        {
            bool isMigrateDatabase = Environment.GetEnvironmentVariable("IsMigrateDatabase") == "enable";
            if (!isMigrateDatabase)
                return;

            DatabaseMigration migration = new DatabaseMigration();

            bool isSuccess = await migration.CreateDatabase();

            if (isSuccess)
                migration.Run(null, typeof(Initial).Assembly);
        }
    }
}