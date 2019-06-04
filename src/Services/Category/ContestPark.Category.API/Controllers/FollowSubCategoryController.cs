using ContestPark.Category.API.Infrastructure.Repositories.Category;
using ContestPark.Category.API.Infrastructure.Repositories.FollowSubCategory;
using ContestPark.Category.API.Infrastructure.Repositories.OpenCategory;
using ContestPark.Category.API.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ContestPark.Category.API.Controllers
{
    public class FollowSubCategoryController : Core.Controllers.ControllerBase
    {
        #region Private Variables

        private readonly IFollowSubCategoryRepository _followSubCategoryRepository;
        private readonly IOpenCategoryRepository _openCategoryRepository;
        private readonly ICategoryRepository _categoryRepository;

        #endregion Private Variables

        #region Constructor

        public FollowSubCategoryController(IFollowSubCategoryRepository followSubCategoryRepository,
                                           IOpenCategoryRepository openCategoryRepository,
                                           ICategoryRepository categoryRepository,
                                           ILogger<FollowSubCategoryController> logger) : base(logger)
        {
            _followSubCategoryRepository = followSubCategoryRepository ?? throw new ArgumentNullException(nameof(followSubCategoryRepository));
            _openCategoryRepository = openCategoryRepository ?? throw new ArgumentNullException(nameof(openCategoryRepository));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        }

        #endregion Constructor

        #region Services

        /// <summary>
        /// Alt kategori açma
        /// </summary>
        /// <param name="subCategoryId">Alt kategori Id</param>
        /// <returns>Alt kategori açma durumu</returns>
        [HttpPost]
        [Route("{subCategoryId}")]
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

        #endregion Services
    }
}