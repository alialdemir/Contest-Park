using ContestPark.Mobile.Helpers;
using Newtonsoft.Json;
using Xamarin.Forms;

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

        [JsonIgnore]
        public CornerRadius CornerRadius { get; set; } = new CornerRadius(0, 0, 0, 0);

        public string UserName { get; set; }
    }
}
