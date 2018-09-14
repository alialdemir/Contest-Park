using ContestPark.Core.Domain.Model;
using ContestPark.Domain.Category.Interfaces;
using ContestPark.Domain.Category.Model.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Orleans;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ContestPark.Category.API.Controller
{
    public class CategoryController : Core.Service.ControllerBase
    {
        #region Private variables

        private readonly ICategoryGrain _categoryGrain;

        #endregion Private variables

        #region Constructor

        public CategoryController(
            ILogger<CategoryController> logger,
            IClusterClient clusterClient
            ) : base(logger)
        {
            _categoryGrain = clusterClient.GetGrain<ICategoryGrain>(0);
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Kategorileri listeleme
        /// </summary>
        /// <returns>Tüm kategorileri döndürür.</returns>
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ServiceResponse<Domain.Category.Model.Response.Category>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get([FromQuery]Paging pagingModel)
        {
            var catgories = await _categoryGrain.GetCategoryList(UserId, CurrentUserLanguage, pagingModel);
            if (catgories == null || !catgories.Items.Any())
            {
                Logger.LogCritical($"{nameof(catgories)} list returned empty.", catgories);
                return NotFound();
            }

            return Ok(catgories);
        }

        /// <summary>
        /// Kategori Id'ye göre kategori listesi getirir 0 ise tüm kategorileri getirir
        /// </summary>
        /// <returns>Tüm kategorileri döndürür.</returns>
        [HttpGet]
        [Route("{categoryId}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ServiceResponse<SubCategorySearch>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get(Int16 categoryId, [FromQuery]Paging pagingModel)
        {
            var catgories = await _categoryGrain.CategorySearch(UserId, categoryId, CurrentUserLanguage, pagingModel);
            if (catgories == null || !catgories.Items.Any())
            {
                Logger.LogCritical($"{nameof(catgories)} list returned empty.", catgories);
                return NotFound();
            }

            return Ok(catgories);
        }

        #endregion Methods
    }
}