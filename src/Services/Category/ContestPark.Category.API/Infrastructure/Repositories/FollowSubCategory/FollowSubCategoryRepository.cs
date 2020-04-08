using ContestPark.Category.API.Models;
using ContestPark.Core.Database.Interfaces;
using ContestPark.Core.Database.Models;
using ContestPark.Core.Enums;
using System.Collections.Generic;
using System.Linq;
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
        /// Alt kategorilerin takip et
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <param name="subCategoryId">Alt kategori id</param>
        /// <returns>Alt kategori takip ediyor ise true değilse false</returns>
        public Task<bool> FollowSubCategoryAsync(string userId, IEnumerable<short> subCategoryIds)
        {
            if (string.IsNullOrEmpty(userId) || !subCategoryIds.Any())
                return Task.FromResult(false);

            return _followSubCategoryRepository.AddRangeAsync(subCategoryIds.Select(subCategoryId => new Tables.FollowSubCategory
            {
                UserId = userId,
                SubCategoryId = subCategoryId,
            }));
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
        /// Kullanıcının takip ettiği alt kategorileri döndürür
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <returns>Alt kategori id</returns>
        public ServiceModel<SearchModel> FollowedSubCategoryIds(string searchText, string userId, Languages language, PagingModel pagingModel)
        {
            string sql = $@"SELECT
                            scl.SubCategoryName,
                            sc.SubCategoryId,
                            2 AS SearchType,
                            cl.Text AS CategoryName,
                            '0' AS DisplayPrice,
                            sc.PicturePath
                            FROM FollowSubCategories fsc
                            INNER JOIN SubCategories sc ON sc.SubCategoryId = fsc.SubCategoryId AND sc.Visibility = 1
                            INNER JOIN SubCategoryLangs scl ON sc.SubCategoryId = scl.SubCategoryId AND scl.`Language` = @language
                            INNER JOIN SubCategoryRls scr ON scr.SubCategoryId = sc.SubCategoryId
                            INNER JOIN CategoryLocalizeds cl ON cl.CategoryId = scr.CategoryId AND cl.`Language` = @language
                            WHERE fsc.UserId = @userId AND scl.SubCategoryName LIKE '%{searchText}%'";

            return _followSubCategoryRepository.ToServiceModel<SearchModel>(sql, new
            {
                userId,
                language,
            }, pagingModel: pagingModel);
        }

        #endregion Methods
    }
}
