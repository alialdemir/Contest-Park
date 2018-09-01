using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Services.Cache;
using ContestPark.Mobile.Services.RequestProvider;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Cp
{
    public class CpService : ICpService
    {
        #region Private variables

        private readonly IRequestProvider _requestProvider;

        private readonly ICacheService _cacheService;

        private const string ApiUrlBase = "api/v1/cp";

        #endregion Private variables

        #region Constructor

        public CpService(IRequestProvider requestProvider,
                         ICacheService cacheService)
        {
            _requestProvider = requestProvider;
            _cacheService = cacheService;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Giriiş toplam altın miktarını verir
        /// </summary>
        /// <returns>Toplam altın miktarı</returns>
        public async Task<int> GetTotalCpByUserIdAsync()
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, ApiUrlBase);

            //if (!_cacheService.IsExpired(key: uri))
            //{
            //    return await _cacheService.Get<int>(uri);
            //}

            int userTotalCp = await _requestProvider.GetAsync<int>(uri);

            //_cacheService.Add(uri, userTotalCp);

            return userTotalCp;
        }

        #endregion Methods
    }
}