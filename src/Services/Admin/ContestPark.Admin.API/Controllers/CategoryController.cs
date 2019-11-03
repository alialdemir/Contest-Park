using ContestPark.Admin.API.Infrastructure.Repositories.Category;
using ContestPark.Admin.API.Model.Category;
using ContestPark.Core.Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Net;

namespace ContestPark.Admin.API.Controllers
{
    public class CategoryController : Core.Controllers.ControllerBase
    {
        #region Private Variables

        private readonly ICategoryRepository _categoryRepository;

        #endregion Private Variables

        #region Constructor

        public CategoryController(ILogger<CategoryController> logger,
                                  ICategoryRepository categoryRepository) : base(logger)
        {
            _categoryRepository = categoryRepository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Kategori güncelleme objesi verir
        /// </summary>
        /// <param name="categoryId">Kategori id</param>
        [HttpGet]
        [ProducesResponseType(typeof(ServiceModel<CategoryModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult GetCategories([FromQuery]PagingModel paging)// Oyunucunun karşısına rakip ekler
        {
            var subCategories = _categoryRepository.GetCategories(CurrentUserLanguage, paging);
            if (subCategories == null || subCategories.Items.Count() == 0)
                return NotFound();

            return Ok(subCategories);
        }

        /// <summary>
        /// Kategori güncelleme objesi verir
        /// </summary>
        /// <param name="categoryId">Kategori id</param>
        [HttpGet("{categoryId}")]
        [ProducesResponseType(typeof(CategoryUpdateModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult GetCategory([FromRoute]short categoryId)// Oyunucunun karşısına rakip ekler
        {
            var category = _categoryRepository.GetCategoryById(categoryId);
            if (category == null)
                return NotFound();

            return Ok(category);
        }

        #endregion Methods
    }
}
