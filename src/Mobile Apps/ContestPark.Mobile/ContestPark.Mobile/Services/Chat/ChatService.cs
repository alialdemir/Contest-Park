using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Chat;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.ServiceModel;
using ContestPark.Mobile.Services.Cache;
using ContestPark.Mobile.Services.RequestProvider;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Chat
{
    public class ChatService : IChatService
    {
        #region Private variables

        private const string ApiUrlBase = "api/v1/chat";
        private readonly ICacheService _cacheService;
        private readonly IRequestProvider _requestProvider;

        #endregion Private variables

        #region Constructor

        public ChatService(IRequestProvider requestProvider,
                           ICacheService cacheService
            )
        {
            _cacheService = cacheService;
            _requestProvider = requestProvider;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Parametreden geln kullanıcı arasındaki sohbet geçmişi
        /// </summary>
        /// <param name="senderId">kullanıcı Id</param>
        /// <returns>Sohbet geçmiþinin listesi</returns>
        public async Task<ServiceModel<ChatDetailModel>> ChatDetailAsync(string senderUserId, PagingModel pagingModel)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/{senderUserId}/{pagingModel.ToString()}");

            return await _requestProvider.GetAsync<ServiceModel<ChatDetailModel>>(uri);
        }

        /// <summary>
        /// Kullanıcının görmediği tüm mesajlarını görüldü yapar
        /// </summary>
        /// <returns>Başarılı ise true değilse false</returns>
        public async Task<bool> ChatSeenAsync()
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, ApiUrlBase);

            bool isSuccess = await _requestProvider.PostAsync<bool>(uri);

            return isSuccess;
        }

        /// <summary>
        /// İlgili chat'deki tüm mesajları sil
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(string receiverUserId)
        {
            if (string.IsNullOrEmpty(receiverUserId))
                return false;

            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, ApiUrlBase);

            bool isSuccess = await _requestProvider.DeleteAsync<bool>($"{uri}/{receiverUserId}");

            return isSuccess;
        }

        /// <summary>
        /// Login olan kullanıcının mesaj listesi
        /// </summary>
        /// <returns>Mesaj listesi.</returns>
        public async Task<ServiceModel<ChatModel>> UserChatList(PagingModel pagingModel)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}{pagingModel.ToString()}");

            if (!_cacheService.IsExpired(key: uri))
            {
                return await _cacheService.Get<ServiceModel<ChatModel>>(uri);
            }

            var chatList = await _requestProvider.GetAsync<ServiceModel<ChatModel>>(uri);

            _cacheService.Add(uri, chatList);

            return chatList;
        }

        /// <summary>
        /// Kullanıcının okunmamış mesaj sayısı
        /// </summary>
        /// <returns>Okunmamýþ mesaj sayısı</returns>
        public async Task<int> UserChatVisibilityCountAsync()
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, ApiUrlBase);

            int chatCount = await _requestProvider.GetAsync<int>($"{uri}/VisibilityCount");

            return chatCount;
        }

        #endregion Methods
    }
}