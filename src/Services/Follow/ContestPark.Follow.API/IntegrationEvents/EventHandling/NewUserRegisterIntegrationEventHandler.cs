using ContestPark.EventBus.Abstractions;
using ContestPark.Follow.API.IntegrationEvents.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Follow.API.IntegrationEvents.EventHandling
{
    public class NewUserRegisterIntegrationEventHandler : IIntegrationEventHandler<NewUserRegisterIntegrationEvent>

    {
        private readonly ILogger<NewUserRegisterIntegrationEventHandler> _logger;

        public NewUserRegisterIntegrationEventHandler(
                                                      ILogger<NewUserRegisterIntegrationEventHandler> logger)
        {
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
                //_searchRepository.Insert(new Search
                //{
                //    SearchType = Model.SearchTypes.Player,
                //    FullName = @event.FullName,
                //    UserId = @event.UserId,
                //    Language = null,// index için null atadık
                //    UserName = @event.UserName,
                //    PicturePath = @event.ProfilePicturePath,
                //    Id = @event.UserId,
                //    Suggest = new Nest.CompletionField
                //    {
                //        Input = new string[] { @event.UserName, @event.FullName }
                //    },
                //});

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Yeni kullanıcı follow user database eklenme hatası. userId: {@event.UserId} event Id: {@event.Id}");

                return Task.FromException(ex);
            }
        }
    }
}