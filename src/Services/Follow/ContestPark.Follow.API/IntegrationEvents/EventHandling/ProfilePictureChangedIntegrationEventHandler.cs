using ContestPark.Core.CosmosDb.Interfaces;
using ContestPark.EventBus.Abstractions;
using ContestPark.Follow.API.Infrastructure.Documents;
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
        private readonly IDocumentDbRepository<User> _userRepository;
        private readonly ILogger<ProfilePictureChangedIntegrationEventHandler> _logger;

        public ProfilePictureChangedIntegrationEventHandler(IDocumentDbRepository<User> userRepository,
                                                            ILogger<ProfilePictureChangedIntegrationEventHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        /// <summary>
        /// Kullanıcı profil resmini database üzerinden günceller.
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public Task Handle(ProfilePictureChangedIntegrationEvent @event)
        {
            try
            {
                User user = _userRepository.GetById(@event.UserId);

                if (user.ProfilePicturePath != @event.NewProfilePicturePath)
                {
                    user.ProfilePicturePath = @event.NewProfilePicturePath;

                    _userRepository.UpdateAsync(user);
                }

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