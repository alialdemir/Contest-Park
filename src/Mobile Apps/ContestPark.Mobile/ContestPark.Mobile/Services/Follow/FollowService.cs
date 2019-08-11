using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Follow;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.ServiceModel;
using ContestPark.Mobile.Services.Cache;
using ContestPark.Mobile.Services.RequestProvider;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Follow
{
    public class FollowService : IFollowService
    {
        #region Private variables

        private const string ApiUrlBase = "api/v1/follow";
        private readonly INewRequestProvider _requestProvider;

        #endregion Private variables

        #region Constructor

        public FollowService(INewRequestProvider requestProvider,
            ICacheService cacheService)
        {
            _requestProvider = requestProvider;
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
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/{followedUserId}/Followers{pagingModel.ToString()}");

            var result = await _requestProvider.GetAsync<ServiceModel<FollowModel>>(uri);

            return result.Data;
        }

        /// <summary>
        /// Takip eden kullanıcılar
        /// </summary>
        /// <param name="followedUserId">Takip edilen user id</param>
        /// <returns>Takip eden kullanıcı listesi</returns>
        public async Task<ServiceModel<FollowModel>> Following(string followedUserId, PagingModel pagingModel)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/{followedUserId}/Following{pagingModel.ToString()}");

            var result = await _requestProvider.GetAsync<ServiceModel<FollowModel>>(uri);

            return result.Data;
        }

        /// <summary>
        /// Takip et
        /// </summary>
        /// <param name="followedUserId">kullanıcı Id</param>
        public async Task<bool> FollowUpAsync(string followedUserId)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/{followedUserId}");

            var result = await _requestProvider.PostAsync<string>(uri);

            return result.IsSuccess;
        }

        /// <summary>
        /// Takibi bırak
        /// </summary>
        /// <param name="followedUserId">kullanıcı Id</param>
        public async Task<bool> UnFollowAsync(string followedUserId)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/{followedUserId}");

            var result = await _requestProvider.DeleteAsync<string>(uri);

            return result.IsSuccess;
        }

        #endregion Methods
    }
}
