using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Categories;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.ServiceModel;
using ContestPark.Mobile.Services.RequestProvider;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.CategoryFollow
{
    public class CategoryFollowService : ICategoryFollowService
    {
        #region Private variables

        private const string ApiUrlBase = "api/v1/category/follow";
        private readonly IRequestProvider _requestProvider;

        #endregion Private variables

        #region Constructor

        public CategoryFollowService(IRequestProvider requestProvider)
        {
            _requestProvider = requestProvider;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Takip ettiği kategorileri search sayfasıbda listeleme
        /// </summary>
        /// <param name="pagingModel">Sayfalama</param>
        /// <returns>Alt kategori listesi</returns>
        public async Task<ServiceModel<SearchModel>> FollowingSubCategorySearchAsync(string searchText, short subCategoryId, PagingModel pagingModel)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/{subCategoryId}{pagingModel}&searchText={searchText}");

            return await _requestProvider.GetAsync<ServiceModel<SearchModel>>(uri);
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

        #endregion Methods
    }
}