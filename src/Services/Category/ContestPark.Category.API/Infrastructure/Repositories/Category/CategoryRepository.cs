using ContestPark.Category.API.Infrastructure.Repositories.OpenCategory;
using ContestPark.Category.API.Model;
using ContestPark.Core.CosmosDb.Interfaces;
using ContestPark.Core.CosmosDb.Models;
using ContestPark.Core.Enums;
using ContestPark.Core.Models;
using Microsoft.Azure.Documents;
using System.Linq;

namespace ContestPark.Category.API.Infrastructure.Repositories.Category
{
    public class CategoryRepository : ICategoryRepository
    {
        #region Private

        private readonly IOpenCategoryRepository _openCategoryRepository;

        #endregion Private

        #region Constructor

        public CategoryRepository(IDocumentDbRepository<Documents.Category> repository,
            IOpenCategoryRepository openCategoryRepository)
        {
            Repository = repository;
            _openCategoryRepository = openCategoryRepository;
        }

        #endregion Constructor

        #region Properties

        public IDocumentDbRepository<Documents.Category> Repository { get; private set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Kategorileri ve alt kategorileri getirir
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <param name="language">Dil</param>
        /// <param name="pagingModel">Sayfalama</param>
        /// <returns>Kategori ve alt kategorileri döndürür</returns>
        public ServiceModel<CategoryModel> GetCategories(string userId, Languages language, PagingModel pagingModel)
        {
            string[] openSubCategories = _openCategoryRepository.OpenSubCategoryIds(userId);

            string sql = @"SELECT DISTINCT
                           cl.categoryName AS categoryName,
                           c.id as categoryId,
                           ARRAY(SELECT DISTINCT VALUE {
                                subCategoryId: sc.id,
                                displayPrice: ARRAY_CONTAINS(@openCategories,  sc.id, true) ? '' : sc.displayPrice,
                                picturePath:  ARRAY_CONTAINS(@openCategories,  sc.id, true) ? sc.picturePath : @defaultImages,
                                price: ARRAY_CONTAINS(@openCategories,  sc.id, true) ? 0 : sc.price,
                                displayOrder: sc.displayOrder,
                                subCategoryName: scl.subCategoryName
                             }   FROM sc IN c.subCategories JOIN scl IN sc.subCategoryLangs WHERE sc.visibility=true AND scl.languageId=@languageId ) as subCategories
                           FROM c
                           JOIN cl IN c.categoryLangs
                           WHERE c.visibility=true AND cl.languageId=@languageId
                           ORDER BY c.displayOrder";

            var categories = Repository.Query<CategoryModel>(new SqlQuerySpec
            {
                QueryText = sql,
                Parameters = new SqlParameterCollection
                 {
                     new SqlParameter("@openCategories", openSubCategories),
                     new SqlParameter("@defaultImages", DefaultImages.DefaultLock),
                     new SqlParameter("@languageId", language.ToString()),
                     //new SqlParameter("@pageSize", pagingModel.PageSize),
                     //new SqlParameter("@pageNumber", QueryableExtension.PageNumberCalculate(pagingModel))
                 }
            }).ToList();

            return new ServiceModel<CategoryModel>
            {
                Items = categories,
                PageNumber = pagingModel.PageNumber,
                PageSize = pagingModel.PageSize,
                Count = categories.Count()
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

            return Repository.UpdateAsync(doc).Result;
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

            return Repository.UpdateAsync(doc).Result;
        }

        private Documents.Category GetCategoryBySubCategoryId(string subCategoryId)
        {
            return Repository.Query<Documents.Category>(new SqlQuerySpec
            {
                QueryText = @"SELECT DISTINCT VALUE  c FROM c
                              JOIN sc IN c.subCategories
                              WHERE sc.id=@subCategoryId",
                Parameters = new SqlParameterCollection
                {
                    new SqlParameter("@subCategoryId", subCategoryId)
                }
            }).AsEnumerable().SingleOrDefault();
        }

        /// <summary>
        /// Alt kategori ücretsiz mi kontrol eder
        /// </summary>
        /// <param name="subCategoryId">Alt kategori id</param>
        /// <returns>Kategori ücretsiz ise true değilse false</returns>
        public bool IsSubCategoryFree(string subCategoryId)
        {
            return Repository.Query<bool>(new SqlQuerySpec
            {
                QueryText = @"SELECT DISTINCT VALUE sc.price=0 FROM c
                              JOIN sc IN c.subCategories
                              WHERE sc.id=@subCategoryId",
                Parameters = new SqlParameterCollection
                {
                    new SqlParameter("@subCategoryId", subCategoryId)
                }
            }).ToList().FirstOrDefault();
        }

        #endregion Methods
    }
}