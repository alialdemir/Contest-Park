using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.ServiceModel;
using ContestPark.Mobile.Models.Slide;
using ContestPark.Mobile.Services.Cache;
using ContestPark.Mobile.Services.RequestProvider;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Notice
{
    public class NoticeService : INoticeService
    {
        #region Private variables

        private const string _apiUrlBase = "api/v1/Notice";
        private readonly ICacheService _cacheService;
        private readonly IRequestProvider _requestProvider;

        #endregion Private variables

        #region Constructor

        public NoticeService(IRequestProvider requestProvider,
                             ICacheService cacheService)
        {
            _cacheService = cacheService;
            _requestProvider = requestProvider;
        }

        #endregion Constructor

        #region Methods

        public async Task<ServiceModel<NoticeModel>> NoticesAsync(PagingModel pagingModel)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{_apiUrlBase}{pagingModel}");

            if (!_cacheService.IsExpired(key: uri))
            {
                return await _cacheService.Get<ServiceModel<NoticeModel>>(uri);
            }

            var response = await _requestProvider.GetAsync<ServiceModel<NoticeModel>>(uri);
            if (response.Data == null)
                response.Data = new ServiceModel<NoticeModel>()
                {
                    Items = new List<NoticeModel>()
                };

            if (response.Data != null && response.IsSuccess)
            {
                if (!_cacheService.IsExpired(uri))
                    _cacheService.Empty(uri);

                _cacheService.Add(uri, response.Data);
            }

            return response.Data;
        }

        #endregion Methods
    }
}
