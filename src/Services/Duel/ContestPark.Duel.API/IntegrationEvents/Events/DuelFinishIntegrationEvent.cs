using ContestPark.Duel.API.Enums;
using ContestPark.EventBus.Events;

namespace ContestPark.Duel.API.IntegrationEvents.Events
{
    public class DuelFinishIntegrationEvent : IntegrationEvent
    {
        public DuelFinishIntegrationEvent(int duelId,
                                          BalanceTypes balanceType,
                                          decimal bet,
                                          byte betCommission,
                                          short subCategoryId,
                                          string founderUserId,
                                          string opponentUserId,
                                          byte founderScore,
                                          byte opponentScore,
                                          bool isFounderFinishedTheGame,
                                          bool isOpponentFinishedTheGame)
        {
            DuelId = duelId;
            BalanceType = balanceType;
            Bet = bet;
            BetCommission = betCommission;
            FounderUserId = founderUserId;
            SubCategoryId = subCategoryId;
            OpponentUserId = opponentUserId;
            FounderScore = founderScore;
            OpponentScore = opponentScore;
            IsFounderFinishedTheGame = isFounderFinishedTheGame;
            IsOpponentFinishedTheGame = isOpponentFinishedTheGame;
        }

        public int DuelId { get; }

        public BalanceTypes BalanceType { get; }
        public decimal Bet { get; }
        public byte BetCommission { get; }
        public string FounderUserId { get; }

        public short SubCategoryId { get; }

        public string OpponentUserId { get; }

        public byte FounderScore { get; }

        public byte OpponentScore { get; }
        public bool IsFounderFinishedTheGame { get; }
        public bool IsOpponentFinishedTheGame { get; }
    }
}
