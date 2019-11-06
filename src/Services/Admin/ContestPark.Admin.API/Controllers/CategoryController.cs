using ContestPark.Admin.API.Infrastructure.Repositories.Category;
using ContestPark.Admin.API.Model.Category;
using ContestPark.Core.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ContestPark.Admin.API.Controllers
{
    [Authorize(Policy = "AdminPolicy")]
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

        #region Services

        /// <summary>
        /// Kategori listesi verir
        /// </summary>
        /// <param name="categoryId">Kategori id</param>
        [HttpGet]
        [ProducesResponseType(typeof(ServiceModel<CategoryModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult GetCategories([FromQuery]PagingModel paging)
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
        public IActionResult GetCategory([FromRoute]short categoryId)
        {
            var category = _categoryRepository.GetCategoryById(categoryId);
            if (category == null)
                return NotFound();

            return Ok(category);
        }

        /// <summary>
        /// Kategori güncelle
        /// </summary>
        /// <param name="categoryUpdate">Kategori bilgisi</param>
        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateCategoryAsync([FromBody]CategoryUpdateModel categoryUpdate)
        {
            // TODO: Elasticsearch evemt publish
            bool isSuccess = await _categoryRepository.UpdateAsync(categoryUpdate);
            if (!isSuccess)
                return BadRequest();

            return Ok();
        }

        /// <summary>
        /// Kategori ekle
        /// </summary>
        /// <param name="categoryInsert">Kategori bilgisi</param>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> InsertCategoryAsync([FromBody]CategoryInsertModel categoryInsert)
        {
            // TODO: Elasticsearch evemt publish
            bool isSuccess = await _categoryRepository.InsertAsync(categoryInsert);
            if (!isSuccess)
                return BadRequest();

            return Ok();
        }

        #endregion Services
    }
}
