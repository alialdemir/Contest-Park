using ContestPark.Category.API.Infrastructure.Repositories.FollowSubCategory;
using ContestPark.Category.API.Infrastructure.Repositories.OpenSubCategory;
using ContestPark.Category.API.Infrastructure.Repositories.SubCategory;
using ContestPark.Category.API.Infrastructure.Repositories.UserLevel;
using ContestPark.Category.API.IntegrationEvents.Events;
using ContestPark.Category.API.Models;
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
        private readonly IUserLevelRepository _userLevelRepository;

        #endregion Private Variables

        #region Constructor

        public SubCategoryController(IFollowSubCategoryRepository followSubCategoryRepository,
                                     IOpenCategoryRepository openCategoryRepository,
                                     ISubCategoryRepository categoryRepository,
                                     ILogger<SubCategoryController> logger,
                                     IBalanceService balanceService,
                                     IUserLevelRepository userLevelRepository,
                                     IEventBus eventBus) : base(logger)
        {
            _followSubCategoryRepository = followSubCategoryRepository ?? throw new ArgumentNullException(nameof(followSubCategoryRepository));
            _openCategoryRepository = openCategoryRepository ?? throw new ArgumentNullException(nameof(openCategoryRepository));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _balanceService = balanceService ?? throw new ArgumentNullException(nameof(balanceService));
            _userLevelRepository = userLevelRepository;
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
        [AllowAnonymous]
        [ProducesResponseType(typeof(ServiceModel<CategoryModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult Get([FromQuery] PagingModel pagingModel)
        {
            ServiceModel<CategoryModel> result = new ServiceModel<CategoryModel>()
            {
                PageNumber = pagingModel.PageNumber,
                PageSize = pagingModel.PageSize,
                Items = new List<CategoryModel>()
            };
            List<CategoryModel> categoryList = new List<CategoryModel>();

            bool isAllOpen = string.IsNullOrEmpty(UserId);// Eğer login olmadan istek gelirse user id boş olacaktır o zaman tüm kategorileri açık olarak döndürdük

            if (!isAllOpen)
            {
                #region Takip ettiğim kategoriler

                ServiceModel<SubCategoryModel> followedSubCategories = _categoryRepository.GetFollowedSubCategories(UserId,
                                                                                                               CurrentUserLanguage,
                                                                                                               pagingModel);
                if (followedSubCategories != null && followedSubCategories.Items != null && followedSubCategories.Items.Any())
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

                #endregion Takip ettiğim kategoriler

                #region En son oynadıklarım

                IEnumerable<SubCategoryModel> lastCategoriesPlayed = _categoryRepository.LastCategoriesPlayed(UserId, CurrentUserLanguage, pagingModel);
                if (lastCategoriesPlayed != null && lastCategoriesPlayed.Any())
                {
                    CategoryModel categoryModel = new CategoryModel()
                    {
                        CategoryId = -2,
                        CategoryName = CategoryResource.TheLastCategoriesIPlayed
                    };

                    categoryModel.SubCategories.AddRange(lastCategoriesPlayed.OrderBy(x => x.Price));

                    categoryList.Add(categoryModel);
                }

                #endregion En son oynadıklarım

                #region Önerilen kategoriler

                IEnumerable<SubCategoryModel> recommendedSubcategories = _categoryRepository.RecommendedSubcategories(UserId, CurrentUserLanguage);
                if (recommendedSubcategories != null && recommendedSubcategories.Any())
                {
                    CategoryModel categoryModel = new CategoryModel()
                    {
                        CategoryId = -3,
                        CategoryName = CategoryResource.RecommendedSubcategories
                    };

                    categoryModel.SubCategories.AddRange(recommendedSubcategories.OrderBy(x => x.Price));

                    categoryList.Add(categoryModel);
                }

                #endregion Önerilen kategoriler
            }

            #region Tüm kategoriler

            ServiceModel<CategoryModel> categories = _categoryRepository.GetCategories(UserId,
                                                                                       CurrentUserLanguage,
                                                                                       pagingModel,
                                                                                       isAllOpen);

            categoryList.AddRange(categories.Items);

            result.HasNextPage = categories.HasNextPage;

            #endregion Tüm kategoriler

            result.Items = categoryList;

            if (result == null || result.Items == null || result.Items.Count() == 0)
            {
                Logger.LogCritical($"{nameof(categories)} list returned empty.");

                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Alt kategori takip et
        /// </summary>
        /// <param name="subCategoryId">Alt kategori Id</param>
        [HttpPost("{subCategoryId}/Follow")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Post([FromRoute] short subCategoryId)
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
        public async Task<IActionResult> Delete([FromRoute] short subCategoryId)
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
        public IActionResult Get([FromRoute] short subCategoryId)
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
        public IActionResult GetDetail([FromRoute] short subCategoryId)
        {
            if (subCategoryId < 0)
                return BadRequest();

            SubCategoryDetailInfoModel subCategoryDetail = _categoryRepository.GetSubCategoryDetail(subCategoryId, CurrentUserLanguage, UserId);
            if (subCategoryDetail == null)
                return NotFound();

            return Ok(subCategoryDetail);
        }

        /// <summary>
        /// Kategorinin detayını döndürür
        /// </summary>
        /// <param name="subCategoryId">Alt kategori Id</param>
        /// <param name="language">Dil</param>
        /// <param name="userId">Kullanıcı id</param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("{subCategoryId}/Info")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult GetSubCategoryInfo([FromRoute] short subCategoryId, [FromQuery] Languages language, [FromQuery] string userId)
        {
            if (subCategoryId < 0)
                return BadRequest();

            SubCategoryDetailInfoModel subCategoryDetail = _categoryRepository.GetSubCategoryDetail(subCategoryId, language, userId);
            if (subCategoryDetail == null)
                return NotFound();

            return Ok(new
            {
                subCategoryDetail.SubCategoryName,
                SubCategoryPicturePath = subCategoryDetail.PicturePath,
                subCategoryDetail.IsSubCategoryOpen
            });
        }

        /// <summary>
        /// Alt kategori kilit açma
        /// </summary>
        /// <param name="subCategoryId">Alt kategori Id</param>
        [HttpPost("{subCategoryId}/unlock")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UnLockSubCategory([FromRoute] short subCategoryId, [FromQuery] BalanceTypes balanceType = BalanceTypes.Gold)
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

            bool isSuccess = await _openCategoryRepository.UnLockSubCategory(UserId, subCategoryId);

            if (!isSuccess)
                return BadRequest(CategoryResource.CouldNotPpenSubcategoryLock);

            Logger.LogInformation("Alt kategori kilidi açılldı. {subCategoryId} {balanceType} {userId} şuanki bakiye: {amount}",
                             subCategoryId,
                             balanceType,
                             UserId,
                             balance.Amount);

            // bakiyesinden alt kategori fiyatı kadar altın/para düşme eventi yollandı
            var @event = new ChangeBalanceIntegrationEvent(-subCategoryPrice,
                                                           UserId,
                                                           balanceType,
                                                           BalanceHistoryTypes.UnLockSubCategory);
            _eventBus.Publish(@event);

            return Ok();
        }

        /// <summary>
        /// Kullanıcı idlerinin ilgili kategorideki leveli
        /// </summary>
        /// <param name="subCategoryId">Alt kategori id</param>
        /// <param name="userIds">Kullanıcı id</param>
        /// <returns>Kullanıcının o alt kategorideki leveli</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("{subCategoryId}/UserLevel")]
        [ProducesResponseType(typeof(IEnumerable<UserLevelModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult UserLevel([FromRoute] short subCategoryId, [FromBody] List<string> userIds)
        {
            if (subCategoryId < 0 || userIds == null || !userIds.Any())
                return BadRequest();

            List<UserLevelModel> userLevels = new List<UserLevelModel>();

            foreach (string userId in userIds)
            {
                userLevels.Add(new UserLevelModel
                {
                    UserId = userId,
                    Level = _userLevelRepository.GetUserLevel(userId, subCategoryId)
                });
            }

            userLevels
                .ForEach(x =>// Eğer user id'leri içinde bot varsa botun levelini gerçek kullanıcının levelini veriyoruz çünkü oyuncu ile botun leveli aynı olsun
                {
                    if (userLevels.Any(x => x.UserId.EndsWith("-bot")) && x.UserId.EndsWith("-bot"))
                    {
                        UserLevelModel userLevel = userLevels.FirstOrDefault(x => !x.UserId.EndsWith("-bot"));
                        if (userLevel != null)
                        {
                            x.Level = userLevel.Level;
                        }
                    }
                });

            return Ok(userLevels);
        }

        #endregion Services
    }
}