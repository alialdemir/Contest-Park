using ContestPark.Core.Models;

namespace ContestPark.Identity.API.Models
{
    public class UserProfileModel
    {
        public string FollowersCount { get; set; }

        public string FollowUpCount { get; set; }
        public string FullName { get; set; }
        private string coverPicture;

        public string CoverPicture
        {
            get { return coverPicture; }
            set
            {
                coverPicture = !string.IsNullOrEmpty(value) ? value : DefaultImages.DefaultCoverPicture;
            }
        }

        private string profilePicturePath;

        public string ProfilePicturePath
        {
            get { return profilePicturePath; }
            set
            {
                profilePicturePath = !string.IsNullOrEmpty(value) ? value : DefaultImages.DefaultProfilePicture;
            }
        }

        public string UserId { get; set; }

        public string GameCount { get; set; }
        public bool IsPrivateProfile { get; set; }
    }
}
