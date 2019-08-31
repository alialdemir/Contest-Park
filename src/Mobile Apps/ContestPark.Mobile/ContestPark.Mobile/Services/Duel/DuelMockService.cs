﻿using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Duel;
using ContestPark.Mobile.Models.Duel.DuelResult;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Duel
{
    public class DuelMockService : IDuelService
    {
        public Task AddOpponent(StandbyModeModel standbyModeModel)
        {
            return Task.CompletedTask;
        }

        public Task<DuelResultModel> DuelResult(int duelId)
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

        public Task<string[]> RandomUserProfilePictures()
        {
            return Task.FromResult(new string[] {
                      DefaultImages.DefaultProfilePicture,
                      DefaultImages.DefaultProfilePicture
            });
        }

        public Task<bool> StandbyMode(StandbyModeModel standbyModeModel)
        {
            return Task.FromResult(true);
        }
    }
}
