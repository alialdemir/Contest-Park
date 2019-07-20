using ContestPark.EventBus.Abstractions;
using ContestPark.Identity.API.IntegrationEvents.Events;
using ContestPark.Identity.API.Models;
using ContestPark.Identity.API.Services.NumberFormat;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ContestPark.Identity.API.IntegrationEvents.EventHandling
{
    public class ChangedGameCountIntegrationEventHandler :
        IIntegrationEventHandler<ChangedGameCountIntegrationEvent>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INumberFormatService _numberFormatService;
        private readonly ILogger<ChangedGameCountIntegrationEventHandler> _logger;

        public ChangedGameCountIntegrationEventHandler(UserManager<ApplicationUser> userManager,
                                                       INumberFormatService numberFormatService,
                                                       ILogger<ChangedGameCountIntegrationEventHandler> logger)
        {
            _userManager = userManager;
            _numberFormatService = numberFormatService;
            _logger = logger;
        }

        public async Task Handle(ChangedGameCountIntegrationEvent @event)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(@event.UserId);
            if (user == null)
            {
                _logger.LogWarning("Düello oynama sayısı güncellenemedi", @event.UserId);

                return;
            }

            user.GameCount += 1;
            user.DisplayGameCount = _numberFormatService.NumberFormating(user.GameCount);

            IdentityResult identityResultFollowUpUser = await _userManager.UpdateAsync(user);

            if (!identityResultFollowUpUser.Succeeded)
            {
                _logger.LogWarning($@"Düello oynama sayısı güncellenirken hata oluştu.
                                      UserId: {@event.UserId}
                                      Status: {identityResultFollowUpUser.Succeeded}");
            }
        }
    }
}
