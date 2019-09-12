using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.Ranking;
using ContestPark.Mobile.Services.Cache;
using ContestPark.Mobile.Services.RequestProvider;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Score
{
    public class ScoreService : IScoreService
    {
        #region Private variables

        private const string ApiUrlBase = "api/v1/Ranking/SubCategory";
        private readonly ICacheService _cacheService;
        private readonly INewRequestProvider _requestProvider;

        #endregion Private variables

        #region Constructor

        public ScoreService(INewRequestProvider requestProvider,
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
        public async Task<RankModel> FollowingRankingAsync(short subCategoryId, BalanceTypes balanceType, PagingModel pagingModel)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/{subCategoryId}/Following{pagingModel.ToString()}&balanceType={balanceType}");

            if (!_cacheService.IsExpired(key: uri))
            {
                return await _cacheService.Get<RankModel>(uri);
            }

            var result = await _requestProvider.GetAsync<RankModel>(uri);
            if (result != null && result.IsSuccess)
            {
                _cacheService.Add(uri, result.Data);
            }

            return result.Data;
        }

        /// <summary>
        /// Alt kategori Id'ye göre sıralama getirir
        /// </summary>
        /// <param name="subCategoryId">Alt kategori Id</param>
        /// <param name="pagingModel">Sayfalama</param>
        /// <returns>Sıralama listesi</returns>
        public async Task<RankModel> SubCategoryRankingAsync(short subCategoryId, BalanceTypes balanceType, PagingModel pagingModel)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/{subCategoryId}{pagingModel.ToString()}&balanceType={balanceType}");

            if (!_cacheService.IsExpired(key: uri))
            {
                return await _cacheService.Get<RankModel>(uri);
            }

            var result = await _requestProvider.GetAsync<RankModel>(uri);
            if (result != null && result.IsSuccess)
            {
                _cacheService.Add(uri, result.Data);
            }

            return result.Data;
        }

        #endregion Methods
    }
}
