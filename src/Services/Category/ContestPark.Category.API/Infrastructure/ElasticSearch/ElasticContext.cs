using ContestPark.Category.API.Model;
using Nest;
using System.Collections.Generic;
using System.Linq;

namespace ContestPark.Category.API.Infrastructure.ElasticSearch
{
    public class ElasticContext : IElasticContext
    {
        private readonly ConnectionSettings _connectionSettings;
        private readonly ElasticClient _elasticClient;

        public ElasticContext(ConnectionSettings connectionSettings)
        {
            _connectionSettings = connectionSettings;

            _elasticClient = new ElasticClient(_connectionSettings);
        }

        /// <summary>
        /// Elasticsearch index oluşturur
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="indexName"></param>
        /// <param name="aliasName"></param>
        /// <returns></returns>
        public IndexResponseModel CreateIndex<T>(string indexName, string aliasName) where T : class
        {
            var createIndexDescriptor = new CreateIndexDescriptor(indexName)
            .Mappings(ms => ms
            .Map<T>(m => m.AutoMap())
            )
            .Aliases(a => a.Alias(aliasName));

            var response = _elasticClient.CreateIndex(createIndexDescriptor);

            return new IndexResponseModel
            {
                IsValid = response.IsValid,
                StatusMessage = response.DebugInformation,
                Exception = response.OriginalException
            };
        }

        /// <summary>
        /// indexing işlemlerini gerçekleştirmektedir
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="indexName"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        public IndexResponseModel Index<T>(string indexName, T document) where T : class
        {
            var response = _elasticClient.Index(document, i => i
                           .Index(indexName)
                           .Type<T>());

            return new IndexResponseModel
            {
                IsValid = response.IsValid,
                StatusMessage = response.DebugInformation,
                Exception = response.OriginalException
            };
        }

        /// <summary>
        /// Güncelleme
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="indexName"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        public IndexResponseModel BulkIndex<T>(string indexName, List<T> document) where T : class
        {
            var response = _elasticClient.IndexMany(document, indexName);

            return new IndexResponseModel
            {
                IsValid = response.IsValid,
                StatusMessage = response.DebugInformation,
                Exception = response.OriginalException
            };
        }

        /// <summary>
        /// Search
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="searchRequest"></param>
        /// <returns></returns>
        public SearchResponseModel<T> Search<T>(ISearchRequest searchRequest) where T : class
        {
            var response = _elasticClient.Search<T>(searchRequest);

            return new SearchResponseModel<T>()
            {
                IsValid = response.IsValid,
                StatusMessage = response.DebugInformation,
                Exception = response.OriginalException,
                Documents = response.Documents
            };
        }
    }
}