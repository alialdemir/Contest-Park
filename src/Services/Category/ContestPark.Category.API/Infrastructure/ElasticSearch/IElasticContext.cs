using ContestPark.Category.API.Model;
using Nest;
using System.Collections.Generic;

namespace ContestPark.Category.API.Infrastructure.ElasticSearch
{
    public interface IElasticContext
    {
        IndexResponseModel CreateIndex<T>(string indexName, string aliasName) where T : class;

        IndexResponseModel BulkIndex<T>(string indexName, List<T> document) where T : class;

        IndexResponseModel Index<T>(string indexName, T document) where T : class;

        SearchResponseModel<T> Search<T>(ISearchRequest searchRequest) where T : class;
    }
}