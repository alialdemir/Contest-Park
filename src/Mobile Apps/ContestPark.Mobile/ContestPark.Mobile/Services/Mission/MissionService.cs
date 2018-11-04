using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Mission;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Services.Cache;
using ContestPark.Mobile.Services.RequestProvider;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Mission
{
    public class MissionService : IMissionService
    {
        #region Private variables

        private readonly IRequestProvider _requestProvider;

        private readonly ICacheService _cacheService;

        private const string ApiUrlBase = "api/v1/mission";

        #endregion Private variables

        #region Constructor

        public MissionService(IRequestProvider requestProvider,
                                ICacheService cacheService)
        {
            _cacheService = cacheService;
            _requestProvider = requestProvider;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Görev listeleme
        /// </summary>
        /// <returns>Tüm görevleri döndürür.</returns>
        public async Task<MissionListModel> MissionListAsync(PagingModel pagingModel)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}{pagingModel.ToString()}");

            if (!_cacheService.IsExpired(key: uri))
            {
                return await _cacheService.Get<MissionListModel>(uri);
            }

            var missions = await _requestProvider.GetAsync<MissionListModel>(uri);

            _cacheService.Add(uri, missions);

            return missions;
        }

        /// <summary>
        /// Görevin altınını topla
        /// </summary>
        /// <param name="missionId">Görev Id</param>
        /// <returns>İşlem sonucu</returns>
        public async Task<bool> TakesMissionGoldAsync(short missionId)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/{missionId}/status");

            var result = await _requestProvider.PostAsync<string>(uri);

            return true;
        }

        #endregion Methods
    }
}