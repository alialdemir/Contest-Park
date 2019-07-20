using ContestPark.Core.Models;

namespace ContestPark.Identity.API.Models
{
    public class UserProfileModel
    {
        public long FollowersCount { get; set; }

        public long FollowUpCount { get; set; }
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

        public long GameCount { get; set; }
    }
}
