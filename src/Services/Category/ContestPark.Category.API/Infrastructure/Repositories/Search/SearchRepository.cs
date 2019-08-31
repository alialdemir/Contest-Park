using AutoMapper;
using ContestPark.Category.API.Extensions;
using ContestPark.Category.API.Infrastructure.ElasticSearch;
using ContestPark.Category.API.Infrastructure.ElasticSearch.BusinessEngines;
using ContestPark.Category.API.Infrastructure.Repositories.FollowSubCategory;
using ContestPark.Category.API.Infrastructure.Repositories.OpenSubCategory;
using ContestPark.Category.API.Model;
using ContestPark.Core.Database.Models;
using ContestPark.Core.Enums;
using ContestPark.Core.Models;
using Microsoft.Extensions.Configuration;
using System;
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
        private readonly IOpenCategoryRepository _openCategoryRepository;
        private readonly IMapper _mapper;

        #endregion Private Variables

        #region Constructor

        public SearchRepository(IElasticContext elasticContext,
                                IFollowSubCategoryRepository followSubCategoryRepository,
                                IConfiguration configuration,
                                IOpenCategoryRepository openCategoryRepository,
                                IMapper mapper)
        {
            ElasticSearchIndexName = configuration["ElasticSearchIndexName"];
            _elasticContext = elasticContext;
            _followSubCategoryRepository = followSubCategoryRepository;
            _openCategoryRepository = openCategoryRepository;
            _mapper = mapper;
        }

        #endregion Constructor

        #region Methods

        public void CreateSearchIndexs()
        {
            string indexNameCategoryTurkish = GetIndexName(SearchTypes.Category, Languages.English);
            string indexNameEnglish = GetIndexName(SearchTypes.Category, Languages.Turkish);
            string indexNamePlayer = GetIndexName(SearchTypes.Player, null);
            string aliasName = ElasticSearchIndexName;

            _elasticContext.CreateIndex<Tables.Search>(indexNameCategoryTurkish, aliasName);
            _elasticContext.CreateIndex<Tables.Search>(indexNameEnglish, aliasName);
            _elasticContext.CreateIndex<Tables.Search>(indexNamePlayer, aliasName);
        }

        public void DeleteSearchIndexs()
        {
            string indexNameCategoryTurkish = GetIndexName(SearchTypes.Category, Languages.English);
            string indexNameEnglish = GetIndexName(SearchTypes.Category, Languages.Turkish);
            string indexNamePlayer = GetIndexName(SearchTypes.Player, null);

            _elasticContext.DeleteIndex(indexNameCategoryTurkish);
            _elasticContext.DeleteIndex(indexNameEnglish);
            _elasticContext.DeleteIndex(indexNamePlayer);
        }

        public void Insert(Tables.Search searchModel)
        {
            string indexName = GetIndexName(searchModel.SearchType, searchModel.Language);
            _elasticContext.Index(indexName, searchModel);
        }

        public void Update(Tables.Search searchModel)
        {
            string indexName = GetIndexName(searchModel.SearchType, searchModel.Language);
            _elasticContext.BulkIndex<Tables.Search>(indexName, new List<Tables.Search>
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
        public Tables.Search SearchById(string id, SearchTypes searchType, Languages? language)
        {
            string indexName = GetIndexName(searchType, language);
            var elasticSearchEngine = new ElasticSearchBuilder(indexName, _elasticContext)
                    .SetSize(1)
                    .SetFrom(0)
                    .AddTermQuery("_id", id)
                    .Build()
                    .Execute<Tables.Search>();

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
        public async Task<ServiceModel<SearchModel>> SearchFollowedSubCategoriesAsync(string searchText, string userId, Languages language, PagingModel pagingModel)
        {
            var serviceModel = new ServiceModel<SearchModel>
            {
                PageNumber = pagingModel.PageNumber,
                PageSize = pagingModel.PageSize
            };

            IEnumerable<short> followedSubCategories = _followSubCategoryRepository.FollowedSubCategoryIds(userId);
            if (followedSubCategories == null || followedSubCategories.Count() == 0)
            {
                return serviceModel;
            }

            var searchFollowedCategories = await DynamicSearchAsync(searchText, language, userId, pagingModel, SearchFilters.SubCategoryId, followedSubCategories.ToArray());

            searchFollowedCategories.Items = serviceModel// takip ettiği kkategorilerde fiyat sıfır(0) ve gösterilecek fiyat sıfır(0) olmalı
                .Items
                .Select(sc => new SearchModel
                {
                    CategoryName = sc.CategoryName,
                    Price = 0,
                    DisplayPrice = "0",
                    FullName = sc.FullName,
                    IsFollowing = sc.IsFollowing,
                    PicturePath = sc.PicturePath,
                    SearchType = sc.SearchType,
                    SubCategoryId = sc.SubCategoryId,
                    SubCategoryName = sc.SubCategoryName,
                    UserId = sc.UserId,
                    UserName = sc.UserName,
                })
                .ToList();

            return searchFollowedCategories;
        }

        /// <summary>
        /// searchText bilgisine göre arama yapar
        /// eğer searchText boş ise ilgili aramadaki tüm kayıtları sayfalayarak döndürür
        /// eğer filterPropertyName değeri subCategoryId ise alt kategori id göre filtreler
        /// </summary>
        /// <param name="searchText">Aranan kelime</param>
        /// <param name="language">Hangi dilde geriye döneceği</param>
        /// <param name="pagingModel">Sayfalama</param>
        /// <param name="searchFilters">Dönen data neye göre filtrelenecek kategor id göre mi yoksa alt kategori id göre mi</param>
        /// <param name="filterIds">Filtrelenmek istenen idler</param>
        /// <returns>Aranan veri listesi</returns>
        public async Task<ServiceModel<SearchModel>> DynamicSearchAsync(string searchText, Languages language, string userId, PagingModel pagingModel, SearchFilters searchFilters, params short[] filterIds)
        {
            var serviceModel = new ServiceModel<SearchModel>
            {
                PageNumber = pagingModel.PageNumber,
                PageSize = pagingModel.PageSize
            };

            string indexName = GetIndexName(SearchTypes.Category, language);

            bool isFilterIdNull = filterIds != null && filterIds[0] != 0;
            if (!isFilterIdNull)// eğer filterIds boş ise tüm kategoriler ve oyuncular üzerinde arama yapmalı o yüzden player indexinide ekledik
            {
                indexName += "," + GetIndexName(SearchTypes.Player, null);
            }

            IEnumerable<Tables.Search> searchResponse = null;

            if (string.IsNullOrEmpty(searchText))
            {
                var elasticSearchBuilder = new ElasticSearchBuilder(indexName, _elasticContext)
                                                                            .SetSize(pagingModel.PageSize)
                                                                            .SetFrom(pagingModel.PageSize * (pagingModel.PageNumber - 1));
                if (isFilterIdNull)
                {
                    elasticSearchBuilder.AddFilter<Tables.Search>(searchFilters.GetDisplayName(), filterIds.Select(x => x.ToString()).ToArray());
                }

                searchResponse = elasticSearchBuilder.Build()
                                                    .Execute<Tables.Search>();
            }
            else
            {
                // Elasticsearch Suggest yaparken filtre koyamadığımız için burada gelen auto complate içinden sadece takip ettiği kategorileri filtreliyoruz
                searchResponse = (await _elasticContext.SearchAsync<Tables.Search>(indexName, searchText, pagingModel))
                                                                    .Where(search =>
                                                                    // eğer filterIds yoksa tüm alt kategorileri listelesin
                                                                    !isFilterIdNull
                                                                    ||
                                                                    // kategori aramadan geliyorsa CategoryId göre filtreledik alt kategori arama yapıyorsa SubCategoryId göre filtreledik
                                                                    filterIds.Contains(searchFilters == SearchFilters.SubCategoryId ? search.SubCategoryId : search.CategoryId)
                                                                    ).ToList();
            }

            if (searchResponse != null && searchResponse.Count() > 0)
            {
                // Filtrenmiş hali ile kaç tane kaldığı sayısını ekledik
                //    serviceModel.HasNextPage = searchResponse.HasNextPage(searchResponse.Count(), pagingModel);

                serviceModel.Items = _mapper.Map<List<SearchModel>>(searchResponse);

                if (serviceModel.Items.Any(x => x.SearchType == SearchTypes.Category))
                {
                    List<short> openSubCategories = _openCategoryRepository.IsSubCategoryOpen(userId, serviceModel.Items.Select(x => x.SubCategoryId).AsEnumerable());

                    serviceModel.Items = serviceModel
                        .Items
                        .Select(sc => new SearchModel
                        {
                            CategoryName = sc.CategoryName,
                            DisplayPrice = openSubCategories.Any(x => x == sc.SubCategoryId) || sc.Price == 0 ? "0" : sc.DisplayPrice,
                            FullName = sc.FullName,
                            IsFollowing = sc.IsFollowing,
                            PicturePath = openSubCategories.Any(x => x == sc.SubCategoryId) || sc.Price == 0 ? sc.PicturePath : DefaultImages.DefaultLock,
                            Price = openSubCategories.Any(x => x == sc.SubCategoryId) || sc.Price == 0 ? 0 : sc.Price,
                            SearchType = sc.SearchType,
                            SubCategoryId = sc.SubCategoryId,
                            SubCategoryName = sc.SubCategoryName,
                            UserId = sc.UserId,
                            UserName = sc.UserName,
                        })
                        .ToList();
                }
            }

            return serviceModel;
        }

        #endregion Methods
    }
}
