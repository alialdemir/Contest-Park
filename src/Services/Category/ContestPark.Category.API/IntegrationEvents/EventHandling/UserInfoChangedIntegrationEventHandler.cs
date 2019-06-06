using ContestPark.Category.API.Infrastructure.Repositories.Search;
using ContestPark.Category.API.IntegrationEvents.Events;
using ContestPark.Category.API.Model;
using ContestPark.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ContestPark.Category.API.IntegrationEvents.EventHandling
{
    public class UserInfoChangedIntegrationEventHandler : IIntegrationEventHandler<UserInfoChangedIntegrationEvent>

    {
        private readonly ISearchRepository _searchRepository;
        private readonly ILogger<UserInfoChangedIntegrationEventHandler> _logger;

        public UserInfoChangedIntegrationEventHandler(ISearchRepository searchRepository,
            ILogger<UserInfoChangedIntegrationEventHandler> logger)
        {
            _searchRepository = searchRepository;
            _logger = logger;
        }

        /// <summary>
        /// Kullanıcı bilgisini elasticsearch üzerinden günceller.
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public Task Handle(UserInfoChangedIntegrationEvent @event)
        {
            try
            {
                SearchModel searchModel = _searchRepository.SearchByUserId(@event.UserId);
                if (searchModel == null)
                {
                    searchModel = new SearchModel
                    {
                        SearchType = SearchTypes.Player,
                        FullName = @event.NewFullName,
                        UserId = @event.UserId,
                        UserName = @event.NewUserName,
                        Id = @event.UserId
                    };
                }
                searchModel.SearchType = SearchTypes.Player;
                searchModel.FullName = @event.NewFullName;
                searchModel.UserId = @event.UserId;
                searchModel.UserName = @event.NewUserName;
                searchModel.Id = @event.UserId;

                _searchRepository.Update(searchModel);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Kullanıcı bilgisi elasticsearch güncelleme hatas. userId: {@event.UserId} event Id: {@event.Id}");

                return Task.FromException(ex);
            }
        }
    }
}