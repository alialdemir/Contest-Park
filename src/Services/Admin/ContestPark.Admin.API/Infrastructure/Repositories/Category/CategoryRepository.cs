using ContestPark.Admin.API.Model;
using ContestPark.Admin.API.Model.Category;
using ContestPark.Core.Database.Interfaces;
using ContestPark.Core.Database.Models;
using ContestPark.Core.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Admin.API.Infrastructure.Repositories.Category
{
    public class CategoryRepository : ICategoryRepository
    {
        #region Private Variables

        private readonly IRepository<Tables.Category> _categoryRepository;
        private readonly IRepository<Tables.CategoryLocalized> _categoryLocalizedRepository;

        #endregion Private Variables

        #region Constructor

        public CategoryRepository(IRepository<Tables.Category> categoryRepository,
                                  IRepository<Tables.CategoryLocalized> categoryLocalizedRepository)
        {
            _categoryRepository = categoryRepository;
            _categoryLocalizedRepository = categoryLocalizedRepository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Yeni kategori ekle
        /// </summary>
        /// <param name="categoryInsert">Kategori bilgisi</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public async Task<bool> InsertAsync(CategoryInsertModel categoryInsert)
        {
            int? categoryId = await _categoryRepository.AddAsync(new Tables.Category
            {
                DisplayOrder = categoryInsert.DisplayOrder,
            });

            if (!categoryId.HasValue)
                return false;

            return await _categoryLocalizedRepository.AddRangeAsync(categoryInsert.LocalizedModels.Select(x => new Tables.CategoryLocalized
            {
                CategoryId = Convert.ToInt16(categoryId.Value),
                Language = x.Language,
                Text = x.Text
            }));
        }

        /// <summary>
        /// Kategori güncelle
        /// </summary>
        /// <param name="categoryUpdate">Kategori bilgisi</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public async Task<bool> UpdateAsync(CategoryUpdateModel categoryUpdate)
        {
            bool isSuccess = await _categoryRepository.UpdateAsync(new Tables.Category
            {
                CategoryId = categoryUpdate.CategoryId,
                Visibility = categoryUpdate.Visibility,
                DisplayOrder = categoryUpdate.DisplayOrder,
                ModifiedDate = DateTime.Now
            });

            if (!isSuccess)
                return false;

            string sqlTemplate = @"UPDATE CategoryLocalizeds
                                            SET
                                            TEXT = '@Text',  ModifiedDate = CURRENT_TIMESTAMP()
                                            WHERE CategoryId = @CategoryId AND LANGUAGE=@LANGUAGE;";

            string sql = "";

            foreach (var categoryLocalized in categoryUpdate.LocalizedModels)// Tek seferde güncelleme yapsın diye bu şekilde yaprım
            {
                sql += sqlTemplate
                    .Replace("@LANGUAGE", ((byte)categoryLocalized.Language).ToString())
                    .Replace("@Text", categoryLocalized.Text);
            }

            return await _categoryRepository.ExecuteAsync(sql, new
            {
                categoryUpdate.CategoryId,
            });
        }

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
                           cl.Text AS CategoryName,
                           c.Visibility,
                           c.DisplayOrder,
                           c.ModifiedDate,
                           c.CreatedDate
                           FROM Categories c
                           INNER JOIN CategoryLocalizeds cl ON cl.CategoryId = c.CategoryId
                           WHERE cl.`Language` = @language";

            return _categoryRepository.ToServiceModel<CategoryModel>(sql, new
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
                          cl.CategoryId as LocalizeCategoryId,
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

            var data = _categoryRepository.SpQuery<CategoryUpdateModel, LocalizedModel, CategoryUpdateModel>(sql, (category, categoryLocalized) =>
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
            }, param, splitOn: "CategoryId, LocalizeCategoryId", CommandType.Text).FirstOrDefault();

            return result;
        }

        #endregion Methods
    }
}
