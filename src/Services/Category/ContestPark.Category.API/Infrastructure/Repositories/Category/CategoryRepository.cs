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

        #endregion Methods
    }
}