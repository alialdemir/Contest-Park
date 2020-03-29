using ContestPark.Core.Enums;
using ContestPark.Core.Models;

namespace ContestPark.Identity.API.Models
{
    public class UserInfoModel
    {
        public string FullName { get; set; }

        private string _profilePicturePath = DefaultImages.DefaultProfilePicture;

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

        public Languages Language { get; set; } = Languages.English;
        public bool IsPrivateProfile { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public string Roles { get; set; }
    }
}
