using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Categories;
using ContestPark.Mobile.Models.Categories.CategoryDetail;
using ContestPark.Mobile.Models.Follow;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.RequestProvider;
using ContestPark.Mobile.Models.ServiceModel;
using ContestPark.Mobile.Services.Analytics;
using ContestPark.Mobile.Services.Cache;
using ContestPark.Mobile.Services.RequestProvider;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Category
{
    public class CategoryServices : ICategoryService
    {
        #region Private variables

        private const string _apiUrlBase = "api/v1/SubCategory";
        private readonly ICacheService _cacheService;
        private readonly IRequestProvider _requestProvider;
        private readonly IAnalyticsService _analyticsService;

        #endregion Private variables

        #region Constructor

        public CategoryServices(
            IRequestProvider requestProvider,
            IAnalyticsService analyticsService,
            ICacheService cacheService)
        {
            _cacheService = cacheService;
            _requestProvider = requestProvider;
            _analyticsService = analyticsService;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Alt kategori takip et
        /// </summary>
        /// <param name="subCategoryId">Alt kategori Id</param>
        public async Task<bool> FollowSubCategoryAsync(short subCategoryId)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{_apiUrlBase}/{subCategoryId}/Follow");

            try
            {
                var response = await _requestProvider.PostAsync<string>(uri);
                if (response.IsSuccess)
                {
                    DeleteCategoryCache(subCategoryId);
                }
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Kullanıcının alt kategoriyi takip etme durumu
        /// </summary>
        /// <param name="subCategoryId">Alt kategori Id</param>
        /// <returns>Alt kategoriyi ise takip ediyor true etmiyorsa ise false</returns>
        public async Task<bool> IsFollowUpStatusAsync(short subCategoryId)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{_apiUrlBase}/{subCategoryId}/FollowStatus");

            var result = await _requestProvider.GetAsync<SubCategoryFollowModel>(uri);
            if (result.Data == null)
                return false;

            return result.Data.IsSubCategoryFollowed;
        }

        /// <summary>
        /// Subcategory takip et takipten çıkart
        /// </summary>
        /// <param name="subCategoryId">Alt kategori Id</param>
        /// <param name="isSubCategoryFollowUpStatus">Takip etme durumu</param>
        /// <returns>İşlem başarılı ise true değilse false</returns>
        public async Task<bool> SubCategoryFollowProgcess(short subCategoryId, bool isSubCategoryFollowUpStatus)
        {
            if (isSubCategoryFollowUpStatus)
                return await UnFollowSubCategoryAsync(subCategoryId);// true ise takip ediyor

            return await FollowSubCategoryAsync(subCategoryId);// false ise takip etmiyor
        }

        /// <summary>
        /// Alt kategori takip bırak
        /// </summary>
        /// <param name="subCategoryId">Alt kategori Id</param>
        public async Task<bool> UnFollowSubCategoryAsync(short subCategoryId)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{_apiUrlBase}/{subCategoryId}/UnFollow");

            try
            {
                var response = await _requestProvider.DeleteAsync<string>(uri);
                if (response.IsSuccess)
                {
                    DeleteCategoryCache(subCategoryId);
                }
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Kategorileri listeleme
        /// </summary>
        /// <returns>Tüm kategorileri döndürür.</returns>
        public async Task<ServiceModel<CategoryModel>> CategoryListAsync(PagingModel pagingModel, bool isForceCache = false)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{_apiUrlBase}{pagingModel}");

            if (isForceCache)
                _cacheService.Empty(uri);

            if (!_cacheService.IsExpired(key: uri))
            {
                return await _cacheService.Get<ServiceModel<CategoryModel>>(uri);
            }

            var response = await _requestProvider.GetAsync<ServiceModel<CategoryModel>>(uri);

            if (response.Data == null)
                response.Data = new ServiceModel<CategoryModel>();

            if (response.Data != null && response.IsSuccess)
            {
                if (_cacheService.IsExpired(uri))
                    _cacheService.Empty(uri);

                _cacheService.Add(uri, response.Data);
            }

            return response.Data;
        }

        /// <summary>
        /// Takip ettiği kategorileri search sayfasında listeleme
        /// </summary>
        /// <param name="pagingModel">Sayfalama</param>
        /// <returns>Alt kategori listesi</returns>
        public async Task<ServiceModel<SearchModel>> FollowedSubCategoriesAsync(string searchText, PagingModel pagingModel)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"api/v1/Search/Followed{pagingModel}&q={searchText}");

            if (!_cacheService.IsExpired(key: uri))
            {
                return await _cacheService.Get<ServiceModel<SearchModel>>(uri);
            }

            var response = await _requestProvider.GetAsync<ServiceModel<SearchModel>>(uri);
            if (response.Data != null && response.IsSuccess)
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
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"api/v1/Search{pagingModel}&categoryId={categoryId}&q=");

            if (!_cacheService.IsExpired(key: uri))
            {
                return await _cacheService.Get<ServiceModel<SearchModel>>(uri);
            }

            var response = await _requestProvider.GetAsync<ServiceModel<SearchModel>>(uri);
            if (response.Data != null && response.IsSuccess)
            {
                if (_cacheService.IsExpired(uri))
                    _cacheService.Empty(uri);

                _cacheService.Add(uri, response.Data);
            }

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

            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{_apiUrlBase}/{subCategoryId}");

            if (!_cacheService.IsExpired(key: uri))
            {
                return await _cacheService.Get<CategoryDetailModel>(uri);
            }

            var response = await _requestProvider.GetAsync<CategoryDetailModel>(uri);
            if (response.Data != null && response.IsSuccess)
                _cacheService.Add(uri, response.Data);

            return response.Data;
        }

        /// <summary>
        /// Alt kategorinin kilidini açar
        /// </summary>
        /// <param name="subCategoryId">Alt kategori id</param>
        /// <returns></returns>
        public async Task<ResponseModel<string>> OpenCategoryAsync(short subCategoryId, BalanceTypes balanceType = BalanceTypes.Gold)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{_apiUrlBase}/{subCategoryId}/unlock?balanceType={balanceType}");

            var result = await _requestProvider.PostAsync<string>(uri);

            if (result.IsSuccess)
            {
                DeleteCategoryCache(subCategoryId);
            }

            return result;
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
            _analyticsService.SendEvent("Arama", "Kategori Kullanıcı ara", searchText);

            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"api/v1/Search{pagingModel}&categoryId={categoryId}&q={searchText}");

            var response = await _requestProvider.GetAsync<ServiceModel<SearchModel>>(uri);

            return response.Data;
        }

        /// <summary>
        /// Kategori takip ederse veya takipten çıkarırsa kategori listeleme cache siler
        /// </summary>
        private void DeleteCategoryCache(short subCategoryId)
        {
            string subCategoryDetailKey = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{_apiUrlBase}/{subCategoryId}");
            string categorieskey = $"{GlobalSetting.Instance.GatewaEndpoint}/api/v1/SubCategory?PageSize=9999&PageNumber=1";

            if (!_cacheService.IsExpired(categorieskey))
            {
                _cacheService.Empty(categorieskey);
            }

            if (!_cacheService.IsExpired(subCategoryDetailKey))
            {
                _cacheService.Empty(subCategoryDetailKey);
            }
        }

        #endregion Methods
    }
}
