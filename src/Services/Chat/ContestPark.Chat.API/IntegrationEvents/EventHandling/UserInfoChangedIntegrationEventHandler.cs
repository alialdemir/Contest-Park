using ContestPark.Chat.API.Infrastructure.Documents;
using ContestPark.Chat.API.IntegrationEvents.Events;
using ContestPark.Core.Database.Interfaces;
using ContestPark.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ContestPark.Chat.API.IntegrationEvents.EventHandling
{
    public class UserInfoChangedIntegrationEventHandler : IIntegrationEventHandler<UserInfoChangedIntegrationEvent>

    {
        private readonly IRepository<User> _userRepository;
        private readonly ILogger<UserInfoChangedIntegrationEventHandler> _logger;

        public UserInfoChangedIntegrationEventHandler(IRepository<User> userRepository,
                                                      ILogger<UserInfoChangedIntegrationEventHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        /// <summary>
        /// Kullanıcı bilgisini database üzerinden günceller.
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public async Task Handle(UserInfoChangedIntegrationEvent @event)
        {
            try
            {
                User user = _userRepository.FindById(@event.UserId);

                user.FullName = @event.NewFullName;
                user.UserName = @event.NewUserName;

                bool isSuccess = await _userRepository.UpdateAsync(user);
                if (!isSuccess)
                    throw new ArgumentException("Yeni kullanıcı güncellenemedi.");
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Kullanıcı bilgisi follow api güncelleme hatas. userId: {@event.UserId} event Id: {@event.Id}", ex);
            }
        }
    }
}