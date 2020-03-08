using ContestPark.Core.Enums;
using ContestPark.EventBus.Events;
using ContestPark.Signalr.API.Enums;

namespace ContestPark.Signalr.API.IntegrationEvents.Events
{
    public class InviteDuelIntegrationEvent : IntegrationEvent
    {
        public InviteDuelIntegrationEvent(string opponentUserId,
                                          string founderUserId,
                                          string founderProfilePicturePath,
                                          Languages founderLanguage,
                                          string founderFullname,
                                          string subCategoryName,
                                          string subCategoryPicture,
                                          BalanceTypes balanceType,
                                          bool isOpponentOpenSubCategory,
                                          decimal bet,
                                          short subCategoryId,
                                          string founderConnectionId)
        {
            OpponentUserId = opponentUserId;
            FounderUserId = founderUserId;
            FounderProfilePicturePath = founderProfilePicturePath;
            FounderLanguage = founderLanguage;
            FounderFullname = founderFullname;
            SubCategoryName = subCategoryName;
            SubCategoryPicture = subCategoryPicture;
            BalanceType = balanceType;
            IsOpponentOpenSubCategory = isOpponentOpenSubCategory;
            Bet = bet;
            SubCategoryId = subCategoryId;
            FounderConnectionId = founderConnectionId;
        }

        public string OpponentUserId { get; }
        public string FounderUserId { get; }
        public string FounderProfilePicturePath { get; }
        public Languages FounderLanguage { get; }
        public string FounderFullname { get; }
        public string SubCategoryName { get; }
        public string SubCategoryPicture { get; }
        public BalanceTypes BalanceType { get; }
        public bool IsOpponentOpenSubCategory { get; }
        public decimal Bet { get; }
        public short SubCategoryId { get; }
        public string FounderConnectionId { get; }
    }
}
