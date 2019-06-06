using ContestPark.Category.API.Infrastructure.ElasticSearch;
using ContestPark.Category.API.Infrastructure.ElasticSearch.BusinessEngines;
using ContestPark.Category.API.Model;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace ContestPark.Category.API.Infrastructure.Repositories.Search
{
    public class SearchRepository : ISearchRepository
    {
        private readonly IElasticContext _elasticContext;
        private readonly string ElasticSearchIndexName;

        public SearchRepository(IElasticContext elasticContext,
                                IConfiguration configuration)
        {
            ElasticSearchIndexName = configuration["ElasticSearchIndexName"];
            _elasticContext = elasticContext;
        }

        public void CreateCategoryIndex()
        {
            string indexName = ElasticSearchIndexName;
            string aliasName = ElasticSearchIndexName;

            _elasticContext.CreateIndex<SearchModel>(indexName, aliasName);
        }

        public void Insert(SearchModel searchModel)
        {
            _elasticContext.Index(ElasticSearchIndexName, searchModel);
        }

        public void Update(SearchModel searchModel)
        {
            _elasticContext.BulkIndex<SearchModel>(ElasticSearchIndexName, new List<SearchModel>
            {
                searchModel
            });
        }

        /// <summary>
        /// user id ait search modeli getirir
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public SearchModel SearchByUserId(string userId)
        {
            var elasticSearchEngine = new ElasticSearchBuilder(ElasticSearchIndexName, _elasticContext)
                    .SetSize(1)
                    .SetFrom(0)
                    .AddTermQuery<SearchModel>("_id", userId)
                    .Build()
                    .Execute<SearchModel>();

            return elasticSearchEngine.First();
        }
    }
}