﻿using ContestPark.Mobile.Models.Duel;
using ContestPark.Mobile.Models.Duel.DuelResult;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.ServiceModel;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Duel
{
    public interface IDuelService
    {
        Task BotStandMode(BotStandbyMode botStandbyMode);

        Task<DuelResultModel> DuelResult(string duelId);

        //   Task<bool> DuelStartWithDuelId(string duelId);

        Task<bool> DuelStartWithUserId(string userId);

        Task ExitStandMode(StandbyModeModel standbyModeModel);

        Task<ServiceModel<string>> RandomUserProfilePictures(PagingModel pagingModel);

        Task<bool> StandbyMode(StandbyModeModel standbyModeModel);
    }
}