using ContestPark.EventBus.Abstractions;
using ContestPark.Follow.API.IntegrationEvents.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Follow.API.IntegrationEvents.EventHandling
{
    public class ProfilePictureChangedIntegrationEventHandler : IIntegrationEventHandler<ProfilePictureChangedIntegrationEvent>

    {
        private readonly ILogger<ProfilePictureChangedIntegrationEventHandler> _logger;

        public ProfilePictureChangedIntegrationEventHandler(
                                                            ILogger<ProfilePictureChangedIntegrationEventHandler> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Kullanıcı profil resmini elasticsearch üzerinden günceller.
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public Task Handle(ProfilePictureChangedIntegrationEvent @event)
        {
            try
            {
                //Search searchModel = _searchRepository.SearchById(@event.UserId, SearchTypes.Player, null);
                //if (searchModel == null)
                //{
                //    searchModel = new Search
                //    {
                //        SearchType = SearchTypes.Player,
                //        UserId = @event.UserId,
                //        Id = @event.UserId
                //    };
                //}
                //if (searchModel.PicturePath != @event.NewProfilePicturePath)
                //{
                //    searchModel.PicturePath = @event.NewProfilePicturePath;

                //    _searchRepository.Update(searchModel);
                //}

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Kullanıcı profil resmi follow api güncelleme hatas. userId: {@event.UserId} event Id: {@event.Id}", ex);

                return Task.FromException(ex);
            }
        }
    }
}