using ContestPark.Category.API.Infrastructure.Documents;
using ContestPark.Core.CosmosDb.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Category.API.Infrastructure.Repositories.OpenCategory
{
    public class OpenCategoryRepository : IOpenCategoryRepository
    {
        #region Private Variables

        private readonly IDocumentDbRepository<OpenSubCategory> _openSubCategoryRepository;

        #endregion Private Variables

        #region Constructor

        public OpenCategoryRepository(IDocumentDbRepository<OpenSubCategory> repository)
        {
            _openSubCategoryRepository = repository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Kullanıcının açık alt kategorilerinin id listesini verir
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <returns>Sub kategori id listesi</returns>
        public string[] OpenSubCategoryIds(string userId)
        {
            return _openSubCategoryRepository.QueryMultipleAsync<string>("SELECT DISTINCT VALUE c.SubCategoryId FROM c where c.UserId = @userId", new
            {
                userId
            }).ToArray();
        }

        /// <summary>
        /// Alt kategorinin kilidi açık mı kontrol eder
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <param name="subCategoryId">Alt kategori id</param>
        /// <returns>Alt kategori kilidi açık ise true değilse false</returns>
        public bool IsSubCategoryOpen(string userId, string subCategoryId)
        {
            return _openSubCategoryRepository.QuerySingleAsync<bool>("SELECT DISTINCT VALUE NOT(IS_NULL(c.id)) FROM c WHERE c.UserId=@userId AND c.SubCategoryId=@subCategoryId", new
            {
                userId,
                subCategoryId
            });
        }

        public Task<bool> AddAsync(OpenSubCategory openSubCategory)
        {
            return _openSubCategoryRepository.AddAsync(openSubCategory);
        }

        #endregion Methods
    }
}