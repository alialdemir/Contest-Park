using ContestPark.EventBus.Abstractions;
using ContestPark.Identity.API.IntegrationEvents.Events;
using ContestPark.Identity.API.Models;
using ContestPark.Identity.API.Services.NumberFormat;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ContestPark.Identity.API.IntegrationEvents.EventHandling
{
    public class FollowIntegrationEventHandler :
        IIntegrationEventHandler<FollowIntegrationEvent>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INumberFormatService _numberFormatService;
        private readonly ILogger<FollowIntegrationEventHandler> _logger;

        public FollowIntegrationEventHandler(UserManager<ApplicationUser> userManager,
                                             INumberFormatService numberFormatService,
                                             ILogger<FollowIntegrationEventHandler> logger)
        {
            _userManager = userManager;
            _numberFormatService = numberFormatService;
            _logger = logger;
        }

        public async Task Handle(FollowIntegrationEvent @event)
        {
            ApplicationUser followUpUser = await _userManager.FindByIdAsync(@event.FollowUpUserId);
            ApplicationUser followedUser = await _userManager.FindByIdAsync(@event.FollowedUserId);
            if (followUpUser == null || followedUser == null)
            {
                _logger.LogWarning("Takip işlemi sırasında takipçi sayıları güncellenemedi", @event.FollowUpUserId, @event.FollowedUserId);

                return;
            }

            followUpUser.FollowingCount += 1;
            followedUser.FollowersCount += 1;

            followUpUser.DisplayFollowingCount = _numberFormatService.NumberFormating(followUpUser.FollowingCount);
            followedUser.DisplayFollowersCount = _numberFormatService.NumberFormating(followedUser.FollowersCount);

            IdentityResult identityResultFollowUpUser = await _userManager.UpdateAsync(followUpUser);
            IdentityResult identityResultFollowedUser = await _userManager.UpdateAsync(followedUser);

            if (!identityResultFollowUpUser.Succeeded || !identityResultFollowedUser.Succeeded)
            {
                _logger.LogWarning($@"Kullanıcıların follow işleminde takipçi sayısı güncellenirken hata oluştu.
                                      FollowUpUserId: {@event.FollowUpUserId}
                                      Status: {identityResultFollowUpUser.Succeeded}
                                      FollowedUserId: {@event.FollowedUserId}
                                      Status: {identityResultFollowedUser.Succeeded}");
            }
        }
    }
}
