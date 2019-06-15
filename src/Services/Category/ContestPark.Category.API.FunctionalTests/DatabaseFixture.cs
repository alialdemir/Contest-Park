using ContestPark.Category.API.Infrastructure.ElasticSearch;
using ContestPark.Category.API.Infrastructure.Repositories.Search;
using ContestPark.Core.FunctionalTests;
using Microsoft.Azure.Documents.Client;
using System;
using Xunit;

namespace ContestPark.Category.API.FunctionalTests
{
    public class DatabaseFixture : IDisposable
    {
        public DatabaseFixture()
        {
        }

        public void Dispose()
        {
            // ... clean up test data from the database ...

            DocumentClient documentClient = new DocumentClient(new Uri(Conf.Configuration["CosmosDbServiceEndpoint"]),
                Conf.Configuration["CosmosDbAuthKeyOrResourceToken"]);
            documentClient.DeleteDatabaseAsync(UriFactory.CreateDatabaseUri(Conf.Configuration["CosmosDbDatabaseId"])).Wait();

            ElasticContext elasticContext = new ElasticContext(new Nest.ConnectionSettings(new Uri(Conf.Configuration["ElasticSearchURI"])), null);
            SearchRepository searchRepository = new SearchRepository(elasticContext, null, Conf.Configuration, null);
            searchRepository.DeleteSearchIndexs();
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