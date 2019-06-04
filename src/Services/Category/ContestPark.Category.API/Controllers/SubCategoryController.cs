using ContestPark.Category.API.Infrastructure.Repositories.Category;
using ContestPark.Category.API.Infrastructure.Repositories.FollowSubCategory;
using ContestPark.Category.API.Infrastructure.Repositories.OpenCategory;
using ContestPark.Category.API.Model;
using ContestPark.Category.API.Resources;
using ContestPark.Core.CosmosDb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ContestPark.Category.API.Controllers
{
    public class SubCategoryController : Core.Controllers.ControllerBase
    {
        #region Private Variables

        private readonly IFollowSubCategoryRepository _followSubCategoryRepository;
        private readonly IOpenCategoryRepository _openCategoryRepository;
        private readonly ICategoryRepository _categoryRepository;

        #endregion Private Variables

        #region Constructor

        public SubCategoryController(IFollowSubCategoryRepository followSubCategoryRepository,
                                           IOpenCategoryRepository openCategoryRepository,
                                           ICategoryRepository categoryRepository,
                                           ILogger<SubCategoryController> logger) : base(logger)
        {
            _followSubCategoryRepository = followSubCategoryRepository ?? throw new ArgumentNullException(nameof(followSubCategoryRepository));
            _openCategoryRepository = openCategoryRepository ?? throw new ArgumentNullException(nameof(openCategoryRepository));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        }

        #endregion Constructor

        #region Services

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

        /// <summary>
        /// Alt kategori takip et
        /// </summary>
        /// <param name="subCategoryId">Alt kategori Id</param>
        [HttpPost]
        [Route("{subCategoryId}/Follow")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Post(string subCategoryId)
        {
            if (string.IsNullOrEmpty(subCategoryId) || string.IsNullOrEmpty(UserId))
                return BadRequest();

            if (_followSubCategoryRepository.IsSubCategoryFollowed(UserId, subCategoryId))// Kategoriyi daha önceden takip etmişmi
                return BadRequest(CategoryResource.YouAreAlreadyFollowingThisCategory);

            if (!(_categoryRepository.IsSubCategoryFree(subCategoryId) ||// kategori ücretsiz değil ise
                _openCategoryRepository.IsSubCategoryOpen(UserId, subCategoryId)))// veya kategorinin kilidi açık değilse
                return BadRequest(CategoryResource.ToBeAbleToFollowThisCategoryYouNeedToUnlockIt);

            bool isSuccess = await _followSubCategoryRepository.Repository.InsertAsync(new Infrastructure.Documents.FollowSubCategory
            {
                SubCategoryId = subCategoryId,
                UserId = UserId
            });

            if (!isSuccess)
            {
                Logger.LogError($"Alt kategori takip etme sırasında hata oluştu. sub Category Id: {subCategoryId} user id: {UserId}");

                return BadRequest();
            }

            isSuccess = _categoryRepository.IncreasingFollowersCount(subCategoryId);
            if (!isSuccess)
            {
                Logger.LogError($"Alt kategori takipçi sayısı artırılamadı. Alt kategori id {subCategoryId}");
            }

            return Ok();
        }

        /// <summary>
        /// Alt kategori takipten çıkar
        /// </summary>
        /// <param name="subCategoryId">Alt kategori Id</param>
        [HttpDelete]
        [Route("{subCategoryId}/UnFollow")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Delete(string subCategoryId)
        {
            if (string.IsNullOrEmpty(subCategoryId) || string.IsNullOrEmpty(UserId))
                return BadRequest();

            bool isSuccess = await _followSubCategoryRepository.DeleteAsync(UserId, subCategoryId);
            if (!isSuccess)
            {
                Logger.LogError($"Alt kategori takip etme sırasında hata oluştu. sub Category Id: {subCategoryId} user id: {UserId}");

                return BadRequest();
            }

            isSuccess = _categoryRepository.DecreasingFollowersCount(subCategoryId);
            if (!isSuccess)
            {
                Logger.LogError($"Alt kategori takipçi sayısı azaltılamadı. Alt kategori id {subCategoryId}");
            }

            return Ok();
        }

        /// <summary>
        /// Alt kategori takip etme durumu
        /// true ise takip ediyor false ise takip etmiyor
        /// </summary>
        /// <param name="subCategoryId">Alt kategori Id</param>
        [HttpGet]
        [Route("{subCategoryId}/FollowStatus")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult Get(string subCategoryId)
        {
            if (string.IsNullOrEmpty(subCategoryId) || string.IsNullOrEmpty(UserId))
                return BadRequest();

            return Ok(new
            {
                IsSubCategoryFollowed = _followSubCategoryRepository.IsSubCategoryFollowed(UserId, subCategoryId)
            });
        }

        #endregion Services
    }
}