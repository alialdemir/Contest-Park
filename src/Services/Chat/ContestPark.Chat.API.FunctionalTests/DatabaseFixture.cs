using ContestPark.Core.Dapper.Extensions;
using ContestPark.Core.FunctionalTests;
using System;
using Xunit;

namespace ContestPark.Chat.API.FunctionalTests
{
    public class DatabaseFixture : IDisposable
    {
        public DatabaseFixture()
        {
        }

        public void Dispose()
        {
            // ... clean up test data from the database ...
            DatabaseExtension.DeleteDatabase(Conf.Configuration["ConnectionString"]);
        }
    }

    [CollectionDefinition("Database remove")]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
