using ContestPark.Core.FunctionalTests;
using Microsoft.Azure.Documents.Client;
using System;

namespace ContestPark.Category.API.FunctionalTests
{
    public class DatabaseFixture : DatabaseFixtureBase
    {
        public override void Dispose()
        {
            DocumentClient documentClient = new DocumentClient(new Uri(Conf.Configuration["CosmosDbServiceEndpoint"]),
                Conf.Configuration["CosmosDbAuthKeyOrResourceToken"]);

            var d = documentClient.DeleteDatabaseAsync(UriFactory.CreateDatabaseUri(Conf.Configuration["CosmosDbDatabaseId"])).Result;
            // ... clean up test data from the database ...
        }
    }
}