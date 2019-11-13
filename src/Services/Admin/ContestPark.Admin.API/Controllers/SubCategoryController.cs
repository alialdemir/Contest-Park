using ContestPark.Admin.API.Infrastructure.Repositories.SubCategory;
using ContestPark.Admin.API.Model.Category;
using ContestPark.Admin.API.Model.SubCategory;
using ContestPark.Admin.API.Services.Picture;
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
    public class SubCategoryController : Core.Controllers.ControllerBase
    {
        #region Private Variables

        private readonly ISubCategoryRepository _subCategoryRepository;
        private readonly IFileUploadService _fileUploadService;

        #endregion Private Variables

        #region Constructor

        /// <summary>
        ///
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="subCategoryRepository"></param>
        /// <param name="fileUploadService"></param>
        public SubCategoryController(ILogger<SubCategoryController> logger,
                                     ISubCategoryRepository subCategoryRepository,
                                     IFileUploadService fileUploadService) : base(logger)
        {
            _subCategoryRepository = subCategoryRepository;
            _fileUploadService = fileUploadService;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Kategori güncelleme objesi verir
        /// </summary>
        /// <param name="subCategoryId">Kategori id</param>
        [HttpGet("{subCategoryId}")]
        [ProducesResponseType(typeof(SubCategoryUpdateModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult GetCategory([FromRoute]short subCategoryId)
        {
            var category = _subCategoryRepository.GetSubCategoryById(subCategoryId);
            if (category == null)
                return NotFound();

            return Ok(category);
        }

        /// <summary>
        /// Kategori listesi verir
        /// </summary>
        /// <param name="categoryId">Kategori id</param>
        [HttpGet]
        [ProducesResponseType(typeof(ServiceModel<SubCategoryModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult GetCategories([FromQuery]PagingModel paging)
        {
            var subCategories = _subCategoryRepository.GetSubCategories(CurrentUserLanguage, paging);
            if (subCategories == null || subCategories.Items.Count() == 0)
                return NotFound();

            return Ok(subCategories);
        }

        /// <summary>
        /// Kategori güncelle
        /// </summary>
        /// <param name="subCategoryUpdate">Kategori bilgisi</param>
        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateSubCategoryAsync([FromBody]SubCategoryUpdateModel subCategoryUpdate)
        {
            // TODO: Elasticsearch evemt publish
            // TODO: alt kategori resmini s3 kayıt edilecek
            bool isSuccess = await _subCategoryRepository.UpdateAsync(subCategoryUpdate);
            if (!isSuccess)
                return BadRequest();

            return Ok();
        }

        /// <summary>
        /// Alt kategori ekle
        /// </summary>
        /// <param name="subCategoryInsert">Alt kategori bilgisi</param>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> InsertSubCategoryAsync([FromBody]SubCategoryInsertModel subCategoryInsert)
        {
            // TODO: Elasticsearch evemt publish
            // TODO: alt kategori resmini s3 kayıt edilecek
            bool isSuccess = await _subCategoryRepository.InsertAsync(subCategoryInsert);
            if (!isSuccess)
                return BadRequest();

            return Ok();
        }

        /// <summary>
        /// Alt kategori adını ve alt kategori id listesi döndürür
        /// </summary>
        /// <param name="standbyModeModel">Bekleme modu bilgileri</param>
        [HttpGet("Dropdown")]
        [ProducesResponseType(typeof(ServiceModel<CategoryDropdownModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult GetSubCategoryDropList([FromQuery]PagingModel paging)
        {
            var subCategories = _subCategoryRepository.GetSubCategoryDropList(CurrentUserLanguage, paging);
            if (subCategories == null || subCategories.Items.Count() == 0)
                return NotFound();

            return Ok(subCategories);
        }

        #endregion Methods
    }
}
