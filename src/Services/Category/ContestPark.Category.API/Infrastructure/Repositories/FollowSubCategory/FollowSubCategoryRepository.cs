using ContestPark.Core.CosmosDb.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Category.API.Infrastructure.Repositories.FollowSubCategory
{
    public class FollowSubCategoryRepository : IFollowSubCategoryRepository
    {
        #region Private variables

        private readonly IDocumentDbRepository<Documents.FollowSubCategory> _followRepository;

        #endregion Private variables

        #region Constructor

        public FollowSubCategoryRepository(IDocumentDbRepository<Documents.FollowSubCategory> repository)
        {
            _followRepository = repository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Kullanıcı alt kategoriyi takip ediyor mu
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <param name="subCategoryId">Alt kategori id</param>
        /// <returns>Alt kategori takip ediyor ise true değilse false</returns>
        public bool IsSubCategoryFollowed(string userId, string subCategoryId)
        {
            return _followRepository.QuerySingle<bool>("SELECT DISTINCT VALUE NOT(IS_NULL(c.id)) FROM c WHERE c.UserId=@userId AND c.SubCategoryId=@subCategoryId", new
            {
                userId,
                subCategoryId
            });
        }

        /// <summary>
        /// Kullanıcı ve alt kategori id göre kategori takipten çıkar
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <param name="subCategoryId">Alt kategori id</param>
        /// <returns>Alt kategori takip ediyor ise true değilse false</returns>
        public Task<bool> DeleteAsync(string userId, string subCategoryId)
        {
            return _followRepository.RemoveAsync(x => x.UserId == userId && x.SubCategoryId == subCategoryId);
        }

        /// <summary>
        /// Kullanıcının takip ettiği alt kategorilerin id'leri
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <returns>Takip edilen alt kategoriler</returns>
        public string[] FollowedSubCategoryIds(string userId)
        {
            return _followRepository.QueryMultiple<string>("SELECT DISTINCT VALUE c.SubCategoryId FROM c where c.UserId = @userId", new
            {
                userId
            }).ToArray();
        }

        public Task<bool> AddAsync(Documents.FollowSubCategory followSubCategory)
        {
            return _followRepository.AddAsync(followSubCategory);
        }

        #endregion Methods
    }
}