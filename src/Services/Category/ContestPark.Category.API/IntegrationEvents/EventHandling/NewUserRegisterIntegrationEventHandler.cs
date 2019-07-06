using ContestPark.Category.API.Infrastructure.Repositories.Search;
using ContestPark.Category.API.Infrastructure.Tables;
using ContestPark.Category.API.IntegrationEvents.Events;
using ContestPark.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ContestPark.Category.API.IntegrationEvents.EventHandling
{
    public class NewUserRegisterIntegrationEventHandler : IIntegrationEventHandler<NewUserRegisterIntegrationEvent>

    {
        private readonly ISearchRepository _searchRepository;
        private readonly ILogger<NewUserRegisterIntegrationEventHandler> _logger;

        public NewUserRegisterIntegrationEventHandler(ISearchRepository searchRepository,
                                                      ILogger<NewUserRegisterIntegrationEventHandler> logger)
        {
            _searchRepository = searchRepository;
            _logger = logger;
        }

        /// <summary>
        /// Yeni kullanıcı üye olunca elasticsearch e o kullanıcının bilgilerini ekler
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public Task Handle(NewUserRegisterIntegrationEvent @event)
        {
            try
            {
                _searchRepository.Insert(new Search
                {
                    SearchType = SearchTypes.Player,
                    FullName = @event.FullName,
                    UserId = @event.UserId,
                    Language = null,// index için null atadık
                    UserName = @event.UserName,
                    PicturePath = @event.ProfilePicturePath,
                    Id = @event.UserId,
                    Suggest = new Nest.CompletionField
                    {
                        Input = new string[] { @event.UserName, @event.FullName }
                    },
                });

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Yeni kullanıcı elasticsearch eklenme hatası. userId: {@event.UserId} event Id: {@event.Id}", ex);

                return Task.FromException(ex);
            }
        }
    }
}
