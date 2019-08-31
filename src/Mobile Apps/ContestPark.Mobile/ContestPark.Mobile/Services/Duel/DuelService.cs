using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Duel;
using ContestPark.Mobile.Models.Duel.DuelResult;
using ContestPark.Mobile.Services.RequestProvider;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Duel
{
    public class DuelService : IDuelService
    {
        #region Private variables

        private const string ApiUrlBase = "api/v1/Duel";

        private readonly INewRequestProvider _requestProvider;

        #endregion Private variables

        #region Constructor

        public DuelService(INewRequestProvider requestProvider)
        {
            _requestProvider = requestProvider;
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
                string url = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/AddOpponent");

                await _requestProvider.PostAsync<string>(url, standbyModeModel);
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

            var result = await _requestProvider.GetAsync<DuelResultModel>(uri);

            return result.Data;
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
                string url = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/ExitStandbyMode");

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

            var result = await _requestProvider.GetAsync<string[]>(uri);

            return result.Data;
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

                var result = await _requestProvider.PostAsync<string>(url, standbyModeModel);

                return result.IsSuccess;
            }

            return false;
        }

        #endregion Methods
    }
}
