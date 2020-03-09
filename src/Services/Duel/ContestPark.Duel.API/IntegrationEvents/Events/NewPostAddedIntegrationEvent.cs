using ContestPark.Duel.API.Enums;
using ContestPark.EventBus.Events;

namespace ContestPark.Duel.API.IntegrationEvents.Events
{
    public class NewPostAddedIntegrationEvent : IntegrationEvent
    {
        #region Constructor

        public NewPostAddedIntegrationEvent(PostTypes postType,
                                          string ownerUserId,
                                          decimal? bet,
                                          BalanceTypes balanceType,
                                          string competitorUserId,
                                          byte? competitorTrueAnswerCount,
                                          int duelId,
                                          string founderUserId,
                                          byte? founderTrueAnswerCount,
                                          int subCategoryId)
        {
            PostType = postType;
            OwnerUserId = ownerUserId;
            Bet = bet;
            BalanceType = balanceType;
            CompetitorUserId = competitorUserId;
            CompetitorTrueAnswerCount = competitorTrueAnswerCount;
            DuelId = duelId;
            FounderUserId = founderUserId;
            FounderTrueAnswerCount = founderTrueAnswerCount;
            SubCategoryId = subCategoryId;
        }

        #endregion Constructor

        #region Post

        public PostTypes PostType { get; set; }

        public string OwnerUserId { get; set; }

        #endregion Post

        #region Post contest

        public BalanceTypes BalanceType { get; set; }

        public decimal? Bet { get; set; }

        public string CompetitorUserId { get; set; }

        public byte? CompetitorTrueAnswerCount { get; set; }

        public int DuelId { get; set; }

        public string FounderUserId { get; set; }

        public byte? FounderTrueAnswerCount { get; set; }

        public int SubCategoryId { get; set; }

        #endregion Post contest
    }
}
