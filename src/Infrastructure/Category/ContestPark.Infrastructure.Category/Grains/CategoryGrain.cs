using ContestPark.Core.Domain.Model;
using ContestPark.Core.Enums;
using ContestPark.Domain.Category.Interfaces;
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
                return Task.FromResult(new ServiceResponse<Domain.Category.Model.Response.Category>());

            return Task.FromResult(_categoryRepository.GetCategoryList(userId, language, paging));
        }

        #endregion Methods

        public override Task OnDeactivateAsync()
        {
            _categoryRepository.Dispose();
            return base.OnDeactivateAsync();
        }
    }
}