using ContestPark.Category.API.Model;
using ContestPark.Core.Database.Interfaces;
using ContestPark.Core.Database.Models;
using ContestPark.Core.Enums;
using ContestPark.Core.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ContestPark.Category.API.Infrastructure.Repositories.SubCategory
{
    public class SubCategoryRepository : ISubCategoryRepository
    {
        #region Private Variables

        private readonly IRepository<Tables.SubCategory> _subCategoryRepository;

        #endregion Private Variables

        #region Constructor

        public SubCategoryRepository(IRepository<Tables.SubCategory> subCategoryRepository)
        {
            _subCategoryRepository = subCategoryRepository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Kategori takipçi sayısını bir azaltır
        /// </summary>
        /// <param name="subCategoryId">Alt kategori id</param>
        /// <returns>İşlem başarılı ise true değilse false</returns>
        public bool DecreasingFollowersCount(short subCategoryId)
        {
            return _subCategoryRepository.ExecuteAsync("SP_ChangeFollowersCount", new
            {
                subCategoryId,
                FollowerCount = -1
            }, CommandType.StoredProcedure).Result;
        }

        /// <summary>
        /// Kategori takipçi sayısını bir artıır
        /// </summary>
        /// <param name="subCategoryId">Alt kategori id</param>
        /// <returns>İşlem başarılı ise true değilse false</returns>
        public bool IncreasingFollowersCount(short subCategoryId)
        {
            return _subCategoryRepository.ExecuteAsync("SP_ChangeFollowersCount", new
            {
                subCategoryId,
                FollowerCount = 1
            }, CommandType.StoredProcedure).Result;
        }

        /// <summary>
        /// Kategorileri ve alt kategorileri getirir
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <param name="language">Dil</param>
        /// <param name="pagingModel">Hem kategorileri hemde alt kategorileri sayfalar</param>
        /// <returns>Kategori ve alt kategorileri döndürür</returns>
        public ServiceModel<CategoryModel> GetCategories(string userId, Languages language, PagingModel pagingModel)
        {
            var param = new
            {
                UserId = userId,
                LangId = (int)language,
                PicturePath = DefaultImages.DefaultLock,
                pagingModel.Offset,
                pagingModel.PageSize
            };
            var lookup = new Dictionary<short, CategoryModel>();
            var data = _subCategoryRepository.SpQuery<CategoryModel, SubCategoryModel, CategoryModel>("SP_GetCategories", (category, subCategory) =>
            {
                if (!lookup.TryGetValue(category.CategoryId, out CategoryModel invoiceEntry))
                {
                    invoiceEntry = category;
                    if (invoiceEntry.SubCategories == null) invoiceEntry.SubCategories = new List<SubCategoryModel>();
                    lookup.Add(invoiceEntry.CategoryId, invoiceEntry);
                }
                invoiceEntry.SubCategories.Add(subCategory);
                return invoiceEntry;
            }, param, splitOn: "ContestCategoryId,SubCategoryId", CommandType.StoredProcedure).ToList();

            return new ServiceModel<CategoryModel>
            {
                Items = lookup.Values.Distinct(),
                PageNumber = pagingModel.PageNumber,
                PageSize = pagingModel.PageSize,
            };
        }

        /// <summary>
        /// Kullanıcının takip ettiği alt kategoriler
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <param name="language">Kategori adları hangi dilde dönecek</param>
        /// <param name="pagingModel">Sayfalama</param>
        /// <returns></returns>
        public ServiceModel<SubCategoryModel> GetFollowedSubCategories(string userId, Languages language, PagingModel pagingModel)
        {
            return _subCategoryRepository.ToServiceModel<SubCategoryModel>("SP_GetFollowedSubCategories", new
            {
                userId,
                langId = (byte)language,
                pagingModel.Offset,
                pagingModel.PageSize
            }, CommandType.StoredProcedure, pagingModel);
        }

        /// <summary>
        /// Alt kategori detay bilgilerini verir
        /// </summary>
        /// <param name="subCategoryId">Alt kategori id</param>
        /// <returns>Alt kategori detay</returns>
        public SubCategoryDetailInfoModel GetSubCategoryDetail(short subCategoryId, Languages language)
        {
            return _subCategoryRepository.QuerySingleOrDefault<SubCategoryDetailInfoModel>("SP_GetSubCategoryDetail", new
            {
                subCategoryId,
                langId = (byte)language
            }, CommandType.StoredProcedure);
        }

        /// <summary>
        /// Alt kategorinin fiyatını getirir
        /// </summary>
        /// <param name="subCategoryId">Alt kategori id</param>
        /// <returns>Fiyat</returns>
        public decimal GetSubCategoryPrice(short subCategoryId)
        {
            string sql = "SELECT sc.Price FROM SubCategories sc WHERE sc.SubCategoryId=@subCategoryId;";
            return _subCategoryRepository.QuerySingleOrDefault<decimal>(sql, new
            {
                subCategoryId
            });
        }

        /// <summary>
        /// Alt kategori ücretsiz mi kontrol eder
        /// </summary>
        /// <param name="subCategoryId">Alt kategori id</param>
        /// <returns>Kategori ücretsiz ise true değilse false</returns>
        public bool IsSubCategoryFree(short subCategoryId)
        {
            string sql = "SELECT TRUE FROM SubCategories sc WHERE sc.SubCategoryId = @subCategoryId AND sc.Price=0;";
            return _subCategoryRepository.QuerySingleOrDefault<bool>(sql, new
            {
                subCategoryId
            });
        }

        #endregion Methods
    }
}
