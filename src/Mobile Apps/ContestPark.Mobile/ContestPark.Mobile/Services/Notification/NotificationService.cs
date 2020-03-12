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

        private const string _apiUrlBase = "api/v1/Notification";
        private readonly ICacheService _cacheService;
        private readonly IRequestProvider _requestProvider;

        #endregion Private variables

        #region Constructor

        public NotificationService(
            IRequestProvider requestProvider,
            ICacheService cacheService)
        {
            _cacheService = cacheService;
            _requestProvider = requestProvider;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Bildirim listesi
        /// </summary>
        /// <param name="pagingModel">Sayfalama</param>
        /// <returns>Kullanıcının bildirimleri</returns>
        public async Task<ServiceModel<NotificationModel>> NotificationsAsync(PagingModel pagingModel)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{_apiUrlBase}{pagingModel.ToString()}");

            if (!_cacheService.IsExpired(key: uri))
            {
                return await _cacheService.Get<ServiceModel<NotificationModel>>(uri);
            }

            var result = await _requestProvider.GetAsync<ServiceModel<NotificationModel>>(uri);

            if (result.Data != null && result.IsSuccess)
            {
                if (_cacheService.IsExpired(uri))
                    _cacheService.Empty(uri);

                _cacheService.Add(uri, result.Data);
            }

            return result.Data;
        }

        #endregion Methods
    }
}
