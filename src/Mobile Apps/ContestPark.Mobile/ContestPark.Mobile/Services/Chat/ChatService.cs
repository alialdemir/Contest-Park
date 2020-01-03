using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Chat;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.ServiceModel;
using ContestPark.Mobile.Services.RequestProvider;
using Prism.Services;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Chat
{
    public class ChatService : IChatService
    {
        #region Private variables

        private const string _apiUrlBase = "api/v1/chat";
        private readonly IRequestProvider _requestProvider;
        private readonly IPageDialogService _pageDialogService;

        #endregion Private variables

        #region Constructor

        public ChatService(IRequestProvider requestProvider,
                           IPageDialogService pageDialogService)
        {
            _requestProvider = requestProvider;
            _pageDialogService = pageDialogService;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Kullanıcının parametreden gelen kullanıcı ile arasındaki sohbet detayı
        /// </summary>
        /// <param name="senderId">kullanıcı Id</param>
        /// <returns>Sohbet geçmiþinin listesi</returns>
        public async Task<ServiceModel<ChatDetailModel>> ChatDetailAsync(string senderUserId, PagingModel pagingModel)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{_apiUrlBase}/{senderUserId}{pagingModel.ToString()}");

            var result = await _requestProvider.GetAsync<ServiceModel<ChatDetailModel>>(uri);

            return result.Data;
        }

        /// <summary>
        /// Kullanıcının görmediği tüm mesajlarını görüldü yapar
        /// </summary>
        /// <returns>Başarılı ise true değilse false</returns>
        public async Task<bool> ChatSeenAsync()
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{_apiUrlBase}/ReadMessages");

            var result = await _requestProvider.PostAsync<string>(uri);

            return result.IsSuccess;
        }

        /// <summary>
        /// İlgili chat'deki tüm mesajları sil
        /// </summary>
        /// <returns>Başarılı ise true değilse false</returns>
        public async Task<bool> DeleteAsync(long conversationId)
        {
            if (conversationId <= 0)
                return false;

            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, _apiUrlBase);

            var result = await _requestProvider.DeleteAsync<string>($"{uri}/{conversationId}");

            return result.IsSuccess;
        }

        /// <summary>
        /// Mesaj gönderme
        /// </summary>
        /// <param name="messageModel">Mesaj ve kullanıcı id</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public async Task<bool> SendMessage(MessageModel messageModel)
        {
            if (messageModel == null || string.IsNullOrEmpty(messageModel.Text) || string.IsNullOrEmpty(messageModel.ReceiverUserId))
                return false;

            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, _apiUrlBase);

            var result = await _requestProvider.PostAsync<string>($"{uri}", messageModel);

            if (!result.IsSuccess)
            {
                await _pageDialogService.DisplayAlertAsync("",
                    result.Error.ErrorMessage,
                    ContestParkResources.Okay);
            }

            return result.IsSuccess;
        }

        /// <summary>
        /// Login olan kullanıcının mesaj listesi
        /// </summary>
        /// <returns>Mesaj listesi.</returns>
        public async Task<ServiceModel<ChatModel>> UserChatList(PagingModel pagingModel)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{_apiUrlBase}{pagingModel.ToString()}");

            var result = await _requestProvider.GetAsync<ServiceModel<ChatModel>>(uri);

            return result.Data;
        }

        /// <summary>
        /// Kullanıcının okunmamış mesaj sayısı
        /// </summary>
        /// <returns>Okunmamýþ mesaj sayısı</returns>
        public async Task<int> UserChatVisibilityCountAsync()
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{_apiUrlBase}/UnReadMessageCount");

            var result = await _requestProvider.GetAsync<UnReadMessageCountModel>(uri);

            return result.Data.UnReadMessageCount;
        }

        #endregion Methods
    }
}
