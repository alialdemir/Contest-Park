using ContestPark.Mobile.Enums;
using Newtonsoft.Json;

namespace ContestPark.Mobile.Models.Duel.InviteDuel
{
    public class InviteModel
    {
        [JsonIgnore]
        public decimal EntryFee
        {
            get { return Bet / 2; }// İki kişi oynandığı için ikiye böldük
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
