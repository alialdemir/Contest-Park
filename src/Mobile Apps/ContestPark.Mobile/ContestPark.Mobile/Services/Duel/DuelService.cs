using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Duel;
using ContestPark.Mobile.Models.Duel.DuelResult;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.ServiceModel;
using ContestPark.Mobile.Services.RequestProvider;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Duel
{
    public class DuelService : IDuelService
    {
        #region Private variables

        private const string ApiUrlBase = "api/v1/duel";
        private readonly IRequestProvider _requestProvider;

        #endregion Private variables

        #region Constructor

        public DuelService(IRequestProvider requestProvider)
        {
            _requestProvider = requestProvider;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Rakip beklediği düelloya bot ekler
        /// </summary>
        /// <param name="botStandbyMode">Bekleme modunda olduğu kategori ve bahis bilgileri</param>
        public async Task BotStandMode(BotStandbyMode botStandbyMode)
        {
            if (botStandbyMode != null && botStandbyMode.SubCategoryId > 0)
            {
                string url = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/bot");
                await _requestProvider.PostAsync<string>(url, botStandbyMode);
            }
        }

        /// <summary>
        /// Düello id'ye ait sonuç ekranını verir
        /// </summary>
        /// <param name="duelId">Düello id</param>
        /// <returns>Düello sonucu</returns>
        public async Task<DuelResultModel> DuelResult(string duelId)
        {
            // TODO: cache
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/{duelId}");
            return await _requestProvider.GetAsync<DuelResultModel>(uri);
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
        public async Task<bool> DuelStartWithUserId(string userId)
        {
            string url = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/duelStart");
            string result = await _requestProvider.PostAsync<string>(url, new { userId });

            return string.IsNullOrEmpty(result);
        }

        /// <summary>
        /// Bekleme modundan çık
        /// </summary>
        /// <param name="standbyModeModel">Bekleme modunda olduğu kategori ve bahis bilgileri</param>
        public async Task ExitStandMode(StandbyModeModel standbyModeModel)
        {
            if (standbyModeModel != null && standbyModeModel.SubCategoryId > 0)
            {
                string url = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/exit");
                await _requestProvider.PostAsync<string>(url, standbyModeModel);
            }
        }

        /// <summary>
        /// Random kullanici resim listesi döndürür
        /// </summary>
        /// <param name="pagingModel">Sayfalama</param>
        /// <returns>Profil resimleri service modeli</returns>
        public async Task<ServiceModel<string>> RandomUserProfilePictures(PagingModel pagingModel)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/randomprofilepictures{pagingModel.ToString()}");
            return await _requestProvider.GetAsync<ServiceModel<string>>(uri);
        }

        /// <summary>
        /// Bekleme moduna al
        /// </summary>
        /// <param name="standbyModeModel">Bekleme modunda olduğu kategori ve bahis bilgileri</param>
        public async Task<bool> StandbyMode(StandbyModeModel standbyModeModel)
        {
            if (standbyModeModel != null && standbyModeModel.SubCategoryId > 0)
            {
                string url = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}");
                string result = await _requestProvider.PostAsync<string>(url, standbyModeModel);

                return string.IsNullOrEmpty(result);
            }

            return false;
        }

        #endregion Methods
    }
}