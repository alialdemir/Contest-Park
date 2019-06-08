using Microsoft.Azure.Documents.Client;
using System;
using Xunit;

namespace ContestPark.Follow.API.FunctionalTests
{
    public class DatabaseFixture : IDisposable
    {
        public DatabaseFixture()
        {
            // ... initialize data in the test database ...
        }

        public void Dispose()
        {
            DocumentClient documentClient = new DocumentClient(new Uri(FollowScenariosBase.Configuration["CosmosDbServiceEndpoint"]),
                FollowScenariosBase.Configuration["CosmosDbAuthKeyOrResourceToken"]);

            var d = documentClient.DeleteDatabaseAsync(UriFactory.CreateDatabaseUri(FollowScenariosBase.Configuration["CosmosDbDatabaseId"])).Result;
            // ... clean up test data from the database ...
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