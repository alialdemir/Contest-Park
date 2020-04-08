using ContestPark.Category.API.Infrastructure.Repositories.FollowSubCategory;
using ContestPark.Category.API.Infrastructure.Repositories.OpenSubCategory;
using ContestPark.Category.API.IntegrationEvents.Events;
using ContestPark.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Category.API.IntegrationEvents.EventHandling
{
    public class OpenSubCategoryAndFollowIntegrationEventHandler : IIntegrationEventHandler<OpenSubCategoryAndFollowIntegrationEvent>

    {
        #region Private variables

        private readonly ILogger<OpenSubCategoryAndFollowIntegrationEventHandler> _logger;
        private readonly IOpenCategoryRepository _openCategoryRepository;
        private readonly IFollowSubCategoryRepository _followSubCategoryRepository;

        #endregion Private variables

        #region Constructor

        public OpenSubCategoryAndFollowIntegrationEventHandler(ILogger<OpenSubCategoryAndFollowIntegrationEventHandler> logger,
                                                               IOpenCategoryRepository openCategoryRepository,
                                                               IFollowSubCategoryRepository followSubCategoryRepository)
        {
            _logger = logger;
            _openCategoryRepository = openCategoryRepository;
            _followSubCategoryRepository = followSubCategoryRepository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Yeni üye olan kullanıcılar için parametreden gelen alt kategorilerin kilitleri ücret ödemeden açılır ve takip edilir
        /// </summary>
        public async Task Handle(OpenSubCategoryAndFollowIntegrationEvent @event)
        {
            if (string.IsNullOrEmpty(@event.UserId) || !@event.SubCategories.Any())
                return;

            bool isOpenSubCategories = await _openCategoryRepository.UnLockSubCategory(@event.UserId, @event.SubCategories);
            if (!isOpenSubCategories)
                _logger.LogError("Yeni üye olan kullanıcıların alt kategori kilit açma işlemi başarısız oldu.");

            bool isFollowSubCategories = await _followSubCategoryRepository.FollowSubCategoryAsync(@event.UserId, @event.SubCategories);
            if (!isFollowSubCategories)
                _logger.LogError("Yeni üye olan kullanıcıların alt kategori takip etme işlemi başarısız oldu.");
        }

        #endregion Methods
    }
}
