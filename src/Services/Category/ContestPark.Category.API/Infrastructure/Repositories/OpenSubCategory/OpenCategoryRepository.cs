using ContestPark.Core.Database.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Category.API.Infrastructure.Repositories.OpenSubCategory
{
    public class OpenCategoryRepository : IOpenCategoryRepository
    {
        #region Private Variables

        private readonly IRepository<Tables.OpenSubCategory> _openSubCategoryreRepository;

        #endregion Private Variables

        #region Constructor

        public OpenCategoryRepository(IRepository<Tables.OpenSubCategory> openSubCategoryreRepository)
        {
            _openSubCategoryreRepository = openSubCategoryreRepository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Kategori kilidi aç
        /// </summary>
        /// <param name="openSubCategory">Kilidi açılan alt kategori</param>
        /// <returns>Kilit açılmış ise true açılmamış ise false döner</returns>
        public async Task<bool> UnLockSubCategory(string userId, short subCategoryId)
        {
            int? openSubCategoryId = await _openSubCategoryreRepository.AddAsync(new Tables.OpenSubCategory
            {
                UserId = userId,
                SubCategoryId = subCategoryId
            });

            return openSubCategoryId.HasValue;
        }

        /// <summary>
        /// Kategori kilidi aç
        /// </summary>
        /// <param name="openSubCategory">Kilidi açılan alt kategoriler</param>
        /// <returns>Kilit açılmış ise true açılmamış ise false döner</returns>
        public Task<bool> UnLockSubCategory(string userId, IEnumerable<short> subCategoryIds)
        {
            if (string.IsNullOrEmpty(userId) || !subCategoryIds.Any())
                return Task.FromResult(false);

            return _openSubCategoryreRepository.AddRangeAsync(subCategoryIds.Select(subCategoryId => new Tables.OpenSubCategory
            {
                UserId = userId,
                SubCategoryId = subCategoryId,
            }));
        }

        /// <summary>
        /// Alt kategorinin kilidini açmış mı kontrol eder
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <param name="subCategoryId">Alt kategori id</param>
        /// <returns>Kilit açılmış ise true açılmamış ise false döner</returns>
        public bool IsSubCategoryOpen(string userId, short subCategoryId)
        {
            string sql = @"SELECT (CASE
                           WHEN EXISTS(
                           SELECT NULL
                           FROM OpenSubCategories osc WHERE osc.UserId = @userId AND osc.SubCategoryId = @subCategoryId)
                           THEN 1
                           ELSE 0
                           END);";

            return _openSubCategoryreRepository.QuerySingleOrDefault<bool>(sql, new
            {
                userId,
                subCategoryId
            });
        }

        /// <summary>
        /// Parametreden gelen alt kategorilerin kilidi açık mı kontrol eder
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <param name="subCategoryIds">Alt kategori idleri</param>
        /// <returns>Açık olan alt kategori idlerini döndürür</returns>
        public List<short> IsSubCategoryOpen(string userId, IEnumerable<short> subCategoryIds)
        {
            string sql = @"SELECT osc.SubCategoryId FROM OpenSubCategories osc
                           WHERE osc.SubCategoryId IN @subCategoryIds
                             AND osc.UserId = @userId";

            return _openSubCategoryreRepository.QueryMultiple<short>(sql, new
            {
                userId,
                subCategoryIds
            }).ToList();
        }

        #endregion Methods
    }
}
