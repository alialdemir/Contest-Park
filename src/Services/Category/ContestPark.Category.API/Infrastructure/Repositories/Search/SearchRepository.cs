using AutoMapper;
using ContestPark.Category.API.Infrastructure.ElasticSearch;
using ContestPark.Category.API.Infrastructure.ElasticSearch.BusinessEngines;
using ContestPark.Category.API.Infrastructure.Repositories.FollowSubCategory;
using ContestPark.Category.API.Model;
using ContestPark.Core.CosmosDb.Models;
using ContestPark.Core.Enums;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Category.API.Infrastructure.Repositories.Search
{
    public class SearchRepository : ISearchRepository
    {
        #region Private Variables

        private readonly IElasticContext _elasticContext;
        private readonly string ElasticSearchIndexName;
        private readonly IFollowSubCategoryRepository _followSubCategoryRepository;
        private readonly IMapper _mapper;

        #endregion Private Variables

        #region Constructor

        public SearchRepository(IElasticContext elasticContext,
                             IFollowSubCategoryRepository followSubCategoryRepository,
                             IConfiguration configuration,
                             IMapper mapper)
        {
            ElasticSearchIndexName = configuration["ElasticSearchIndexName"];
            _elasticContext = elasticContext;
            _followSubCategoryRepository = followSubCategoryRepository;
            _mapper = mapper;
        }

        #endregion Constructor

        #region Methods

        public void CreateCategoryIndex()
        {
            string indexNameCategoryTurkish = GetIndexName(SearchTypes.Category, Languages.English);
            string indexNameEnglish = GetIndexName(SearchTypes.Category, Languages.Turkish);
            string indexNamePlayer = GetIndexName(SearchTypes.Player, null);
            string aliasName = ElasticSearchIndexName;

            _elasticContext.CreateIndex<Documents.Search>(indexNameCategoryTurkish, aliasName);
            _elasticContext.CreateIndex<Documents.Search>(indexNameEnglish, aliasName);
            _elasticContext.CreateIndex<Documents.Search>(indexNamePlayer, aliasName);
        }

        public void Insert(Documents.Search searchModel)
        {
            string indexName = GetIndexName(searchModel.SearchType, searchModel.Language);
            _elasticContext.Index(indexName, searchModel);
        }

        public void Update(Documents.Search searchModel)
        {
            string indexName = GetIndexName(searchModel.SearchType, searchModel.Language);
            _elasticContext.BulkIndex<Documents.Search>(indexName, new List<Documents.Search>
            {
                searchModel
            });
        }

        /// <summary>
        /// Index adı oluşturur
        /// </summary>
        /// <param name="searchType">Arama tipi, kategori yada oyuncu</param>
        /// <param name="language">Hangi dilde aranacağı</param>
        /// <returns>Index name</returns>
        private string GetIndexName(SearchTypes searchType, Languages? language)
        {
            string indexName = $"{ElasticSearchIndexName}_{searchType.ToString()}";
            if (language != null)
            {
                indexName += $"_{language.ToString()}";
            }

            return indexName.ToLowerInvariant();
        }

        /// <summary>
        /// user id ait search modeli getirir
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Documents.Search SearchById(string id, SearchTypes searchType, Languages? language)
        {
            string indexName = GetIndexName(searchType, language);
            var elasticSearchEngine = new ElasticSearchBuilder(indexName, _elasticContext)
                    .SetSize(1)
                    .SetFrom(0)
                    .AddTermQuery<Documents.Search>("_id", id)
                    .Build()
                    .Execute<Documents.Search>();

            return elasticSearchEngine.FirstOrDefault();
        }

        /// <summary>
        /// Kullanıcının takip ettiği kategorileri getirir
        /// </summary>
        /// <param name="searchText">Arann kategori</param>
        /// <param name="userId">Kullanıcı d</param>
        /// <param name="language">Dil</param>
        /// <param name="pagingModel">Sayfalama</param>
        /// <returns>Takip edilen kategoriler</returns>
        public async Task<ServiceModel<SearchModel>> GetFollowedSubCategories(string searchText, string userId, Languages language, PagingModel pagingModel)
        {
            var serviceModel = new ServiceModel<SearchModel>
            {
                PageNumber = pagingModel.PageNumber,
                PageSize = pagingModel.PageSize
            };

            string[] followedSubCategories = _followSubCategoryRepository.FollowedSubCategoryIds(userId);
            if (followedSubCategories.Length == 0)
            {
                return serviceModel;
            }

            string indexName = GetIndexName(SearchTypes.Category, language);

            IEnumerable<Documents.Search> searchResponse = null;
            if (string.IsNullOrEmpty(searchText))
            {
                searchResponse = new ElasticSearchBuilder(indexName, _elasticContext)
                                                                          .SetSize(pagingModel.PageSize)
                                                                          .SetFrom(pagingModel.PageNumber - 1)
                                                                          .AddTermsQuery<Documents.Search>("_id", followedSubCategories)
                                                                          .Build()
                                                                          .Execute<Documents.Search>();
            }
            else
            {
                // Elasticsearch Suggest yaparken filtre koyamadığımız için burada gelen auto complate içinden sadece takip ettiği kategorileri filtreliyoruz
                searchResponse = (await _elasticContext.SearchAsync<Documents.Search>(indexName, searchText))
                                                                    .Where(search => followedSubCategories.Contains(search.SubCategoryId));
            }

            if (searchResponse != null && searchResponse.Count() > 0)
            {
                // Filtrenmiş hali ile kaç tane kaldığı sayısını ekledik
                serviceModel.Count = searchResponse.Count();

                // Paing için skip take yaptık
                searchResponse = searchResponse.Skip(pagingModel.PageSize * (pagingModel.PageNumber - 1))
                                               .Take(pagingModel.PageSize)
                                               .AsEnumerable();

                serviceModel.Items = _mapper.Map<List<SearchModel>>(searchResponse);
            }

            return serviceModel;
        }

        #endregion Methods
    }
}