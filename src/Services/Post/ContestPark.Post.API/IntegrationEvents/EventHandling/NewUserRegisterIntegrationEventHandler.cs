using ContestPark.Core.CosmosDb.Interfaces;
using ContestPark.EventBus.Abstractions;
using ContestPark.Post.API.Infrastructure.Documents;
using ContestPark.Post.API.IntegrationEvents.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ContestPark.Post.API.IntegrationEvents.EventHandling
{
    public class NewUserRegisterIntegrationEventHandler : IIntegrationEventHandler<NewUserRegisterIntegrationEvent>

    {
        private readonly IDocumentDbRepository<User> _userRepository;
        private readonly ILogger<NewUserRegisterIntegrationEventHandler> _logger;

        public NewUserRegisterIntegrationEventHandler(IDocumentDbRepository<User> userRepository,
                                                      ILogger<NewUserRegisterIntegrationEventHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        /// <summary>
        /// Yeni kullanıcı üye olunca database e o kullanıcının bilgilerini ekler
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public async Task Handle(NewUserRegisterIntegrationEvent @event)
        {
            try
            {
                bool isSuccess = await _userRepository.AddAsync(new User
                {
                    Id = @event.UserId,
                    FullName = @event.FullName,
                    ProfilePicturePath = @event.ProfilePicturePath,
                    UserName = @event.UserName
                });

                if (!isSuccess)
                    throw new ArgumentException("Yeni kullanıcı kayıt edilemedi");
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"CRITICAL: Yeni kullanıcı follow user database eklenme hatası. userId: {@event.UserId} event Id: {@event.Id}", ex);
                // TODO: kullanıcı kayıt edilemediyse büyük sıkıntı var
            }
        }
    }
}
