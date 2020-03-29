namespace ContestPark.Core.Models
{
    public class UserModel
    {
        public string FullName { get; set; }

        public string UserName { get; set; }

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

        public string UserId { get; set; }
    }
}
