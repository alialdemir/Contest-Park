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

        private const string ApiUrlBase = "api/v1/blocking";
        private readonly ICacheService _cacheService;
        private readonly IRequestProvider _requestProvider;
        private string _lastUserBlockingListKey = string.Empty;

        #endregion Private variables

        #region Constructor

        public BlockingService(IRequestProvider requestProvider,
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

            string result = await _requestProvider.PostAsync<string>(uri);

            CacheEmpty();

            return string.IsNullOrEmpty(result);
        }

        public async Task<ServiceModel<BlockModel>> BlockingList(PagingModel pagingModel)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}{pagingModel.ToString()}");

            if (!_cacheService.IsExpired(key: uri))
            {
                return await _cacheService.Get<ServiceModel<BlockModel>>(uri);
            }

            var blockingList = await _requestProvider.GetAsync<ServiceModel<BlockModel>>(uri);

            _cacheService.Add(uri, blockingList);
            _lastUserBlockingListKey = uri;

            return blockingList;
        }

        /// <summary>
        /// Kullanıcının engellini kaldırır
        /// </summary>
        /// <param name="userId">Kullanicii id</param>
        public async Task<bool> UnBlock(string userId)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/{userId}");

            string result = await _requestProvider.DeleteAsync<string>(uri);

            CacheEmpty();

            return string.IsNullOrEmpty(result);
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