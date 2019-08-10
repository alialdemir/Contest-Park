using ContestPark.Mobile.Helpers;

namespace ContestPark.Mobile.Models.Ranking
{
    public class RankingModel : BaseModel
    {
        public string TotalScore { get; set; }

        public string UserFullName { get; set; }
        private string userProfilePicturePath = DefaultImages.DefaultProfilePicture;

        public string UserProfilePicturePath
        {
            get { return userProfilePicturePath; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    userProfilePicturePath = value;
                }
            }
        }

        public string UserName { get; set; }
    }
}
