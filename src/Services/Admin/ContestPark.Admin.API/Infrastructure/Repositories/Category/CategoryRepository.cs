using ContestPark.Admin.API.Model;
using ContestPark.Admin.API.Model.Category;
using ContestPark.Core.Database.Interfaces;
using ContestPark.Core.Database.Models;
using ContestPark.Core.Enums;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ContestPark.Admin.API.Infrastructure.Repositories.Category
{
    public class CategoryRepository : ICategoryRepository
    {
        #region Private Variables

        private readonly IQueryRepository _queryRepository;

        #endregion Private Variables

        #region Constructor

        public CategoryRepository(IQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Seçilen dile göre tüm kategorileri döndürür
        /// </summary>
        /// <param name="language">Dil</param>
        /// <param name="paging">Sayfalama</param>
        /// <returns>Kategoriler</returns>
        public ServiceModel<CategoryModel> GetCategories(Languages language, PagingModel paging)
        {
            string sql = @"SELECT
                           c.CategoryId,
                           cl.TEXT,
                           c.Visibility,
                           c.DisplayOrder,
                           c.ModifiedDate,
                           c.CreatedDate
                           FROM Categories c
                           INNER JOIN CategoryLocalizeds cl ON cl.CategoryId = c.CategoryId
                           WHERE cl.`Language` = @language";

            return _queryRepository.ToServiceModel<CategoryModel>(sql, new
            {
                language
            }, pagingModel: paging);
        }

        /// <summary>
        /// Kategori güncelleme objesi verir
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="pagingModel"></param>
        /// <returns>Kategori</returns>
        public CategoryUpdateModel GetCategoryById(short categoryId)
        {
            string sql = @"SELECT
                          c.CategoryId,
                          cl.Text,
                          c.Visibility,
                          c.DisplayOrder,
                          cl.`Language`
                          FROM Categories c
                          INNER JOIN CategoryLocalizeds cl ON cl.CategoryId = c.CategoryId
                          WHERE c.CategoryId = @categoryId";

            var param = new
            {
                categoryId
            };

            var result = new CategoryUpdateModel();

            var data = _queryRepository.SpQuery<CategoryUpdateModel, LocalizedModel, CategoryUpdateModel>(sql, (category, categoryLocalized) =>
            {
                if (result.CategoryId == 0)
                {
                    result.CategoryId = category.CategoryId;
                    result.DisplayOrder = category.DisplayOrder;
                    result.Visibility = category.Visibility;
                }

                if (result.LocalizedModels == null)
                    result.LocalizedModels = new List<LocalizedModel>();

                if (!result.LocalizedModels.Any(x => x.Language == categoryLocalized.Language))
                    result.LocalizedModels.Add(categoryLocalized);

                return result;
            }, param, splitOn: "CategoryId", CommandType.Text).FirstOrDefault();

            return result;
        }

        #endregion Methods
    }
}
