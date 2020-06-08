﻿using ContestPark.Duel.API.Enums;
using ContestPark.Duel.API.Models;
using System.Threading.Tasks;

namespace ContestPark.Duel.API.Infrastructure.Repositories.Duel
{
    public interface IDuelRepository
    {
        Models.DuelResultModel DuelResultByDuelId(int duelId, string userId);

        Tables.Duel GetDuelByDuelId(int duelId);

        DuelBalanceInfoModel GetDuelBalanceInfoByDuelId(int duelId);

        Task<int?> Insert(Tables.Duel duel);

        Task<bool> UpdateDuelScores(int duelId, DuelTypes duelType, byte founderTotalScore, byte opponentTotalScore, byte founderFinishScore, byte opponentFinishScore, byte founderVictoryScore, byte opponentVictoryScore);

        DuelWinStatusModel WinStatus(string userId);

        int LastPlayingDuel(string userId);

        bool PlaysInTheLastHour(string founderUserId, string opponentUserId);
        bool IsDuelFinish(int duelId);
    }
}
