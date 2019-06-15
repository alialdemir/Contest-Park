using Nest;
using System.Collections.Generic;
using System.Linq;

namespace ContestPark.Category.API.Infrastructure.ElasticSearch.BusinessEngines
{
    public class ElasticSearchEngine : IElasticSearchEngine
    {
        #region Private Variables

        private readonly string _indexName;
        private readonly int _size;
        private readonly int _from;
        private readonly IQueryContainer _queryContainer;
        private readonly IElasticContext _elasticContext;

        #endregion Private Variables

        #region Methods

        public ElasticSearchEngine(ElasticSearchBuilder elasticSearchBuilder)
        {
            _indexName = elasticSearchBuilder.IndexName;
            _size = elasticSearchBuilder.Size;
            _from = elasticSearchBuilder.From;
            _queryContainer = elasticSearchBuilder.QueryContainer;
            _elasticContext = elasticSearchBuilder.ElasticContext;
        }

        #endregion Methods

        #region IElasticSearchEngine Members

        public List<T> Execute<T>() where T : class
        {
            var response = _elasticContext.Search<T>(new SearchRequest(_indexName, typeof(T))
            {
                Size = _size,
                From = _from,
                Query = (QueryContainer)_queryContainer
            });

            if (response.IsValid)
            {
                return response.Documents.ToList();
            }

            return null;
        }

        #endregion IElasticSearchEngine Members
    }
}