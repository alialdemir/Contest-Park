using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Follow;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.ServiceModel;
using ContestPark.Mobile.Services.Cache;
using ContestPark.Mobile.Services.RequestProvider;
using Prism.Events;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Follow
{
    public class FollowService : IFollowService
    {
        #region Private variables

        private const string _apiUrlBase = "api/v1/Follow";
        private readonly IRequestProvider _requestProvider;
        private readonly IEventAggregator _eventAggregator;
        private readonly ICacheService _cacheService;

        #endregion Private variables

        #region Constructor

        public FollowService(IRequestProvider requestProvider,
                             IEventAggregator eventAggregator,
                             ICacheService cacheService)
        {
            _requestProvider = requestProvider;
            _eventAggregator = eventAggregator;
            _cacheService = cacheService;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Takip edilen kullanıcılar
        /// </summary>
        /// <param name="followedUserId">Takip edilen user id</param>
        /// <returns>Takip edilen kullanıcı listesi</returns>
        public async Task<ServiceModel<FollowModel>> Followers(string followedUserId, PagingModel pagingModel)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{_apiUrlBase}/{followedUserId}/Followers{pagingModel.ToString()}");

            if (!_cacheService.IsExpired(key: uri))
            {
                return await _cacheService.Get<ServiceModel<FollowModel>>(uri);
            }

            var result = await _requestProvider.GetAsync<ServiceModel<FollowModel>>(uri);
            if (result.Data != null && result.IsSuccess)
            {
                _cacheService.Add(uri, result.Data);
            }

            return result.Data;
        }

        /// <summary>
        /// Takip eden kullanıcılar
        /// </summary>
        /// <param name="followedUserId">Takip edilen user id</param>
        /// <returns>Takip eden kullanıcı listesi</returns>
        public async Task<ServiceModel<FollowModel>> Following(string followedUserId, PagingModel pagingModel)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{_apiUrlBase}/{followedUserId}/Following{pagingModel.ToString()}");

            if (!_cacheService.IsExpired(key: uri))
            {
                return await _cacheService.Get<ServiceModel<FollowModel>>(uri);
            }

            var result = await _requestProvider.GetAsync<ServiceModel<FollowModel>>(uri);
            if (result.Data != null && result.IsSuccess)
            {
                _cacheService.Add(uri, result.Data);
            }

            return result.Data;
        }

        /// <summary>
        /// Takip et
        /// </summary>
        /// <param name="followedUserId">kullanıcı Id</param>
        public async Task<bool> FollowUpAsync(string followedUserId)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{_apiUrlBase}/{followedUserId}");

            var result = await _requestProvider.PostAsync<string>(uri);
            if (result.IsSuccess)
                EmptyFollowCache(followedUserId);

            return result.IsSuccess;
        }

        /// <summary>
        /// Takibi bırak
        /// </summary>
        /// <param name="followedUserId">kullanıcı Id</param>
        public async Task<bool> UnFollowAsync(string followedUserId)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{_apiUrlBase}/{followedUserId}");

            var result = await _requestProvider.DeleteAsync<string>(uri);
            if (result.IsSuccess)
                EmptyFollowCache(followedUserId);

            return result.IsSuccess;
        }

        private void EmptyFollowCache(string userId)
        {
            _cacheService.EmptyByKey(_apiUrlBase);
            _cacheService.EmptyByKey("/api/v1/Account/Profile");
            _cacheService.EmptyByKey("/api/v1/Notification");

            _eventAggregator
                .GetEvent<ChangedFollowCountEvent>()
                .Publish(userId);
        }

        #endregion Methods
    }
}
