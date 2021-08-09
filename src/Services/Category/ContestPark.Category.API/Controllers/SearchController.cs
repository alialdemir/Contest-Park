using ContestPark.Category.API.Infrastructure.Repositories.Search;
using ContestPark.Category.API.Infrastructure.Repositories.SubCategory;
using ContestPark.Category.API.Models;
using ContestPark.Category.API.Resources;
using ContestPark.Core.Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ContestPark.Category.API.Controllers
{
    public class SearchController : Core.Controllers.ControllerBase
    {
        #region Private Variables

        private readonly ISearchRepository _searchRepository;
        private readonly ISubCategoryRepository _subCategoryRepository;

        #endregion Private Variables

        #region Constructor

        public SearchController(ISearchRepository searchRepository,
                                ISubCategoryRepository subCategoryRepository,
                                ILogger<SubCategoryController> logger) : base(logger)
        {
            _searchRepository = searchRepository ?? throw new ArgumentNullException(nameof(searchRepository));
            _subCategoryRepository = subCategoryRepository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Takip ettiğin alt kategoriler
        /// </summary>
        /// <returns>Tüm kategorileri döndürür.</returns>
        [HttpGet("Followed")]
        [ProducesResponseType(typeof(ServiceModel<SearchModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SearchFollowedSubCategories([FromQuery]PagingModel pagingModel, [FromQuery(Name = "q")]string searchText = "")
        {
            if (searchText == "'")
                searchText = string.Empty;

            ServiceModel<SearchModel> followedSearchSubCategories = await _searchRepository.SearchFollowedSubCategoriesAsync(searchText,
                                                                                                                             UserId,
                                                                                                                             CurrentUserLanguage,
                                                                                                                             pagingModel);

            if (followedSearchSubCategories == null || followedSearchSubCategories.Items == null || !followedSearchSubCategories.Items.Any())
            {
                Logger.LogInformation($"{nameof(followedSearchSubCategories)} list returned empty.", searchText);

                return NotFound();
            }

            return Ok(followedSearchSubCategories);
        }

        /// <summary>
        /// Kategori arama
        /// </summary>
        /// <returns>Tüm kategorileri döndürür.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ServiceModel<SearchModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SearchAsync([FromQuery]PagingModel pagingModel, [FromQuery]short categoryId, [FromQuery(Name = "q")]string searchText = "")
        {
            if (searchText == "'")
                searchText = string.Empty;

            if (categoryId == -2)
            {
                #region En son oynadıklarım

                IEnumerable<SubCategoryModel> lastCategoriesPlayed = _subCategoryRepository.LastCategoriesPlayed(UserId, CurrentUserLanguage, pagingModel);
                if (lastCategoriesPlayed == null || !lastCategoriesPlayed.Any())
                    return NotFound();

                return Ok(new ServiceModel<SearchModel>
                {
                    Items = lastCategoriesPlayed
                                        .Where(x => searchText == "'" || x.SubCategoryName.Contains(searchText))
                                        .Select(x => new SearchModel
                                        {
                                            SubCategoryId = x.SubCategoryId,
                                            SubCategoryName = x.SubCategoryName,
                                            SearchType = SearchTypes.Category,
                                            IsSubCategoryOpen = x.IsSubCategoryOpen,
                                            DisplayPrice = x.DisplayPrice,
                                            PicturePath = x.PicturePath,
                                            Price = x.Price,
                                            CategoryName = CategoryResource.TheLastCategoriesIPlayed,
                                        })
                                        .ToList()
                });

                #endregion En son oynadıklarım
            }
            else if (categoryId == -3)
            {
                #region Önerilen kategoriler

                IEnumerable<SubCategoryModel> recommendedSubcategories = _subCategoryRepository.RecommendedSubcategories(UserId, CurrentUserLanguage);
                if (recommendedSubcategories == null || !recommendedSubcategories.Any())
                    return NotFound();

                return Ok(new ServiceModel<SearchModel>
                {
                    Items = recommendedSubcategories
                                        .Where(x => searchText == "'" || x.SubCategoryName.Contains(searchText))
                                        .Select(x => new SearchModel
                                        {
                                            SubCategoryId = x.SubCategoryId,
                                            SubCategoryName = x.SubCategoryName,
                                            IsSubCategoryOpen = x.IsSubCategoryOpen,
                                            SearchType = SearchTypes.Category,
                                            DisplayPrice = x.DisplayPrice,
                                            PicturePath = x.PicturePath,
                                            Price = x.Price,
                                            CategoryName = CategoryResource.RecommendedSubcategories,
                                        })
                                        .ToList()
                });

                #endregion Önerilen kategoriler
            }
            ServiceModel<SearchModel> searchCategories = await _searchRepository.DynamicSearchAsync(searchText,
                                                                                                    CurrentUserLanguage,
                                                                                                    UserId,
                                                                                                    pagingModel,
                                                                                                    SearchFilters.CategoryId,
                                                                                                    categoryId);

            if (searchCategories == null || searchCategories.Items == null || searchCategories.Items.Count() == 0)
            {
                Logger.LogInformation($"{nameof(searchCategories)} list returned empty.", categoryId, searchText);

                return NotFound();
            }

            return Ok(searchCategories);
        }

        #endregion Methods
    }
}
