using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Duel;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.ServiceModel;
using ContestPark.Mobile.Services.RequestProvider;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Duel
{
    public class DuelService : IDuelService
    {
        #region Private variables

        private readonly IRequestProvider _requestProvider;

        private const string ApiUrlBase = "api/v1/duel";

        #endregion Private variables

        #region Constructor

        public DuelService(IRequestProvider requestProvider)
        {
            _requestProvider = requestProvider;
        }

        #endregion Constructor

        #region Methods

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
        public async Task StandbyMode(StandbyModeModel standbyModeModel)
        {
            if (standbyModeModel != null && standbyModeModel.SubCategoryId > 0)
            {
                string url = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}");
                await _requestProvider.PostAsync<string>(url, standbyModeModel);
            }
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

        #endregion Methods
    }
}