using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Enums;
using Newtonsoft.Json;

namespace ContestPark.Mobile.Models.Duel.InviteDuel
{
    public class InviteModel : BaseModel
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
        private string _founderFullname;

        public string FounderFullname
        {
            get { return _founderFullname; }
            set
            {
                _founderFullname = value;

                RaisePropertyChanged(() => FounderFullname);
            }
        }

        public string SubCategoryName { get; set; }
        public string SubCategoryPicture { get; set; }
        public BalanceTypes BalanceType { get; set; }
        public bool IsOpponentOpenSubCategory { get; set; }
        public decimal Bet { get; set; }
        public short SubCategoryId { get; set; }
        public string FounderConnectionId { get; set; }
        private string _description;

        public string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                    return string.Format(ContestParkResources.InvitedYouToADuel, FounderFullname);

                return _description;
            }

            set
            {
                _description = value;

                RaisePropertyChanged(() => Description);
            }
        }
    }
}
