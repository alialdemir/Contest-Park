using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Blocking;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.ServiceModel;
using ContestPark.Mobile.Services.Cache;
using ContestPark.Mobile.Services.RequestProvider;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Blocking
{
    public class BlockingService : IBlockingService
    {
        #region Private variables

        private const string ApiUrlBase = "api/v1/Chat/Block";
        private readonly ICacheService _cacheService;
        private readonly INewRequestProvider _requestProvider;
        private string _lastUserBlockingListKey = string.Empty;

        #endregion Private variables

        #region Constructor

        public BlockingService(INewRequestProvider requestProvider,
                                ICacheService cacheService
            )
        {
            _cacheService = cacheService;
            _requestProvider = requestProvider;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Kullanıcıyı engelle
        /// </summary>
        /// <param name="userId">Kullanicii id</param>
        public async Task<bool> Block(string userId)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/{userId}");

            var result = await _requestProvider.PostAsync<string>(uri);

            CacheEmpty();

            return result.IsSuccess;
        }

        public async Task<ServiceModel<BlockModel>> BlockingList(PagingModel pagingModel)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}{pagingModel.ToString()}");

            if (!_cacheService.IsExpired(key: uri))
            {
                return await _cacheService.Get<ServiceModel<BlockModel>>(uri);
            }

            var result = await _requestProvider.GetAsync<ServiceModel<BlockModel>>(uri);

            _cacheService.Add(uri, result.Data);
            _lastUserBlockingListKey = uri;

            return result.Data;
        }

        /// <summary>
        /// İki kullanıcı arasında engelleme var mı kontrol eder
        /// </summary>
        public async Task<bool> BlockingStatusAsync(string senderUserId)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/Status/{senderUserId}");

            var result = await _requestProvider.GetAsync<BlockStatusModel>(uri);

            return result.Data.IsBlocked;
        }

        /// <summary>
        /// Kullanıcının engellini kaldırır
        /// </summary>
        /// <param name="userId">Kullanicii id</param>
        public async Task<bool> UnBlock(string userId)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"api/v1/chat/unblock/{userId}");

            var result = await _requestProvider.DeleteAsync<string>(uri);

            CacheEmpty();

            return result.IsSuccess;
        }

        /// <summary>
        /// Listelme kısmındaki cache temizleer
        /// </summary>
        private void CacheEmpty()
        {
            if (!string.IsNullOrEmpty(_lastUserBlockingListKey))
            {
                _cacheService.Empty(_lastUserBlockingListKey);
            }
        }

        #endregion Methods
    }
}
