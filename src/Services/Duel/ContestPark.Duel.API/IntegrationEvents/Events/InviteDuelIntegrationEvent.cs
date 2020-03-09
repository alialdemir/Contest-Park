using ContestPark.Core.Enums;
using ContestPark.Duel.API.Enums;
using ContestPark.EventBus.Events;

namespace ContestPark.Duel.API.IntegrationEvents.Events
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

        public string OpponentUserId { get; set; }
        public string FounderUserId { get; set; }
        public string FounderProfilePicturePath { get; set; }
        public Languages FounderLanguage { get; set; }
        public string FounderFullname { get; set; }
        public string SubCategoryName { get; set; }
        public string SubCategoryPicture { get; set; }
        public BalanceTypes BalanceType { get; set; }
        public bool IsOpponentOpenSubCategory { get; set; }
        public decimal Bet { get; set; }
        public short SubCategoryId { get; set; }
        public string FounderConnectionId { get; set; }
    }
}
