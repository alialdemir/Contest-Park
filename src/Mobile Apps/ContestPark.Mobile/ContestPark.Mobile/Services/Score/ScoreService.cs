using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.Ranking;
using ContestPark.Mobile.Models.ServiceModel;
using ContestPark.Mobile.Services.Cache;
using ContestPark.Mobile.Services.RequestProvider;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Score
{
    public class ScoreService : IScoreService
    {
        #region Private variables

        private const string ApiUrlBase = "api/v1/category";
        private readonly ICacheService _cacheService;
        private readonly IRequestProvider _requestProvider;

        #endregion Private variables

        #region Constructor

        public ScoreService(IRequestProvider requestProvider,
                            ICacheService cacheService
            )
        {
            _cacheService = cacheService;
            _requestProvider = requestProvider;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// kullanicinin takip ettigi arkadaşlarının sıralamadaki durumunu verir
        /// </summary>
        /// <param name="subCategoryId">Alt kategori Id</param>
        /// <param name="pagingModel">Sayfalama</param>
        /// <returns>Takip ettiklerinin sıralama listesi</returns>
        public async Task<ServiceModel<RankingModel>> FollowingRankingAsync(short subCategoryId, PagingModel pagingModel)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/{subCategoryId}/ranking/following{pagingModel.ToString()}");

            if (!_cacheService.IsExpired(key: uri))
            {
                return await _cacheService.Get<ServiceModel<RankingModel>>(uri);
            }

            ServiceModel<RankingModel> rankings = await _requestProvider.GetAsync<ServiceModel<RankingModel>>(uri);

            _cacheService.Add(uri, rankings);

            return rankings;
        }

        /// <summary>
        /// x
        /// </summary>
        /// <param name="subCategoryId">Alt kategori Id</param>
        /// <returns>Yarışmanın biteceği tarih</returns>
        public async Task<TimeLeftModel> GetTimeLeft(short subCategoryId)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/{subCategoryId}/ranking/timeleft");

            if (!_cacheService.IsExpired(key: uri))
            {
                return await _cacheService.Get<TimeLeftModel>(uri);
            }

            TimeLeftModel timeLeft = await _requestProvider.GetAsync<TimeLeftModel>(uri);

            _cacheService.Add(uri, timeLeft);

            return timeLeft;
        }

        /// <summary>
        /// Alt kategori Id'ye göre sıralama getirir
        /// </summary>
        /// <param name="subCategoryId">Alt kategori Id</param>
        /// <param name="pagingModel">Sayfalama</param>
        /// <returns>Sıralama listesi</returns>
        public async Task<ServiceModel<RankingModel>> SubCategoryRankingAsync(short subCategoryId, PagingModel pagingModel)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/{subCategoryId}/ranking{pagingModel.ToString()}");

            if (!_cacheService.IsExpired(key: uri))
            {
                return await _cacheService.Get<ServiceModel<RankingModel>>(uri);
            }

            ServiceModel<RankingModel> rankings = await _requestProvider.GetAsync<ServiceModel<RankingModel>>(uri);

            _cacheService.Add(uri, rankings);

            return rankings;
        }

        #endregion Methods
    }
}