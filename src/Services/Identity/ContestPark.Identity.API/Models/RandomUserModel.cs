using ContestPark.Core.Models;

namespace ContestPark.Identity.API.Models
{
    public class RandomUserModel
    {
        public string UserId { get; set; }
        public string FullName { get; set; }

        private string _profilePicturePath = DefaultImages.DefaultProfilePicture;

        public string ProfilePicturePath
        {
            get { return _profilePicturePath; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    _profilePicturePath = value;
            }
        }
    }
}
