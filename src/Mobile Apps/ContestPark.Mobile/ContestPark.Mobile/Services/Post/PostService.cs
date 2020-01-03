using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.Post;
using ContestPark.Mobile.Models.Post.PostLikes;
using ContestPark.Mobile.Models.ServiceModel;
using ContestPark.Mobile.Services.Cache;
using ContestPark.Mobile.Services.RequestProvider;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Post
{
    public class PostService : IPostService
    {
        #region Private variables

        private const string _apiUrlBase = "api/v1/Post";
        private readonly ICacheService _cacheService;
        private readonly IRequestProvider _requestProvider;

        #endregion Private variables

        #region Constructor

        public PostService(IRequestProvider requestProvider,
                           ICacheService cacheService)
        {
            _requestProvider = requestProvider;
            _cacheService = cacheService;
        }

        public static string ApiUrlBase1 => _apiUrlBase;

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Post beğenmekten vazgeç
        /// </summary>
        /// <param name="postId">Post id</param>
        /// <returns>İşlem başarılı ise true değilse false</returns>
        public async Task<bool> DisLikeAsync(int postId)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase1}/{postId}/UnLike");

            var result = await _requestProvider.DeleteAsync<string>(uri);

            return result.IsSuccess;
        }

        /// <summary>
        /// Post id ait postu döndürür
        /// </summary>
        /// <param name="postId">Post id</param>
        /// <returns>Post detayı</returns>
        public async Task<PostDetailModel> GetPostByPostIdAsync(int postId, PagingModel pagingModel)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase1}/{postId}{pagingModel.ToString()}");

            var result = await _requestProvider.GetAsync<PostDetailModel>(uri);
            if (result.Data != null && result.IsSuccess)
            {
                _cacheService.Add(uri, result.Data);
            }

            return result.Data;
        }

        /// <summary>
        /// Alt kategoriye ait postlar
        /// </summary>
        /// <param name="pagingModel">Sayfalama</param>
        /// <returns>Alt kategori postları</returns>
        public async Task<ServiceModel<PostModel>> GetPostsBySubCategoryIdAsync(short subCategoryId, PagingModel pagingModel, bool isForceCache = false)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase1}/Subcategory/{subCategoryId}{pagingModel.ToString()}");

            if (isForceCache)
                _cacheService.Empty(uri);

            if (!_cacheService.IsExpired(key: uri))
            {
                return await _cacheService.Get<ServiceModel<PostModel>>(uri);
            }

            var result = await _requestProvider.GetAsync<ServiceModel<PostModel>>(uri);
            if (result.Data != null && result.IsSuccess)
            {
                _cacheService.Add(uri, result.Data);
            }

            return result.Data;
        }

        /// <summary>
        /// Kullanıcı id'ye göre posts döner
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <param name="pagingModel">Sayfalama</param>
        /// <returns>Posts listesi</returns>
        public async Task<ServiceModel<PostModel>> GetPostsByUserIdAsync(string userId, PagingModel pagingModel)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase1}/User/{userId}{pagingModel.ToString()}");

            if (!_cacheService.IsExpired(key: uri))
            {
                return await _cacheService.Get<ServiceModel<PostModel>>(uri);
            }

            var result = await _requestProvider.GetAsync<ServiceModel<PostModel>>(uri);
            if (result.Data != null && result.IsSuccess)
            {
                _cacheService.Add(uri, result.Data);
            }
            return result.Data;
        }

        /// <summary>
        /// Post beğenme
        /// </summary>
        /// <param name="postId">Post id</param>
        /// <returns>İşlem başarılı ise true değilse false</returns>
        public async Task<bool> LikeAsync(int postId)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase1}/{postId}/Like");

            var result = await _requestProvider.PostAsync<string>(uri);

            return result.IsSuccess;
        }

        /// <summary>
        /// Post id göre o postu beğenen kullanıcıları döndürür
        /// </summary>
        /// <param name="postId">Post id</param>
        /// <param name="pagingModel">Sayfalama</param>
        /// <returns>Postu beğenen kullanıcı listesi</returns>
        public async Task<ServiceModel<PostLikeModel>> PostLikesAsync(int postId, PagingModel pagingModel)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase1}/{postId}/Like{pagingModel.ToString()}");

            if (!_cacheService.IsExpired(key: uri))
            {
                return await _cacheService.Get<ServiceModel<PostLikeModel>>(uri);
            }

            var result = await _requestProvider.GetAsync<ServiceModel<PostLikeModel>>(uri);
            if (result.Data != null && result.IsSuccess)
            {
                _cacheService.Add(uri, result.Data);
            }

            return result.Data;
        }

        /// <summary>
        /// Postaa yorum yaz
        /// </summary>
        /// <param name="postId">Post id</param>
        /// <param name="comment">Yorumu</param>
        /// <returns>Başarılı ise true değilsa falase</returns>
        public async Task<bool> SendCommentAsync(int postId, string comment)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase1}/{postId}/Comment");

            var result = await _requestProvider.PostAsync<string>(uri, new { comment });

            return result.IsSuccess;
        }

        #endregion Methods
    }
}
