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
