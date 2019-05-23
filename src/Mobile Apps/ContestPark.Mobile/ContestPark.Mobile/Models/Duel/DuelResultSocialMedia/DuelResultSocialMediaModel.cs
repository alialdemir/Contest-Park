namespace ContestPark.Mobile.Models.Duel.DuelResultSocialMedia
{
    public class DuelResultSocialMediaModel
    {
        public string Date { get; internal set; }
        public string FounderColor { get; set; }
        public string FounderFullName { get; set; }
        public string FounderProfilePicturePath { get; set; }
        public byte FounderScore { get; internal set; }
        public int Gold { get; internal set; }
        public string OpponentColor { get; set; }
        public string OpponentFullName { get; set; }
        public string OpponentProfilePicturePath { get; set; }
        public byte OpponentScore { get; internal set; }
        public string SubCategoryName { get; internal set; }
        public string SubCategoryPicturePath { get; set; }
    }
}