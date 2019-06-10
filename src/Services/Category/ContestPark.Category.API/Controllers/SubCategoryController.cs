using ContestPark.Category.API.Infrastructure.Documents;
using ContestPark.Category.API.Infrastructure.Repositories.Category;
using ContestPark.Category.API.Infrastructure.Repositories.FollowSubCategory;
using ContestPark.Category.API.Infrastructure.Repositories.OpenCategory;
using ContestPark.Category.API.Infrastructure.Repositories.Search;
using ContestPark.Category.API.Model;
using ContestPark.Category.API.Resources;
using ContestPark.Core.CosmosDb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
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
        /// <param name="pagingModel">Hem kategorileri hemde alt kategorileri sayfalar</param>
        [HttpGet]
        [ProducesResponseType(typeof(ServiceModel<CategoryModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult Get([FromQuery]PagingModel pagingModel)
        {
            ServiceModel<CategoryModel> catgories = _categoryRepository.GetCategories(UserId,
                                                                                      CurrentUserLanguage,
                                                                                      pagingModel);

            if (catgories == null || catgories.Items == null || catgories.Items.Count() == 0)
            {
                Logger.LogCritical($"{nameof(catgories)} list returned empty.");

                return NotFound();
            }

            return Ok(catgories);
        }

        /// <summary>
        /// Takip ettiğin alt kategoriler
        /// </summary>
        /// <returns>Tüm kategorileri döndürür.</returns>
        [HttpGet("Followed")]
        [ProducesResponseType(typeof(ServiceModel<SubCategoryModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult FollowedSubCategories([FromQuery]PagingModel pagingModel)
        {
            ServiceModel<SubCategoryModel> followedSubCategories = _categoryRepository.GetFollowedSubCategories(UserId,
                                                                                                                CurrentUserLanguage,
                                                                                                                pagingModel);

            if (followedSubCategories == null || followedSubCategories.Items == null || followedSubCategories.Items.Count() == 0)
            {
                Logger.LogInformation($"{nameof(followedSubCategories)} list returned empty.");

                return NotFound();
            }

            return Ok(followedSubCategories);
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

            bool isSuccess = await _followSubCategoryRepository.AddAsync(new FollowSubCategory
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
        [ProducesResponseType((int)HttpStatusCode.OK)]
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

        /// <summary>
        /// Kategorinin detayını döndürür
        /// </summary>
        /// <param name="subCategoryId">Alt kategori Id</param>
        [HttpGet]
        [Route("{subCategoryId}")]
        [ProducesResponseType(typeof(SubCategoryDetailModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult GetDetail(string subCategoryId)
        {
            if (string.IsNullOrEmpty(subCategoryId) || string.IsNullOrEmpty(UserId))
                return BadRequest();

            SubCategoryDetailInfoModel subCategoryDetail = _categoryRepository.GetSubCategoryDetail(subCategoryId, CurrentUserLanguage);
            if (subCategoryDetail == null)
                return NotFound();

            return Ok(new SubCategoryDetailModel
            {
                Level = 1, // TODO: level bilgisi çekilmeli

                // TODO: user id için login olmuş olması gerekir sadece bunun için identity serverden token kontrol eder sadece kategori takip etme durumunu ayri bir entpoint e alırsak bu entpointi AllowAnonymous yapıp performans kazanılabilir
                IsSubCategoryFollowUpStatus = _followSubCategoryRepository.IsSubCategoryFollowed(UserId, subCategoryId),

                CategoryFollowersCount = subCategoryDetail.CategoryFollowersCount,
                Description = subCategoryDetail.Description,
                SubCategoryId = subCategoryDetail.SubCategoryId,
                SubCategoryName = subCategoryDetail.SubCategoryName,
                SubCategoryPicturePath = subCategoryDetail.SubCategoryPicturePath
            });
        }

        /// <summary>
        /// Alt kategori kilit açma
        /// </summary>
        /// <param name="subCategoryId">Alt kategori Id</param>
        [HttpPost]
        [Route("{subCategoryId}/unlock")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UnLockSubCategory(string subCategoryId)
        {
            if (string.IsNullOrEmpty(subCategoryId) || string.IsNullOrEmpty(UserId))
                return BadRequest();

            bool isSubCategoryOpen = _openCategoryRepository.IsSubCategoryOpen(UserId, subCategoryId);
            if (isSubCategoryOpen)
                return BadRequest(CategoryResource.ThisCategoryIsAlreadyUnlocked);

            // TODO: check altın miktarı yeterli mi

            bool isSuccess = await _openCategoryRepository.AddAsync(new OpenSubCategory
            {
                UserId = UserId,
                SubCategoryId = subCategoryId
            });

            if (!isSuccess)
                return BadRequest(CategoryResource.CouldNotPpenSubcategoryLock);

            return Ok();
        }

        #endregion Services
    }
}