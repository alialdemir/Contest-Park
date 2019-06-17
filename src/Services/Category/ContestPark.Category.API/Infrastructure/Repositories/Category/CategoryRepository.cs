using ContestPark.Category.API.Infrastructure.Repositories.FollowSubCategory;
using ContestPark.Category.API.Infrastructure.Repositories.OpenCategory;
using ContestPark.Category.API.Model;
using ContestPark.Core.CosmosDb.Extensions;
using ContestPark.Core.CosmosDb.Interfaces;
using ContestPark.Core.CosmosDb.Models;
using ContestPark.Core.Enums;
using ContestPark.Core.Models;
using System.Linq;

namespace ContestPark.Category.API.Infrastructure.Repositories.Category
{
    public partial class CategoryRepository : ICategoryRepository
    {
        #region Private Variables

        private readonly IOpenCategoryRepository _openCategoryRepository;
        private readonly IFollowSubCategoryRepository _followSubCategoryRepository;

        private readonly IDocumentDbRepository<Documents.Category> _categoryRepository;

        #endregion Private Variables

        #region Constructor

        public CategoryRepository(IDocumentDbRepository<Documents.Category> repository,
                                  IFollowSubCategoryRepository followSubCategoryRepository,
                                  IOpenCategoryRepository openCategoryRepository)
        {
            _categoryRepository = repository;
            _followSubCategoryRepository = followSubCategoryRepository;
            _openCategoryRepository = openCategoryRepository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Kategorileri ve alt kategorileri getirir
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <param name="language">Dil</param>
        /// <param name="pagingModel">Hem kategorileri hemde alt kategorileri sayfalar</param>
        /// <returns>Kategori ve alt kategorileri döndürür</returns>
        public ServiceModel<CategoryModel> GetCategories(string userId, Languages language, PagingModel pagingModel)
        {
            string[] openSubCategories = _openCategoryRepository.OpenSubCategoryIds(userId);

            string sql = @"SELECT DISTINCT
                           cl.CategoryName AS categoryName,
                           c.id as categoryId,
                           ARRAY(SELECT DISTINCT VALUE {
                                subCategoryId: sc.id,
                                displayPrice: ARRAY_CONTAINS(@openSubCategories,  sc.id, true) ? '' : sc.DisplayPrice,
                                picturePath:  ARRAY_CONTAINS(@openSubCategories,  sc.id, true) ? sc.PicturePath : @defaultLock,
                                price: ARRAY_CONTAINS(@openSubCategories,  sc.id, true) ? 0 : sc.Price,
                                displayOrder: sc.DisplayOrder,
                                subCategoryName: scl.SubCategoryName
                             }   FROM sc IN c.SubCategories JOIN scl IN sc.SubCategoryLangs WHERE sc.Visibility=true AND scl.LanguageId=@language) as subCategories
                           FROM c
                           JOIN cl IN c.CategoryLangs
                           WHERE c.Visibility=true AND cl.LanguageId=@language
                           ORDER BY c.DisplayOrder";

            var categories = _categoryRepository.QueryMultipleAsync<CategoryModel>(sql, new
            {
                openSubCategories,
                defaultLock = DefaultImages.DefaultLock,
                language
            });

            // TODO: Tam bu noktada kullanıcı id göre cache alınmalı bir sonraki geldiğinde cachdeki data üzerinden sayfalama yapmalı

            categories.ToList().ForEach(c =>
            {
                c.SubCategories = c.SubCategories.Skip(pagingModel.PageSize * (pagingModel.PageNumber - 1)).Take(pagingModel.PageSize).ToList();
            });

            return new ServiceModel<CategoryModel>
            {
                Items = categories.Skip(pagingModel.PageSize * (pagingModel.PageNumber - 1)).Take(pagingModel.PageSize),// cosmos db queryde offset limit kullanmama izin vermediği için buradan filtreledik
                PageNumber = pagingModel.PageNumber,
                PageSize = pagingModel.PageSize,
                HasNextPage = (pagingModel.PageSize * (pagingModel.Offset + 1)) < categories.Count()
            };
        }

        /// <summary>
        /// Kategori takipçi sayısını bir artıır
        /// </summary>
        /// <param name="subCategoryId">Alt kategori id</param>
        /// <returns>İşlem başarılı ise true değilse false</returns>
        public bool IncreasingFollowersCount(string subCategoryId)
        {
            Documents.Category doc = GetCategoryBySubCategoryId(subCategoryId);

            if (doc == null)
                return false;

            doc.SubCategories.Where(x => x.Id == subCategoryId).FirstOrDefault().FollowerCount += 1;

            return _categoryRepository.UpdateAsync(doc).Result;
        }

        /// <summary>
        /// Kategori takipçi sayısını bir azaltır
        /// </summary>
        /// <param name="subCategoryId">Alt kategori id</param>
        /// <returns>İşlem başarılı ise true değilse false</returns>
        public bool DecreasingFollowersCount(string subCategoryId)
        {
            Documents.Category doc = GetCategoryBySubCategoryId(subCategoryId);

            if (doc == null || doc.SubCategories.Where(x => x.Id == subCategoryId).FirstOrDefault().FollowerCount == 0)
                return false;

            doc.SubCategories.Where(x => x.Id == subCategoryId).FirstOrDefault().FollowerCount -= 1;

            return _categoryRepository.UpdateAsync(doc).Result;
        }

        private Documents.Category GetCategoryBySubCategoryId(string subCategoryId)
        {
            string sql = @"SELECT DISTINCT VALUE c FROM c
                          JOIN sc IN c.SubCategories
                          WHERE sc.id = @subCategoryId";

            return _categoryRepository.QuerySingleAsync(sql, new { subCategoryId });
        }

        /// <summary>
        /// Alt kategori ücretsiz mi kontrol eder
        /// </summary>
        /// <param name="subCategoryId">Alt kategori id</param>
        /// <returns>Kategori ücretsiz ise true değilse false</returns>
        public bool IsSubCategoryFree(string subCategoryId)
        {
            string sql = @"SELECT DISTINCT VALUE sc.Price=0 FROM c
                           JOIN sc IN c.SubCategories
                           WHERE sc.id=@subCategoryId";

            return _categoryRepository.QuerySingleAsync<bool>(sql, new { subCategoryId });
        }

        /// <summary>
        /// Kullanıcının takip ettiği kategoriler
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <param name="language">Dil</param>
        /// <param name="pagingModel">Sayfalama</param>
        /// <returns>Kullanıcının takip ettiği kategoriler</returns>
        public ServiceModel<SubCategoryModel> GetFollowedSubCategories(string userId, Languages language, PagingModel pagingModel)
        {
            ServiceModel<SubCategoryModel> serviceModel = new ServiceModel<SubCategoryModel>
            {
                PageNumber = pagingModel.PageNumber,
                PageSize = pagingModel.PageSize,
            };

            string[] followedSubCategories = _followSubCategoryRepository
                                                            .FollowedSubCategoryIds(userId)
                                                            .ToPaging(pagingModel)
                                                            .ToArray();
            if (followedSubCategories.Length == 0)
            {
                return serviceModel;
            }

            string sql = @"SELECT DISTINCT
                           ARRAY(SELECT DISTINCT VALUE {
                                subCategoryId: sc.id,
                                picturePath: sc.PicturePath,
                                price: 0,
                                displayPrice: '',
                                displayOrder: sc.DisplayOrder,
                                subCategoryName: scl.SubCategoryName
                             }  FROM sc IN c.SubCategories JOIN scl IN sc.SubCategoryLangs WHERE sc.Visibility=true AND scl.LanguageId=@languageId AND ARRAY_CONTAINS(@followedSubCategories,  sc.id, true))
                             AS  SubCategories
                           FROM c
                           JOIN cl IN c.CategoryLangs
                           WHERE c.Visibility=true AND cl.LanguageId=@languageId
                           ORDER BY c.DisplayOrder";

            // NOTE: queryden datayı alabilmek için FollowedSubCategoryModel model oluşturup onun içinden alt kategorileri alabildim

            serviceModel.Items = _categoryRepository.QuerySingleAsync<FollowedSubCategoryModel>(sql, new
            {
                followedSubCategories,
                languageId = (byte)language
            })?.SubCategories;

            return serviceModel;
        }

        /// <summary>
        /// Alt kategori detay bilgilerini verir
        /// </summary>
        /// <param name="subCategoryId"></param>
        /// <returns>Alt kategori detay</returns>
        public SubCategoryDetailInfoModel GetSubCategoryDetail(string subCategoryId, Languages language)
        {
            string sql = @"SELECT
                           sc.FollowerCount AS categoryFollowersCount,
                           sc.Description,
                           sc.id AS subCategoryId,
                           scl.SubCategoryName,
                           sc.PicturePath AS SubCategoryPicturePath
                           FROM c
                           JOIN sc IN c.SubCategories
                           JOIN scl IN sc.SubCategoryLangs
                           WHERE c.Visibility=true AND sc.Visibility=true And sc.id = @subCategoryId AND scl.LanguageId=@language";

            return _categoryRepository.QuerySingleAsync<SubCategoryDetailInfoModel>(sql, new
            {
                subCategoryId,
                language = (byte)language
            });
        }

        /// <summary>
        /// Alt kategorinin fiyatını getirir
        /// </summary>
        /// <param name="subCategoryId">Alt kategori id</param>
        /// <returns>Fiyat</returns>
        public int GetSubCategoryPrice(string subCategoryId)
        {
            string sql = @"SELECT value sc.Price
                           FROM c JOIN sc IN c.SubCategories
                           where sc.id=@subCategoryId";

            return _categoryRepository.QuerySingleAsync<int>(sql, new { subCategoryId });
        }

        #endregion Methods
    }
}