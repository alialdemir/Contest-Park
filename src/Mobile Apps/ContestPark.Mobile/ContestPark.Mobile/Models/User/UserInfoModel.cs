using ContestPark.Mobile.Enums;
using Newtonsoft.Json;

namespace ContestPark.Mobile.Models.User
{
    public class UserInfoModel
    {
        [JsonProperty("sub")]
        public string UserId { get; set; }

        [JsonProperty("unique_name")]
        public string UserName { get; set; }

        //public string Password { get; set; }// todo password alınacak mı

        [JsonProperty("full_name")]
        public string FullName { get; set; }

        [JsonProperty("profile_picture_path")]
        public string ProfilePicturePath { get; set; }

        [JsonProperty("cover_picture_path")]
        public string CoverPicturePath { get; set; }

        [JsonProperty("language")]
        public Languages Language { get; set; }
    }
}