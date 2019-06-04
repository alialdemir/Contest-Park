using ContestPark.Core.CosmosDb.Interfaces;
using Microsoft.Azure.Documents;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Category.API.Infrastructure.Repositories.FollowSubCategory
{
    public class FollowSubCategoryRepository : IFollowSubCategoryRepository
    {
        #region Constructor

        public FollowSubCategoryRepository(IDocumentDbRepository<Documents.FollowSubCategory> repository)
        {
            Repository = repository;
        }

        #endregion Constructor

        #region Properties

        public IDocumentDbRepository<Documents.FollowSubCategory> Repository { get; private set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Kullanıcı alt kategoriyi takip ediyor mu
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <param name="subCategoryId">Alt kategori id</param>
        /// <returns>Alt kategori takip ediyor ise true değilse false</returns>
        public bool IsSubCategoryFollowed(string userId, string subCategoryId)
        {
            return Repository.Query<bool>(new SqlQuerySpec
            {
                QueryText = "SELECT DISTINCT VALUE NOT(IS_NULL(c.id)) FROM c WHERE c.userId=@userId AND c.subCategoryId=@subCategoryId",
                Parameters = new SqlParameterCollection
                {
                    new SqlParameter("@userId", userId),
                    new SqlParameter("@subCategoryId", subCategoryId)
                }
            }).ToList().FirstOrDefault();
        }

        /// <summary>
        /// Kullanıcı ve alt kategori id göre kategori takipten çıkar
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <param name="subCategoryId">Alt kategori id</param>
        /// <returns>Alt kategori takip ediyor ise true değilse false</returns>
        public Task<bool> DeleteAsync(string userId, string subCategoryId)
        {
            string id = Repository.Query<string>(new SqlQuerySpec
            {
                QueryText = @"SELECT DISTINCT VALUE c.id FROM c
                              WHERE c.subCategoryId=@subCategoryId AND c.userId=@userId",
                Parameters = new SqlParameterCollection
                {
                    new SqlParameter("@userId", userId),
                    new SqlParameter("@subCategoryId", subCategoryId)
                }
            }).AsEnumerable().SingleOrDefault();

            if (string.IsNullOrEmpty(id))
                return Task.FromResult(false);

            return Repository.DeleteAsync(id);
        }

        /// <summary>
        /// Kullanıcının takip ettiği alt kategorilerin id'leri
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <returns>Takip edilen alt kategoriler</returns>
        public string[] FollowedSubCategoryIds(string userId)
        {
            return Repository.Query<string>(new SqlQuerySpec
            {
                QueryText = "SELECT DISTINCT VALUE c.subCategoryId FROM c where c.userId = @userId",
                Parameters = new SqlParameterCollection
                 {
                     new SqlParameter("@userId", userId)
                 }
            }).ToArray();
        }

        #endregion Methods
    }
}