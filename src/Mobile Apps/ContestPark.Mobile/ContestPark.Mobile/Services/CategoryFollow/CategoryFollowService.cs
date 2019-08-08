using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Categories;
using ContestPark.Mobile.Models.Follow;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.ServiceModel;
using ContestPark.Mobile.Services.Cache;
using ContestPark.Mobile.Services.RequestProvider;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.CategoryFollow
{
    public class CategoryFollowService : ICategoryFollowService
    {
        #region Private variables

        private const string ApiUrlBase = "api/v1/SubCategory";
        private readonly INewRequestProvider _requestProvider;
        private readonly ICacheService _cacheService;

        #endregion Private variables

        #region Constructor

        public CategoryFollowService(INewRequestProvider requestProvider,
                                     ICacheService cacheService)
        {
            _requestProvider = requestProvider;
            _cacheService = cacheService;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Takip ettiği kategorileri search sayfasında listeleme
        /// </summary>
        /// <param name="pagingModel">Sayfalama</param>
        /// <returns>Alt kategori listesi</returns>
        public async Task<ServiceModel<SearchModel>> FollowedSubCategoriesAsync(string searchText, PagingModel pagingModel)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"api/v1/Search/Followed{pagingModel.ToString()}&q={searchText}");

            var result = await _requestProvider.GetAsync<ServiceModel<SearchModel>>(uri);

            return result.Data;
        }

        /// <summary>
        /// Alt kategori takip et
        /// </summary>
        /// <param name="subCategoryId">Alt kategori Id</param>
        public async Task<bool> FollowSubCategoryAsync(short subCategoryId)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/{subCategoryId}/Follow");

            try
            {
                var response = await _requestProvider.PostAsync<string>(uri);
                if (response.IsSuccess)
                {
                    DeleteCategoryCache();
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
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/{subCategoryId}/FollowStatus");

            var result = await _requestProvider.GetAsync<SubCategoryFollowModel>(uri);

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
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/{subCategoryId}/UnFollow");

            try
            {
                var response = await _requestProvider.DeleteAsync<string>(uri);
                if (response.IsSuccess)
                {
                    DeleteCategoryCache();
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
        /// Kategori takip ederse veya takipten çıkarırsa kategori listeleme cache siler
        /// </summary>
        private void DeleteCategoryCache()
        {
            string key = $"{GlobalSetting.Instance.GatewaEndpoint}/api/v1/SubCategory?PageSize=9999&PageNumber=1";
            if (!_cacheService.IsExpired(key))
            {
                _cacheService.Empty(key);
            }
        }

        #endregion Methods
    }
}
