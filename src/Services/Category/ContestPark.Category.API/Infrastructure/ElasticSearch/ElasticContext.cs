using ContestPark.Category.API.Infrastructure.Documents;
using ContestPark.Category.API.Model;
using Nest;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public bool CreateIndex<T>(string indexName, string aliasName) where T : class, ISearchBase
        {
            if (_elasticClient.IndexExists(indexName.ToLowerInvariant()).Exists)
                return true;

            var createIndexDescriptor = new CreateIndexDescriptor(indexName)
        .Mappings(ms => ms
                  .Map<T>(m => m
                        .AutoMap()
                        .Properties(ps => ps
                            .Completion(c => c
                                .Name(p => p.Suggest))))

                 );//.Aliases(a => a.Alias(aliasName));

            var response = _elasticClient.CreateIndex(createIndexDescriptor);

            return response.IsValid;
        }

        /// <summary>
        /// indexing işlemlerini gerçekleştirmektedir
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="indexName"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        public bool Index<T>(string indexName, T document) where T : class
        {
            var response = _elasticClient.Index(document, i => i
                           .Index(indexName)
                           .Type<T>());

            return response.IsValid;
        }

        /// <summary>
        /// Güncelleme
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="indexName"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        public bool BulkIndex<T>(string indexName, List<T> document) where T : class
        {
            var response = _elasticClient.IndexMany(document, indexName);

            return response.IsValid;
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

        /// <summary>
        /// Index ve search texte göre autocomplete seach yapar
        /// </summary>
        /// <param name="indexName">Index adı</param>
        /// <param name="searchText">Aranan kelime</param>
        /// <returns>Arama sonucu</returns>
        public async Task<List<Search>> SearchAsync(string indexName, string searchText)
        {
            ISearchResponse<Search> searchResponse = await _elasticClient.SearchAsync<Search>(s => s
                                 .Index(indexName)
                                 .Type<Search>()
                                 .Suggest(su => su
                                      .Completion("suggestions", c => c
                                           .Field(f => f.Suggest)
                                           .Prefix(searchText)
                                           .Fuzzy(f => f
                                               .Fuzziness(Fuzziness.Auto)
                                           ))
                                         ));
            if (!searchResponse.IsValid)
            {
                // TODO: elasticsearch hatası oluştu logu yazılmalı
                return new List<Search>();
            }

            return searchResponse.Suggest["suggestions"].Select(p => p.Options.Select(x => x.Source).ToList()).FirstOrDefault();
        }
    }
}