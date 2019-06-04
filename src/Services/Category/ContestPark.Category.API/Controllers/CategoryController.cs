using ContestPark.Category.API.Infrastructure.Repositories.Category;
using ContestPark.Category.API.Model;
using ContestPark.Core.CosmosDb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;

namespace ContestPark.Category.API.Controllers
{
    public class CategoryController : Core.Controllers.ControllerBase
    {
        #region Private variables

        private readonly ICategoryRepository _categoryRepository;

        #endregion Private variables

        #region Constructor

        public CategoryController(ILogger<CategoryController> logger,
                                  ICategoryRepository categoryRepository) : base(logger)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Kategorileri listeleme
        /// </summary>
        /// <returns>Tüm kategorileri döndürür.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ServiceModel<CategoryModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult Get([FromQuery]PagingModel pagingModel)
        {
            ServiceModel<CategoryModel> catgories = _categoryRepository.GetCategories(UserId, CurrentUserLanguage, pagingModel);
            if (catgories == null)
            {
                Logger.LogCritical($"{nameof(catgories)} list returned empty.", catgories);
                return NotFound();
            }

            return Ok(catgories);
        }

        #endregion Methods
    }
}