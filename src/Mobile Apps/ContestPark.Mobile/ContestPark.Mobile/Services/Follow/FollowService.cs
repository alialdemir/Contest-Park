using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Services.RequestProvider;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Follow
{
    public class FollowService : IFollowService
    {
        #region Private variables

        private const string ApiUrlBase = "api/v1/follow";
        private readonly IRequestProvider _requestProvider;

        #endregion Private variables

        #region Constructor

        public FollowService(IRequestProvider requestProvider)
        {
            _requestProvider = requestProvider;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Takip et
        /// </summary>
        /// <param name="followedUserId">kullanıcı Id</param>
        public async Task<bool> FollowUpAsync(string followedUserId)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/{followedUserId}");

            string result = await _requestProvider.PostAsync<string>(uri);

            return string.IsNullOrEmpty(result);
        }

        /// <summary>
        /// Takibi bırak
        /// </summary>
        /// <param name="followedUserId">kullanıcı Id</param>
        public async Task<bool> UnFollowAsync(string followedUserId)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/{followedUserId}");

            string result = await _requestProvider.DeleteAsync<string>(uri);

            return string.IsNullOrEmpty(result);
        }

        #endregion Methods
    }
}