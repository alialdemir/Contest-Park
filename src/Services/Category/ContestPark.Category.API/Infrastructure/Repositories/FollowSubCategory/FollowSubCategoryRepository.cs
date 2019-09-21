using ContestPark.Core.Database.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Category.API.Infrastructure.Repositories.FollowSubCategory
{
    public class FollowSubCategoryRepository : IFollowSubCategoryRepository
    {
        #region Private Variables

        private readonly IRepository<Tables.FollowSubCategory> _followSubCategoryRepository;

        #endregion Private Variables

        #region Constructor

        public FollowSubCategoryRepository(IRepository<Tables.FollowSubCategory> followSubCategoryRepository)
        {
            _followSubCategoryRepository = followSubCategoryRepository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Alt kategori takip et
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <param name="subCategoryId">Alt kategori id</param>
        /// <returns>Alt kategori takip ediyor ise true değilse false</returns>
        public async Task<bool> FollowSubCategoryAsync(string userId, short subCategoryId)
        {
            int? followSubCategoryId = await _followSubCategoryRepository.AddAsync(new Tables.FollowSubCategory
            {
                UserId = userId,
                SubCategoryId = subCategoryId,
            });

            return followSubCategoryId.HasValue;
        }

        /// <summary>
        /// Kullanıcı ve alt kategori id göre kategori takipten çıkar
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <param name="subCategoryId">Alt kategori id</param>
        /// <returns>Alt kategori takip ediyor ise true değilse false</returns>
        public Task<bool> UnFollowSubCategoryAsync(string userId, short subCategoryId)
        {
            string sql = "DELETE FROM FollowSubCategories WHERE UserId=@userId AND SubCategoryId=@subCategoryId;";

            return _followSubCategoryRepository.ExecuteAsync(sql, new
            {
                userId,
                subCategoryId
            });
        }

        /// <summary>
        /// Kullanıcı alt kategoriyi takip ediyor mu
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <param name="subCategoryId">Alt kategori id</param>
        /// <returns>Alt kategori takip ediyor ise true değilse false</returns>
        public bool IsSubCategoryFollowed(string userId, short subCategoryId)
        {
            string sql = @"SELECT (CASE
                           WHEN EXISTS(
                           SELECT NULL
                           FROM FollowSubCategories fsc WHERE fsc.UserId = @userId AND fsc.SubCategoryId = @subCategoryId)
                           THEN 1
                           ELSE 0
                           END)";

            return _followSubCategoryRepository.QuerySingleOrDefault<bool>(sql, new
            {
                userId,
                subCategoryId
            });
        }

        /// <summary>
        /// Kullanıcının takip ettiği alt kategorilerin idlerini döndürür
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <returns>Alt kategori id</returns>
        public IEnumerable<short> FollowedSubCategoryIds(string userId)
        {
            string sql = @"SELECT fsc.SubCategoryId
                           FROM FollowSubCategories fsc
                           WHERE fsc.UserId = @userId";

            return _followSubCategoryRepository.QueryMultiple<short>(sql, new
            {
                userId
            });
        }

        #endregion Methods
    }
}
