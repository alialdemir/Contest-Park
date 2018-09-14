using ContestPark.Core.Domain.Model;
using ContestPark.Core.Enums;
using ContestPark.Domain.Category.Interfaces;
using ContestPark.Domain.Category.Model.Response;
using ContestPark.Infrastructure.Category.Repositories.Category;
using Orleans;
using System.Threading.Tasks;

namespace ContestPark.Infrastructure.Category.Grains
{
    public class CategoryGrain : Grain, ICategoryGrain

    {
        #region Private variables

        private readonly ICategoryRepository _categoryRepository;

        #endregion Private variables

        #region Constructor

        public CategoryGrain(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Kategorilerin listesi
        /// </summary>
        /// <param name="userId">Kullanıcı Id</param>
        /// <param name="paging">Sayfalama</param>
        /// <returns>Kategori listesi</returns>
        public Task<ServiceResponse<Domain.Category.Model.Response.Category>> GetCategoryList(string userId, Languages language, Paging paging)
        {
            if (string.IsNullOrEmpty(userId))
                return null;

            return Task.FromResult(_categoryRepository.GetCategoryList(userId, language, paging));
        }

        /// <summary>
        /// Alt kategori Id'ye göre kategori listesi getirir
        /// </summary>
        /// <param name="userId">Kullanıcı Id</param>
        /// <param name="categoryId">Alt kategori Id</param>
        /// <param name="language">Kullanıcı dili</param>
        /// <param name="paging">Sayfalama</param>
        /// <returns>Aranan kategorilerin listesi</returns>
        public Task<ServiceResponse<SubCategorySearch>> CategorySearch(string userId, short categoryId, Languages language, Paging paging)
        {
            if (string.IsNullOrEmpty(userId) || categoryId <= 0)
                return null;

            return Task.FromResult(_categoryRepository.CategorySearch(userId, categoryId, language, paging));
        }

        #endregion Methods
    }
}