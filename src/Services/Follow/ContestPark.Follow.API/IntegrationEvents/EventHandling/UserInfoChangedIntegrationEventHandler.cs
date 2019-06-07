using ContestPark.EventBus.Abstractions;
using ContestPark.Follow.API.IntegrationEvents.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ContestPark.Follow.API.IntegrationEvents.EventHandling
{
    public class UserInfoChangedIntegrationEventHandler : IIntegrationEventHandler<UserInfoChangedIntegrationEvent>

    {
        private readonly ILogger<UserInfoChangedIntegrationEventHandler> _logger;

        public UserInfoChangedIntegrationEventHandler(ILogger<UserInfoChangedIntegrationEventHandler> logger)
        {
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
                //Search searchModel = _searchRepository.SearchById(@event.UserId, SearchTypes.Player, null);
                //if (searchModel == null)
                //{
                //    searchModel = new Search
                //    {
                //        SearchType = SearchTypes.Player,
                //        FullName = @event.NewFullName,
                //        UserId = @event.UserId,
                //        UserName = @event.NewUserName,
                //        Id = @event.UserId
                //    };
                //}
                //searchModel.SearchType = SearchTypes.Player;
                //searchModel.FullName = @event.NewFullName;
                //searchModel.UserId = @event.UserId;
                //searchModel.UserName = @event.NewUserName;
                //searchModel.Id = @event.UserId;

                //searchModel.Suggest = new Nest.CompletionField
                //{
                //    Input = new string[] { @event.NewUserName, @event.NewFullName }
                //};
                //_searchRepository.Update(searchModel);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Kullanıcı bilgisi follow api güncelleme hatas. userId: {@event.UserId} event Id: {@event.Id}");

                return Task.FromException(ex);
            }
        }
    }
}