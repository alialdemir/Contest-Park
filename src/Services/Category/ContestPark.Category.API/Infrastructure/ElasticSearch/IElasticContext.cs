using ContestPark.Category.API.Infrastructure.Documents;
using ContestPark.Category.API.Model;
using ContestPark.Core.CosmosDb.Models;
using ContestPark.Core.Enums;
using Nest;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Category.API.Infrastructure.ElasticSearch
{
    public interface IElasticContext
    {
        bool CreateIndex<T>(string indexName, string aliasName) where T : class, ISearchBase;

        bool BulkIndex<T>(string indexName, List<T> document) where T : class;

        bool Index<T>(string indexName, T document) where T : class;

        SearchResponseModel<T> Search<T>(ISearchRequest searchRequest) where T : class;

        Task<List<Search>> SearchAsync(string indexName, string searchText);
    }
}