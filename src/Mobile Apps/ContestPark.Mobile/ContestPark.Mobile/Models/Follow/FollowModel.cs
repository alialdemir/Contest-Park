using Newtonsoft.Json;

namespace ContestPark.Mobile.Models.Follow
{
    public class FollowModel : BaseModel
    {
        private bool isFollowing;
        public string FullName { get; set; }

        public bool IsFollowing
        {
            get { return isFollowing; }
            set
            {
                isFollowing = value;
                RaisePropertyChanged(() => IsFollowing);
            }
        }

        public string ProfilePicturePath { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }

        [JsonIgnore]
        public string Description { get => $"@{UserName}"; }// Bildirim ekranı ile çakıştığı için böyle bir yöntem ekledik
    }
}
