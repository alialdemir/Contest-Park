using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Categories;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.ServiceModel;
using ContestPark.Mobile.Services.Cache;
using ContestPark.Mobile.Services.RequestProvider;
using System.Diagnostics;
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

        #region Global

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

        /// <summary>
        /// Kategori Id'ye göre kategori listesi getirir 0 ise tüm kategorileri getirir
        /// </summary>
        /// <returns>Tüm kategorileri döndürür.</returns>
        public async Task<ServiceModel<SubCategorySearch>> CategorySearchAsync(short subCategoryId, PagingModel pagingModel)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/{subCategoryId}{pagingModel.ToString()}");

            var categories = await _requestProvider.GetAsync<ServiceModel<SubCategorySearch>>(uri);

            return categories;
        }

        #endregion Global

        #region Follow

        /// <summary>
        /// Kullanıcının alt kategoriyi takip etme durumu
        /// </summary>
        /// <param name="subCategoryId">Alt kategori Id</param>
        /// <returns>Alt kategoriyi ise takip ediyor true etmiyorsa ise false</returns>
        public async Task<bool> IsFollowUpStatusAsync(short subCategoryId)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/{subCategoryId}/follow/status");

            var isFollowSubCategory = await _requestProvider.GetAsync<bool>(uri);

            return isFollowSubCategory;
        }

        /// <summary>
        /// Alt kategori Id'ye göre o kategoriyi takip eden kullanıcı sayısı
        /// </summary>
        /// <param name="subCategoryId">Alt kategori Id</param>
        /// <returns>Kategoriyi takip eden kullanıcı sayısı</returns>
        public async Task<int> FollowersCountAsync(short subCategoryId)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/{subCategoryId}/follow/count");

            var followCount = await _requestProvider.GetAsync<int>(uri);

            return followCount;
        }

        /// <summary>
        /// Alt kategori takip et
        /// </summary>
        /// <param name="subCategoryId">Alt kategori Id</param>
        public async Task<bool> FollowSubCategoryAsync(short subCategoryId)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/{subCategoryId}/follow");

            try
            {
                await _requestProvider.PostAsync<string>(uri);
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Alt kategori takip býrak
        /// </summary>
        /// <param name="subCategoryId">Alt kategori Id</param>
        public async Task<bool> UnFollowSubCategoryAsync(short subCategoryId)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/{subCategoryId}/unfollow");

            try
            {
                await _requestProvider.DeleteAsync<string>(uri);
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }

            return true;
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

        #endregion Follow

        #region Open subCategory

        /// <summary>
        /// Alt kategorinin kilidini açar
        /// </summary>
        /// <param name="subCategoryId">Alt kategori id</param>
        /// <returns></returns>
        public async Task<bool> OpenCategoryAsync(short subCategoryId)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/{subCategoryId}/open");

            var isOpenSubCategory = await _requestProvider.PostAsync<bool>(uri);

            return isOpenSubCategory;
        }

        #endregion Open subCategory

        #region Following sub categories

        /// <summary>
        /// Takip ettiği kategorileri search sayfasında listeleme
        /// </summary>
        /// <returns>Alt kategori listesi</returns>
        public async Task<ServiceModel<SubCategorySearch>> FollowingSubCategorySearchAsync(PagingModel pagingModel)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/following{pagingModel.ToString()}");

            var followingSubCategories = await _requestProvider.GetAsync<ServiceModel<SubCategorySearch>>(uri);

            return followingSubCategories;
        }

        #endregion Following sub categories

        #endregion Methods
    }
}