﻿using ContestPark.Admin.API.Model;
using ContestPark.Admin.API.Model.SubCategory;
using ContestPark.Core.Database.Interfaces;
using ContestPark.Core.Database.Models;
using ContestPark.Core.Enums;
using ContestPark.Core.Services.NumberFormat;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Admin.API.Infrastructure.Repositories.SubCategory
{
    public class SubCategoryRepository : ISubCategoryRepository
    {
        #region Private Variables

        private readonly IRepository<Tables.SubCategory> _subCategoryRepository;
        private readonly IRepository<Tables.SubCategoryOfCategory> _subCategoryOfCategoryRepository;
        private readonly IRepository<Tables.SubCategoryLocalized> _subCategoryLocalizedRepository;
        private readonly INumberFormatService _numberFormatService;

        #endregion Private Variables

        #region Constructor

        public SubCategoryRepository(IRepository<Tables.SubCategory> subCategoryRepository,
                                     IRepository<Tables.SubCategoryOfCategory> subCategoryOfCategoryRepository,
                                     IRepository<Tables.SubCategoryLocalized> subCategoryLocalizedRepository,
                                     INumberFormatService numberFormatService)
        {
            _subCategoryRepository = subCategoryRepository;
            _subCategoryOfCategoryRepository = subCategoryOfCategoryRepository;
            _subCategoryLocalizedRepository = subCategoryLocalizedRepository;
            _numberFormatService = numberFormatService;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Kategori güncelleme objesi verir
        /// </summary>
        /// <param name="subCategoryId"></param>
        /// <param name="pagingModel"></param>
        /// <returns>Kategori</returns>
        public SubCategoryUpdateModel GetSubCategoryById(short subCategoryId)
        {
            string sql = @"SELECT
                           scl.SubCategoryName as Text,
                           scl.Language,
                           sc.Price,
                           sc.PicturePath,
                           scl.Description,
                           sc.Visibility,
                           sc.DisplayOrder
                           FROM SubCategories sc
                           INNER JOIN SubCategoryLangs scl ON scl.SubCategoryId = sc.SubCategoryId
                           WHERE sc.SubCategoryId = @subCategoryId";

            var param = new
            {
                subCategoryId
            };

            var data = _subCategoryRepository.QueryMultiple<SubCategoryUpdateModel>(sql, param, CommandType.Text);

            SubCategoryUpdateModel subCategory = data.FirstOrDefault();

            return new SubCategoryUpdateModel
            {
                CategoryIds = _subCategoryRepository.QueryMultiple<short>("SELECT scr.CategoryId FROM SubCategoryRls scr WHERE scr.SubCategoryId = @subCategoryId", param),
                DisplayOrder = subCategory.DisplayOrder,
                PicturePath = subCategory.PicturePath,
                Price = subCategory.Price,
                SubCategoryId = subCategoryId,
                Visibility = subCategory.Visibility,
                LocalizedModels = _subCategoryRepository.QueryMultiple<LocalizedModel>("SELECT scl.`Language`, scl.SubCategoryName AS TEXT, scl.Description FROM SubCategoryLangs scl WHERE scl.SubCategoryId = @subCategoryId", param)
            };
        }

        /// <summary>
        /// Seçilen dile göre tüm alt kategori döndürür
        /// </summary>
        /// <param name="language">Dil</param>
        /// <param name="paging">Sayfalama</param>
        /// <returns>Alt kategoriler</returns>
        public ServiceModel<SubCategoryModel> GetSubCategories(Languages language, PagingModel paging)
        {
            string sql = @"SELECT
                         sc.SubCategoryId,
                         scl.SubCategoryName,
                         sc.Visibility,
                         sc.DisplayOrder,
                         sc.DisplayPrice,
                         scl.ModifiedDate,
                         scl.CreatedDate,
                         (SELECT COUNT(*) FROM SubCategoryRls scr WHERE scr.SubCategoryId = sc.SubCategoryId) AS LinkedCategories
                         FROM SubCategories sc
                         INNER JOIN SubCategoryLangs scl ON scl.SubCategoryId = sc.SubCategoryId
                         WHERE scl.`Language` = @language
                         ORDER BY sc.CreatedDate DESC";

            return _subCategoryRepository.ToServiceModel<SubCategoryModel>(sql, new
            {
                language
            }, pagingModel: paging);
        }

        /// <summary>
        /// Alt kategori güncelle
        /// </summary>
        /// <param name="subCategoryUpdate">Alt kategori bilgileri</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public async Task<bool> UpdateAsync(SubCategoryUpdateModel subCategoryUpdate)
        {
            bool isSuccess = await _subCategoryRepository.UpdateAsync(new Tables.SubCategory
            {
                SubCategoryId = subCategoryUpdate.SubCategoryId,
                Visibility = subCategoryUpdate.Visibility,
                DisplayOrder = subCategoryUpdate.DisplayOrder,
                ModifiedDate = DateTime.Now,
                PicturePath = subCategoryUpdate.PicturePath,
                Price = subCategoryUpdate.Price,
                DisplayPrice = _numberFormatService.NumberFormating((long)subCategoryUpdate.Price),
            });

            if (!isSuccess)
                return false;

            await SubCategoryOfCategory(subCategoryUpdate.SubCategoryId, subCategoryUpdate.CategoryIds);

            return await UpdateSubCategoryLang(subCategoryUpdate.SubCategoryId, subCategoryUpdate.LocalizedModels);
        }

        /// <summary>
        /// Alt kategori localized bilgilerini günceller
        /// </summary>
        /// <param name="subCategoryId"></param>
        /// <param name="localizedModels"></param>
        /// <returns></returns>
        private async Task<bool> UpdateSubCategoryLang(short subCategoryId, IEnumerable<LocalizedModel> localizedModels)
        {
            string sqlTemplate = @"UPDATE SubCategoryLangs
                                   SET
                                   SubCategoryName = '@SubCategoryName',  ModifiedDate = CURRENT_TIMESTAMP(), Description = '@Description'
                                   WHERE SubCategoryId = @SubCategoryId AND LANGUAGE=@LANGUAGE;";

            string sql = "";

            foreach (var categoryLocalized in localizedModels)// Tek seferde güncelleme yapsın diye bu şekilde yaprım
            {
                sql += sqlTemplate
                    .Replace("@LANGUAGE", ((byte)categoryLocalized.Language).ToString())
                    .Replace("@Description", categoryLocalized.Description)
                    .Replace("@SubCategoryName", categoryLocalized.Text);
            }

            return await _subCategoryLocalizedRepository.ExecuteAsync(sql, new
            {
                subCategoryId
            });
        }

        /// <summary>
        /// Yeni listedeki kategori id'leri eklenenleri ekler silinenleri siler
        /// </summary>
        /// <param name="subCategoryId">Alt kategory Id</param>
        /// <returns></returns>
        private async Task SubCategoryOfCategory(short subCategoryId, IEnumerable<short> categoryIds)
        {
            string sql = @"SELECT scr.CategoryId FROM SubCategoryRls scr WHERE  scr.SubCategoryId = @subCategoryId";

            IEnumerable<short> subCategoryOfCategories = _subCategoryOfCategoryRepository.QueryMultiple<short>(sql, new
            {
                subCategoryId
            });

            // ekli olmayanlar eklendi
            List<short> addedCategoryIds = categoryIds.Where(x => !subCategoryOfCategories.Any(categoryId => categoryId == x)).ToList();
            await _subCategoryOfCategoryRepository.AddRangeAsync(addedCategoryIds.Select(categoryId => new Tables.SubCategoryOfCategory
            {
                CategoryId = categoryId,
                SubCategoryId = subCategoryId
            }).ToList());

            // Ekli olupda kaldırılanlar silindi
            List<short> deletedCategoryIds = subCategoryOfCategories.Where(x => !categoryIds.Any(categoryId => categoryId == x)).ToList();
            foreach (short categoryId in deletedCategoryIds)
            {
                await _subCategoryOfCategoryRepository.ExecuteAsync("DELETE FROM SubCategoryRls scr WHERE scr.SubCategoryId = @subCategoryId AND scr.CategoryId = @categoryId", new
                {
                    categoryId,
                    subCategoryId
                });
            }
        }

        /// <summary>
        /// Alt kategori ekle
        /// </summary>
        /// <param name="subCategoryInsert">Alt kategori bilgisi</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public async Task<short?> InsertAsync(SubCategoryInsertModel subCategoryInsert)
        {
            int? subCategoryId = await _subCategoryRepository.AddAsync(new Tables.SubCategory
            {
                DisplayOrder = subCategoryInsert.DisplayOrder,
                Price = subCategoryInsert.Price,
                PicturePath = subCategoryInsert.PicturePath,
                DisplayPrice = _numberFormatService.NumberFormating((long)subCategoryInsert.Price),
            });

            if (!subCategoryId.HasValue)
                return (short)subCategoryId;

            await _subCategoryOfCategoryRepository.AddRangeAsync(subCategoryInsert.CategoryIds.Select(categoryId => new Tables.SubCategoryOfCategory
            {
                SubCategoryId = Convert.ToInt16(subCategoryId.Value),
                CategoryId = categoryId,
            }));

            await _subCategoryLocalizedRepository.AddRangeAsync(subCategoryInsert.LocalizedModels.Select(x => new Tables.SubCategoryLocalized
            {
                Description = x.Description,
                Language = x.Language,
                SubCategoryId = Convert.ToInt16(subCategoryId.Value),
                SubCategoryName = x.Text,
            }));

            return (short)subCategoryId;
        }

        /// <summary>
        /// Tüm alt kategorileri isim ve id listesi seçilen dile göre getirir
        /// </summary>
        /// <param name="language">Dil</param>
        /// <param name="paging">Sayfalama</param>
        /// <returns>Alt kategori dropdown list</returns>
        public ServiceModel<SubCategoryDropdownModel> GetSubCategoryDropList(Languages language, PagingModel paging)
        {
            string sql = @"SELECT scl.SubCategoryName, scl.SubCategoryId
                           FROM SubCategoryLangs scl
                           WHERE scl.`Language`=@language
                           ORDER BY scl.CreatedDate DESC";

            return _subCategoryRepository.ToServiceModel<SubCategoryDropdownModel>(sql, new
            {
                language
            }, pagingModel: paging);
        }

        #endregion Methods
    }
}
