using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Duel;
using ContestPark.Mobile.Models.Duel.DuelResult;
using ContestPark.Mobile.Services.Cache;
using ContestPark.Mobile.Services.RequestProvider;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Duel
{
    public class DuelService : IDuelService
    {
        #region Private variables

        private const string _apiUrlBase = "api/v1/Duel";

        private readonly IRequestProvider _requestProvider;
        private readonly ICacheService _cacheService;

        #endregion Private variables

        #region Constructor

        public DuelService(IRequestProvider requestProvider,
                           ICacheService cacheService)
        {
            _requestProvider = requestProvider;
            _cacheService = cacheService;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Rakip beklediği düelloya bot ekler
        /// </summary>
        /// <param name="standbyModeModel">Bekleme modunda olduğu kategori ve bahis bilgileri</param>
        public async Task AddOpponent(StandbyModeModel standbyModeModel)
        {
            if (standbyModeModel != null && standbyModeModel.SubCategoryId > 0)
            {
                string url = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{_apiUrlBase}/AddOpponent");

                await _requestProvider.PostAsync<string>(url, standbyModeModel);
            }
        }

        /// <summary>
        /// Düello id'ye ait sonuç ekranını verir
        /// </summary>
        /// <param name="duelId">Düello id</param>
        /// <returns>Düello sonucu</returns>
        public async Task<DuelResultModel> DuelResult(int duelId)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{_apiUrlBase}/{duelId}");

            var result = await _requestProvider.GetAsync<DuelResultModel>(uri);

            return result.Data;
        }

        /// <summary>
        /// Düellodan yenilmiş olarak çıkartır
        /// </summary>
        /// <param name="duelId">Duello id</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public async Task<bool> DuelEscape(int duelId)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{_apiUrlBase}/{duelId}/DuelEscape");

            var result = await _requestProvider.PostAsync<string>(uri);

            return result.IsSuccess;
        }

        ///// <summary>
        ///// Düel id ye ait düelloyu başlatır
        ///// </summary>
        ///// <param name="duelId">Duello id</param>
        ///// <returns>İşlem başarılı ise true değilse fale</returns>
        //public async Task<bool> DuelStartWithDuelId(string duelId)
        //{
        //    string url = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/duelStart");
        //    string result = await _requestProvider.PostAsync<string>(url, new { duelId });

        //    return string.IsNullOrEmpty(result);
        //}

        /// <summary>
        /// Kullanıcı id ile arasında düello başlatır
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <returns>İşlem başarılı ise true değilse fale</returns>
        //////////public async Task<bool> DuelStartWithUserId(string userId)// NOTE: bu bildirimden gelen kullanıcı ile düello yapmak için şuan bildirim olmadığı için gerek yok
        //////////{
        //////////    string url = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/duelStart");

        //////////    var result = await _requestProvider.PostAsync<string>(url, new { userId });

        //////////    return result.IsSuccess;
        //////////}

        /// <summary>
        /// Bekleme modundan çık
        /// </summary>
        /// <param name="standbyModeModel">Bekleme modunda olduğu kategori ve bahis bilgileri</param>
        public async Task ExitStandMode(StandbyModeModel standbyModeModel)
        {
            if (standbyModeModel != null && standbyModeModel.SubCategoryId > 0)
            {
                string url = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{_apiUrlBase}/ExitStandbyMode");

                await _requestProvider.PostAsync<string>(url, standbyModeModel);
            }
        }

        /// <summary>
        /// Random kullanici resim listesi döndürür
        /// </summary>
        /// <param name="pagingModel">Sayfalama</param>
        /// <returns>Profil resimleri service modeli</returns>
        public async Task<string[]> RandomUserProfilePictures()
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, "api/v1/Account/GetRandomProfilePictures");

            if (!_cacheService.IsExpired(key: uri))
            {
                return await _cacheService.Get<string[]>(uri);
            }

            var response = await _requestProvider.GetAsync<string[]>(uri);
            if (response.IsSuccess)
            {
                if (_cacheService.IsExpired(uri))
                    _cacheService.Empty(uri);

                _cacheService.Add(uri, response.Data);
            }

            return response.Data;
        }

        /// <summary>
        /// Bekleme moduna al
        /// </summary>
        /// <param name="standbyModeModel">Bekleme modunda olduğu kategori ve bahis bilgileri</param>
        public async Task<bool> StandbyMode(StandbyModeModel standbyModeModel)
        {
            if (standbyModeModel != null && standbyModeModel.SubCategoryId > 0)
            {
                string url = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{_apiUrlBase}");

                var result = await _requestProvider.PostAsync<string>(url, standbyModeModel);

                return result.IsSuccess;
            }

            return false;
        }

        #endregion Methods
    }
}
