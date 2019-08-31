using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Helpers;
using Newtonsoft.Json;

namespace ContestPark.Mobile.Models.User
{
    public class UserInfoModel : BaseModel
    {
        [JsonProperty("sub")]
        public string UserId { get; set; }

        [JsonProperty("unique_name")]
        public string UserName { get; set; }

        [JsonProperty("full_name")]
        public string FullName { get; set; }

        private string _profilePicturePath = DefaultImages.DefaultProfilePicture;

        [JsonProperty("profile_picture_path")]
        public string ProfilePicturePath
        {
            get { return _profilePicturePath; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _profilePicturePath = value;
                }
            }
        }

        private string _coverPicturePath = DefaultImages.DefaultCoverPicture;

        [JsonProperty("cover_picture_path")]
        public string CoverPicturePath
        {
            get { return _coverPicturePath; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _coverPicturePath = value;
                }
            }
        }

        [JsonProperty("language")]
        public Languages Language { get; set; }

        private bool _isPrivateProfile;

        [JsonProperty("is_private_profile")]
        public bool IsPrivateProfile
        {
            get { return _isPrivateProfile; }
            set
            {
                _isPrivateProfile = value;

                RaisePropertyChanged(() => IsPrivateProfile);
            }
        }
    }
}
