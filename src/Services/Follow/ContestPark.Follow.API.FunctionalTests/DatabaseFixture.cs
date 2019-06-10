using ContestPark.Core.FunctionalTests;
using Microsoft.Azure.Documents.Client;
using System;
using Xunit;

namespace ContestPark.Follow.API.FunctionalTests
{
    public class DatabaseFixture : IDisposable
    {
        public DatabaseFixture()
        {
        }

        public void Dispose()
        {
            DocumentClient documentClient = new DocumentClient(new Uri(Conf.Configuration["CosmosDbServiceEndpoint"]),
                Conf.Configuration["CosmosDbAuthKeyOrResourceToken"]);

            // ... clean up test data from the database ...
            documentClient.DeleteDatabaseAsync(UriFactory.CreateDatabaseUri(Conf.Configuration["CosmosDbDatabaseId"])).Wait();
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