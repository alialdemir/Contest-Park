using ContestPark.Duel.API.Enums;
using ContestPark.EventBus.Events;

namespace ContestPark.Duel.API.IntegrationEvents.Events
{
    public class InviteDuelIntegrationEvent : IntegrationEvent
    {
        public InviteDuelIntegrationEvent(string opponentUserId,
                                          string founderUserId,
                                          string founderProfilePicturePath,
                                          string founderFullname,
                                          string subCategoryName,
                                          string subCategoryPicture,
                                          BalanceTypes balanceType,
                                          bool isOpponentOpenSubCategory,
                                          decimal bet)
        {
            OpponentUserId = opponentUserId;
            FounderUserId = founderUserId;
            FounderProfilePicturePath = founderProfilePicturePath;
            FounderFullname = founderFullname;
            SubCategoryName = subCategoryName;
            SubCategoryPicture = subCategoryPicture;
            BalanceType = balanceType;
            IsOpponentOpenSubCategory = isOpponentOpenSubCategory;
            Bet = bet;
        }

        public string OpponentUserId { get; }
        public string FounderUserId { get; }
        public string FounderProfilePicturePath { get; }
        public string FounderFullname { get; }
        public string SubCategoryName { get; }
        public string SubCategoryPicture { get; }
        public BalanceTypes BalanceType { get; }
        public bool IsOpponentOpenSubCategory { get; }
        public decimal Bet { get; }
    }
}
