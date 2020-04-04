using ContestPark.Mobile.Enums;

namespace ContestPark.Mobile.Models.Duel.DuelResultSocialMedia
{
    public class DuelResultSocialMediaModel
    {
        public string Date { get; set; }
        public string FounderColor { get; set; }
        public string FounderFullName { get; set; }
        public string FounderProfilePicturePath { get; set; }
        public byte FounderScore { get; set; }
        public decimal Gold { get; set; }
        public string OpponentColor { get; set; }
        public string OpponentFullName { get; set; }
        public string OpponentProfilePicturePath { get; set; }
        public byte OpponentScore { get; set; }
        public string SubCategoryName { get; set; }
        public string SubCategoryPicturePath { get; set; }
        public BalanceTypes BalanceType { get; set; }
        public bool IsShowFireworks { get; set; }
    }
}
