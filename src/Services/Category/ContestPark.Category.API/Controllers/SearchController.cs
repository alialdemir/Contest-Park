using ContestPark.Category.API.Infrastructure.Repositories.Search;
using ContestPark.Category.API.Model;
using ContestPark.Core.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ContestPark.Category.API.Controllers
{
    public class SearchController : Core.Controllers.ControllerBase
    {
        #region Private Variables

        private readonly ISearchRepository _searchRepository;

        #endregion Private Variables

        #region Constructor

        public SearchController(ISearchRepository searchRepository,
                                ILogger<SubCategoryController> logger) : base(logger)
        {
            _searchRepository = searchRepository ?? throw new ArgumentNullException(nameof(searchRepository));
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

            if (followedSearchSubCategories == null || followedSearchSubCategories.Items == null || followedSearchSubCategories.Items.Count() == 0)
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
        [AllowAnonymous]
        [ProducesResponseType(typeof(ServiceModel<SearchModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SearchAsync([FromQuery]PagingModel pagingModel, [FromQuery]short categoryId, [FromQuery(Name = "q")]string searchText = "")
        {
            if (searchText == "'")
                searchText = string.Empty;

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
