using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Categories;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.ServiceModel;
using ContestPark.Mobile.Services.Cache;
using ContestPark.Mobile.Services.RequestProvider;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Category
{
    public class CategoryServices : ICategoryServices
    {
        #region Private variables

        private readonly IRequestProvider _requestProvider;

        private readonly ICacheService _cacheService;

        private const string ApiUrlBase = "api/v1/category";

        #endregion Private variables

        #region Constructor

        public CategoryServices(IRequestProvider requestProvider,
                                ICacheService cacheService
            )
        {
            _cacheService = cacheService;
            _requestProvider = requestProvider;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Kategorileri listeleme
        /// </summary>
        /// <returns>Tüm kategorileri döndürür.</returns>
        public async Task<ServiceModel<CategoryModel>> CategoryListAsync(PagingModel pagingModel)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}{pagingModel.ToString()}");

            if (!_cacheService.IsExpired(key: uri))
            {
                return await _cacheService.Get<ServiceModel<CategoryModel>>(uri);
            }

            var categories = await _requestProvider.GetAsync<ServiceModel<CategoryModel>>(uri);

            _cacheService.Add(uri, categories);

            return categories;
        }

        #endregion Methods
    }
}