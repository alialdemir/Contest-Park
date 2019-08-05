using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Duel;
using ContestPark.Mobile.Models.Duel.DuelResult;
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

        public Task<DuelResultModel> DuelResult(string duelId)
        {
            return Task.FromResult(new DuelResultModel
            {
                FounderProfilePicturePath = DefaultImages.DefaultProfilePicture,
                FounderUserName = "witcherfearless",
                OpponentProfilePicturePath = DefaultImages.DefaultProfilePicture,
                OpponentUserName = "eliföz",
                SubCategoryName = "Futbol",
                FounderFullName = "Ali Aldemir",
                OpponentFullName = "Elif Öz",
                FounderUserId = "1111-1111-1111-1111",
                OpponentUserId = "2222-2222-2222-2222",
                FounderScore = 12,
                OpponentScore = 12,
                FinishBonus = 40,
                VictoryBonus = 30,
                OpponentLevel = 1,
                FounderLevel = 7,
                SubCategoryPicturePath = DefaultImages.DefaultLock,
                SubCategoryId = 1,
                Gold = 6234
            });
        }

        public Task<bool> DuelStartWithUserId(string userId)
        {
            return Task.FromResult(true);
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

        public Task<bool> StandbyMode(StandbyModeModel standbyModeModel)
        {
            return Task.FromResult(true);
        }
    }
}
