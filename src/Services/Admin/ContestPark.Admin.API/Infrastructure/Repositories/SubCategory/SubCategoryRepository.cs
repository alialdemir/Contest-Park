using ContestPark.Admin.API.Model.SubCategory;
using ContestPark.Core.Database.Interfaces;
using ContestPark.Core.Database.Models;
using ContestPark.Core.Enums;
using ContestPark.Core.Services.NumberFormat;
using System;
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

            string sqlTemplate = @"UPDATE SubCategoryLangs
                                            SET
                                            SubCategoryName = '@SubCategoryName',  ModifiedDate = CURRENT_TIMESTAMP(), Description = '@Description'
                                            WHERE SubCategoryId = @SubCategoryId AND LANGUAGE=@LANGUAGE;";

            string sql = "";

            foreach (var categoryLocalized in subCategoryUpdate.LocalizedModels)// Tek seferde güncelleme yapsın diye bu şekilde yaprım
            {
                sql += sqlTemplate
                    .Replace("@LANGUAGE", ((byte)categoryLocalized.Language).ToString())
                    .Replace("@Description", categoryLocalized.Description)
                    .Replace("@SubCategoryName", categoryLocalized.Text);
            }

            return await _subCategoryLocalizedRepository.ExecuteAsync(sql, new
            {
                subCategoryUpdate.SubCategoryId,
            });
        }

        /// <summary>
        /// Alt kategori ekle
        /// </summary>
        /// <param name="subCategoryInsert">Alt kategori bilgisi</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public async Task<bool> InsertAsync(SubCategoryInsertModel subCategoryInsert)
        {
            int? subCategoryId = await _subCategoryRepository.AddAsync(new Tables.SubCategory
            {
                DisplayOrder = subCategoryInsert.DisplayOrder,
                Price = subCategoryInsert.Price,
                PicturePath = subCategoryInsert.PicturePath,
                DisplayPrice = _numberFormatService.NumberFormating((long)subCategoryInsert.Price),
            });

            if (!subCategoryId.HasValue)
                return false;

            await _subCategoryOfCategoryRepository.AddRangeAsync(subCategoryInsert.CategoryIds.Select(categoryId => new Tables.SubCategoryOfCategory
            {
                SubCategoryId = Convert.ToInt16(subCategoryId.Value),
                CategoryId = categoryId,
            }));

            return await _subCategoryLocalizedRepository.AddRangeAsync(subCategoryInsert.LocalizedModels.Select(x => new Tables.SubCategoryLocalized
            {
                Description = x.Description,
                Language = x.Language,
                SubCategoryId = Convert.ToInt16(subCategoryId.Value),
                SubCategoryName = x.Text,
            }));
        }

        /// <summary>
        /// Tüm alt kategorileri isim ve id listesi seçilen dile göre getirir
        /// </summary>
        /// <param name="language">Dil</param>
        /// <param name="paging">Sayfalama</param>
        /// <returns>Alt kategori dropdown list</returns>
        public ServiceModel<SubCategoryDropdownModel> GetSubCategories(Languages language, PagingModel paging)
        {
            string sql = @"SELECT scl.SubCategoryName, scl.SubCategoryId
                           FROM SubCategoryLangs scl
                           WHERE scl.`Language`=@language";

            return _subCategoryRepository.ToServiceModel<SubCategoryDropdownModel>(sql, new
            {
                language
            }, pagingModel: paging);
        }

        #endregion Methods
    }
}
