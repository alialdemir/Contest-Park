using ContestPark.EventBus.Events;
using ContestPark.Signalr.API.Enums;

namespace ContestPark.Signalr.API.IntegrationEvents.Events
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

        public string OpponentUserId { get; set; }
        public string FounderUserId { get; set; }
        public string FounderProfilePicturePath { get; set; }
        public string FounderFullname { get; set; }
        public string SubCategoryName { get; set; }
        public string SubCategoryPicture { get; set; }
        public BalanceTypes BalanceType { get; set; }
        public bool IsOpponentOpenSubCategory { get; set; }
        public decimal Bet { get; set; }
    }
}
