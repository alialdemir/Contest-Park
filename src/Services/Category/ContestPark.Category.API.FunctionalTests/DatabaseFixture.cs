﻿using ContestPark.Category.API.Infrastructure.ElasticSearch;
using ContestPark.Core.Dapper.Extensions;
using ContestPark.Core.FunctionalTests;
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

            DatabaseExtension.DeleteDatabase(Conf.Configuration["ConnectionString"]);

            ElasticContext elasticContext = new ElasticContext(new Nest.ConnectionSettings(new Uri(Conf.Configuration["ElasticSearchURI"])), null);
            //SearchRepository searchRepository = new SearchRepository(elasticContext, null, Conf.Configuration, null);
            //searchRepository.DeleteSearchIndexs();
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
