﻿using ContestPark.Category.API.Infrastructure.Repositories.Search;
using ContestPark.Category.API.Infrastructure.Tables;
using ContestPark.Category.API.IntegrationEvents.Events;
using ContestPark.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ContestPark.Category.API.IntegrationEvents.EventHandling
{
    public class ProfilePictureChangedIntegrationEventHandler : IIntegrationEventHandler<ProfilePictureChangedIntegrationEvent>

    {
        private readonly ISearchRepository _searchRepository;
        private readonly ILogger<ProfilePictureChangedIntegrationEventHandler> _logger;

        public ProfilePictureChangedIntegrationEventHandler(ISearchRepository searchRepository,
                                                            ILogger<ProfilePictureChangedIntegrationEventHandler> logger)
        {
            _searchRepository = searchRepository;
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
                Search searchModel = _searchRepository.SearchById(@event.UserId, SearchTypes.Player, null);
                if (searchModel == null)
                {
                    searchModel = new Search
                    {
                        SearchType = SearchTypes.Player,
                        UserId = @event.UserId,
                        Id = @event.UserId
                    };
                }
                if (searchModel.PicturePath != @event.NewProfilePicturePath)
                {
                    searchModel.PicturePath = @event.NewProfilePicturePath;

                    _searchRepository.Update(searchModel);
                }

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Kullanıcı profil resmi elasticsearch güncelleme hatas. userId: {@event.UserId} event Id: {@event.Id}", ex);

                return Task.FromException(ex);
            }
        }
    }
}
