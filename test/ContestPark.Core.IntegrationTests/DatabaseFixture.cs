using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ContestPark.Core.IntegrationTests
{
    public class DatabaseFixtureBase : IDisposable
    {
        public DatabaseFixtureBase()
        {
            // ... initialize data in the test database ...
        }

        public virtual void Dispose()
        {
            //DocumentClient documentClient = new DocumentClient(new Uri(FollowScenariosBase.Configuration["CosmosDbServiceEndpoint"]),
            //    FollowScenariosBase.Configuration["CosmosDbAuthKeyOrResourceToken"]);

            //var d = documentClient.DeleteDatabaseAsync(UriFactory.CreateDatabaseUri(FollowScenariosBase.Configuration["CosmosDbDatabaseId"])).Result;
            // ... clean up test data from the database ...
        }
    }

    [CollectionDefinition("Database remove")]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixtureBase>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}