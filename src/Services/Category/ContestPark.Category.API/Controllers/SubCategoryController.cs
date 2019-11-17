using ContestPark.Category.API.Infrastructure.Repositories.FollowSubCategory;
using ContestPark.Category.API.Infrastructure.Repositories.OpenSubCategory;
using ContestPark.Category.API.Infrastructure.Repositories.SubCategory;
using ContestPark.Category.API.Infrastructure.Tables;
using ContestPark.Category.API.IntegrationEvents.Events;
using ContestPark.Category.API.Model;
using ContestPark.Category.API.Resources;
using ContestPark.Category.API.Services.Balance;
using ContestPark.Core.Database.Models;
using ContestPark.Core.Enums;
using ContestPark.EventBus.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
        private readonly ISubCategoryRepository _categoryRepository;
        private readonly IEventBus _eventBus;
        private readonly IBalanceService _balanceService;

        #endregion Private Variables

        #region Constructor

        public SubCategoryController(IFollowSubCategoryRepository followSubCategoryRepository,
                                     IOpenCategoryRepository openCategoryRepository,
                                     ISubCategoryRepository categoryRepository,
                                     ILogger<SubCategoryController> logger,
                                     IBalanceService balanceService,
                                     IEventBus eventBus) : base(logger)
        {
            _followSubCategoryRepository = followSubCategoryRepository ?? throw new ArgumentNullException(nameof(followSubCategoryRepository));
            _openCategoryRepository = openCategoryRepository ?? throw new ArgumentNullException(nameof(openCategoryRepository));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _balanceService = balanceService ?? throw new ArgumentNullException(nameof(balanceService));
            _eventBus = eventBus;
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
            ServiceModel<CategoryModel> result = new ServiceModel<CategoryModel>()
            {
                PageNumber = pagingModel.PageNumber,
                PageSize = pagingModel.PageSize,
                Items = new List<CategoryModel>()
            };
            List<CategoryModel> categoryList = new List<CategoryModel>();

            ServiceModel<SubCategoryModel> followedSubCategories = _categoryRepository.GetFollowedSubCategories(UserId,
                                                                                                               CurrentUserLanguage,
                                                                                                               pagingModel);
            if (followedSubCategories != null && followedSubCategories.Items != null && followedSubCategories.Items.Count() > 0)
            {
                CategoryModel categoryModel = new CategoryModel()
                {
                    CategoryId = -1,
                    CategoryName = CategoryResource.FollowedCategories
                };

                categoryModel.SubCategories.AddRange(followedSubCategories.Items);

                categoryList.Add(categoryModel);

                result.HasNextPage = followedSubCategories.HasNextPage;
            }

            ServiceModel<CategoryModel> categories = _categoryRepository.GetCategories(UserId,
                                                                                      CurrentUserLanguage,
                                                                                      pagingModel);

            categoryList.AddRange(categories.Items);

            result.HasNextPage = categories.HasNextPage;

            result.Items = categoryList;

            if (result == null || result.Items == null || result.Items.Count() == 0)
            {
                Logger.LogCritical($"{nameof(categories)} list returned empty.");

                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Takip ettiğin alt kategoriler
        /// </summary>
        /// <returns>Tüm kategorileri döndürür.</returns>
        ////////[HttpGet("Followed")]
        ////////[ProducesResponseType(typeof(ServiceModel<SubCategoryModel>), (int)HttpStatusCode.OK)]
        ////////[ProducesResponseType((int)HttpStatusCode.BadRequest)]
        ////////public IActionResult FollowedSubCategories([FromQuery]PagingModel pagingModel) kategoriler içerisinde takip ettiği kategorilerde döndürürdü
        ////////{
        ////////    ServiceModel<SubCategoryModel> followedSubCategories = _categoryRepository.GetFollowedSubCategories(UserId,
        ////////                                                                                                        CurrentUserLanguage,
        ////////                                                                                                        pagingModel);

        ////////    if (followedSubCategories == null || followedSubCategories.Items == null || followedSubCategories.Items.Count() == 0)
        ////////    {
        ////////        Logger.LogInformation($"{nameof(followedSubCategories)} list returned empty.");

        ////////        return NotFound();
        ////////    }

        ////////    return Ok(followedSubCategories);
        ////////}

        /// <summary>
        /// Alt kategori takip et
        /// </summary>
        /// <param name="subCategoryId">Alt kategori Id</param>
        [HttpPost("{subCategoryId}/Follow")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Post([FromRoute]short subCategoryId)
        {
            if (subCategoryId < 0 || string.IsNullOrEmpty(UserId))
                return BadRequest();

            if (_followSubCategoryRepository.IsSubCategoryFollowed(UserId, subCategoryId))// Kategoriyi daha önceden takip etmişmi
                return BadRequest(CategoryResource.YouAreAlreadyFollowingThisCategory);

            if (!(
                _categoryRepository.IsSubCategoryFree(subCategoryId) ||// kategori ücretsiz değil ise
                _openCategoryRepository.IsSubCategoryOpen(UserId, subCategoryId)// veya kategorinin kilidi açık değilse
                ))
            {
                return BadRequest(CategoryResource.ToBeAbleToFollowThisCategoryYouNeedToUnlockIt);
            }

            bool isSuccess = await _followSubCategoryRepository.FollowSubCategoryAsync(UserId, subCategoryId);

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
        public async Task<IActionResult> Delete([FromRoute]short subCategoryId)
        {
            if (subCategoryId < 0 || string.IsNullOrEmpty(UserId))
                return BadRequest();

            if (!_followSubCategoryRepository.IsSubCategoryFollowed(UserId, subCategoryId))
                return BadRequest(CategoryResource.YouMustFollowThisCategoryToDeactivateTheCategory);

            bool isSuccess = await _followSubCategoryRepository.UnFollowSubCategoryAsync(UserId, subCategoryId);
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
        public IActionResult Get([FromRoute]short subCategoryId)
        {
            if (subCategoryId < 0 || string.IsNullOrEmpty(UserId))
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
        [ProducesResponseType(typeof(SubCategoryDetailInfoModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult GetDetail([FromRoute]short subCategoryId)
        {
            if (subCategoryId < 0)
                return BadRequest();

            SubCategoryDetailInfoModel subCategoryDetail = _categoryRepository.GetSubCategoryDetail(subCategoryId, CurrentUserLanguage);
            if (subCategoryDetail == null)
                return NotFound();

            return Ok(subCategoryDetail);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("{subCategoryId}/Info")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult GetSubCategoryInfo([FromRoute]short subCategoryId, [FromQuery]Languages language)
        {
            if (subCategoryId < 0)
                return BadRequest();

            SubCategoryDetailInfoModel subCategoryDetail = _categoryRepository.GetSubCategoryDetail(subCategoryId, language);
            if (subCategoryDetail == null)
                return NotFound();

            return Ok(new
            {
                subCategoryDetail.SubCategoryName,
                SubCategoryPicturePath = subCategoryDetail.PicturePath
            });
        }

        /// <summary>
        /// Alt kategori kilit açma
        /// </summary>
        /// <param name="subCategoryId">Alt kategori Id</param>
        [HttpPost("{subCategoryId}/unlock")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UnLockSubCategory([FromRoute]short subCategoryId, [FromQuery]BalanceTypes balanceType = BalanceTypes.Gold)
        {
            Logger.LogInformation("Alt kategori kilit açma isteği geldi. {subCategoryId} {balanceType} {userId}",
                                  subCategoryId,
                                  balanceType,
                                  UserId);

            if (subCategoryId < 0)
                return BadRequest();

            decimal subCategoryPrice = _categoryRepository.GetSubCategoryPrice(subCategoryId);
            if (subCategoryPrice == 0)// subCategoryPrice sıfır ise ücretsizdir kilidini açmaya gerek yok
                return BadRequest(CategoryResource.YouCanNotUnlockTheFreeCategory);

            bool isSubCategoryOpen = _openCategoryRepository.IsSubCategoryOpen(UserId, subCategoryId);
            if (isSubCategoryOpen)
                return BadRequest(CategoryResource.ThisCategoryIsAlreadyUnlocked);

            // Kullanıcı bakiye bilgisi alındı
            BalanceModel balance = await _balanceService.GetBalance(UserId, balanceType);
            if (balance == null)
                return BadRequest(CategoryResource.AnErrorHasOccurredFromTheSystemPleaseTryAgain);

            if (balance.Amount < subCategoryPrice)
                return BadRequest(CategoryResource.YourBalanceIsInsufficient, ErrorStatuCodes.YouCanNotUnlockTheFreeCategory);

            bool isSuccess = await _openCategoryRepository.UnLockSubCategory(new OpenSubCategory
            {
                UserId = UserId,
                SubCategoryId = subCategoryId
            });

            if (!isSuccess)
                return BadRequest(CategoryResource.CouldNotPpenSubcategoryLock);

            // bakiyesinden alt kategori fiyatı kadar altın/para düşme eventi yollandı
            var @event = new ChangeBalanceIntegrationEvent(-subCategoryPrice,
                                                           UserId,
                                                           balanceType,
                                                           BalanceHistoryTypes.UnLockSubCategory);
            _eventBus.Publish(@event);

            return Ok();
        }

        #endregion Services
    }
}
