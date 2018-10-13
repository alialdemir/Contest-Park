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

        private readonly IRequestProvider _requestProvider;

        private readonly ICacheService _cacheService;

        private const string ApiUrlBase = "api/v1/blocking";

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
        public async Task Block(string userId)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/{userId}");

            await _requestProvider.PostAsync<string>(uri);

            CacheEmpty();
        }

        /// <summary>
        /// Kullanıcının engellini kaldırır
        /// </summary>
        /// <param name="userId">Kullanicii id</param>
        public async Task UnBlock(string userId)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/{userId}");

            await _requestProvider.DeleteAsync<string>(uri);

            CacheEmpty();
        }

        public async Task<ServiceModel<UserBlocking>> UserBlockingList(PagingModel pagingModel)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}{pagingModel.ToString()}");

            if (!_cacheService.IsExpired(key: uri))
            {
                return await _cacheService.Get<ServiceModel<UserBlocking>>(uri);
            }

            var blockingList = await _requestProvider.GetAsync<ServiceModel<UserBlocking>>(uri);

            _cacheService.Add(uri, blockingList);
            _lastUserBlockingListKey = uri;

            return blockingList;
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