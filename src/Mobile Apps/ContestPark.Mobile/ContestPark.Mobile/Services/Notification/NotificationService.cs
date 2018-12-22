using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Notification;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.ServiceModel;
using ContestPark.Mobile.Services.Cache;
using ContestPark.Mobile.Services.RequestProvider;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Notification
{
    public class NotificationService : INotificationService
    {
        #region Private variables

        private readonly IRequestProvider _requestProvider;

        private readonly ICacheService _cacheService;

        private const string ApiUrlBase = "api/v1/notification";

        #endregion Private variables

        #region Constructor

        public NotificationService(IRequestProvider requestProvider,
                                ICacheService cacheService
            )
        {
            _cacheService = cacheService;
            _requestProvider = requestProvider;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        //  login olan kullanıcının bildirim listesini verir
        /// </summary>
        /// <param name="pagingModel">Sayfalama</param>
        /// <returns></returns>
        public async Task<ServiceModel<NotificationModel>> NotificationListAsync(PagingModel pagingModel)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}{pagingModel.ToString()}");

            if (!_cacheService.IsExpired(key: uri))
            {
                return await _cacheService.Get<ServiceModel<NotificationModel>>(uri);
            }

            ServiceModel<NotificationModel> notifications = await _requestProvider.GetAsync<ServiceModel<NotificationModel>>(uri);

            _cacheService.Add(uri, notifications);

            return notifications;
        }

        #endregion Methods
    }
}