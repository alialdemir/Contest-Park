using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Duel;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.ServiceModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Duel
{
    public class DuelMockService : IDuelService
    {
        public Task BotStandMode(BotStandbyMode botStandbyMode)
        {
            return Task.CompletedTask;
        }

        public Task ExitStandMode(StandbyModeModel standbyModeModel)
        {
            return Task.CompletedTask;
        }

        public Task<ServiceModel<string>> RandomUserProfilePictures(PagingModel pagingModel)
        {
            return Task.FromResult(new ServiceModel<string>
            {
                Items = new List<string>
                 {
                      DefaultImages.DefaultProfilePicture,
                      DefaultImages.DefaultProfilePicture
                 }
            });
        }

        public Task StandbyMode(StandbyModeModel standbyModeModel)
        {
            return Task.CompletedTask;
        }
    }
}