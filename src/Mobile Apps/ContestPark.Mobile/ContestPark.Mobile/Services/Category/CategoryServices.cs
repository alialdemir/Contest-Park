using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Categories;
using ContestPark.Mobile.Models.Categories.CategoryDetail;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.RequestProvider;
using ContestPark.Mobile.Models.ServiceModel;
using ContestPark.Mobile.Services.Cache;
using ContestPark.Mobile.Services.RequestProvider;
using Prism.Services;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Category
{
    public class CategoryServices : ICategoryService
    {
        #region Private variables

        private const string ApiUrlBase = "api/v1/SubCategory";
        private readonly ICacheService _cacheService;
        private readonly INewRequestProvider _requestProvider;
        private readonly IPageDialogService _dialogService;

        #endregion Private variables

        #region Constructor

        public CategoryServices(INewRequestProvider requestProvider,
                                IPageDialogService dialogService,
                                ICacheService cacheService
            )
        {
            _cacheService = cacheService;
            _requestProvider = requestProvider;
            _dialogService = dialogService;
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

            var response = await _requestProvider.GetAsync<ServiceModel<CategoryModel>>(uri);

            if (response.IsSuccess)
            {
                if (_cacheService.IsExpired(uri))
                    _cacheService.Empty(uri);

                _cacheService.Add(uri, response.Data);
            }

            return response.Data;
        }

        /// <summary>
        /// Kategori Id'ye göre kategori listesi getirir 0 ise tüm kategorileri getirir
        /// </summary>
        /// <returns>Tüm kategorileri döndürür.</returns>
        public async Task<ServiceModel<SearchModel>> CategorySearchAsync(short categoryId, PagingModel pagingModel)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"api/v1/Search{pagingModel.ToString()}&categoryId={categoryId}&q=");

            var response = await _requestProvider.GetAsync<ServiceModel<SearchModel>>(uri);

            return response.Data;
        }

        /// <summary>
        /// Kategori detaydaki gerekli verileri döndürür
        /// </summary>
        /// <param name="subCategoryId">Alt kategori id</param>
        /// <returns>Kategori detay ekranı</returns>
        public async Task<CategoryDetailModel> GetSubCategoryDetail(short subCategoryId)
        {
            // TODO: kullanıcı kategoriyi takip ederse veya level atlarsa cache deki kategori detay bilgilerinin yenilenmesi lazım

            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/{subCategoryId}"); ;

            if (!_cacheService.IsExpired(key: uri))
            {
                return await _cacheService.Get<CategoryDetailModel>(uri);
            }

            var response = await _requestProvider.GetAsync<CategoryDetailModel>(uri);

            _cacheService.Add(uri, response.Data);

            return response.Data;
        }

        /// <summary>
        /// Alt kategorinin kilidini açar
        /// </summary>
        /// <param name="subCategoryId">Alt kategori id</param>
        /// <returns></returns>
        public Task<ResponseModel<string>> OpenCategoryAsync(short subCategoryId, BalanceTypes balanceType = BalanceTypes.Gold)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/{subCategoryId}/unlock?balanceType={balanceType}");

            return _requestProvider.PostAsync<string>(uri);
        }

        /// <summary>
        /// Kategori adı veya kullanıcı adına göre arama yapar eğer subCategoryId sıfırdan büyükse o alt kategori içinde arar
        /// </summary>
        /// <param name="searchText">Aranan kategori adı, kullanıcı adı veya kullanıcı ad soyad</param>
        /// <param name="pagingModel">Sayfalama</param>
        /// <param name="categoryId">Alt kategori id</param>
        /// <returns>Alt kategori listesi</returns>
        public async Task<ServiceModel<SearchModel>> SearchAsync(string searchText, short categoryId, PagingModel pagingModel)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"api/v1/Search{pagingModel.ToString()}&categoryId={categoryId}&q=");

            var response = await _requestProvider.GetAsync<ServiceModel<SearchModel>>(uri);

            return response.Data;
        }

        #endregion Methods
    }
}
