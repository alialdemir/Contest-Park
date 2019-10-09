namespace ContestPark.Identity.API.Models
{
    public class ProfileInfoModel
    {
        public string CoverPicture { get; set; }

        private string _followersCount = "0";

        public string FollowersCount
        {
            get { return _followersCount; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _followersCount = value;
                }
            }
        }

        private string _followUpCount = "0";

        public string FollowUpCount
        {
            get { return _followUpCount; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _followUpCount = value;
                }
            }
        }

        public string FullName { get; set; }

        private string _gameCount = "0";

        public string GameCount
        {
            get { return _gameCount; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _gameCount = value;
                }
            }
        }

        public bool? IsBlocked { get; set; }

        public bool? IsFollowing { get; set; }

        public string ProfilePicturePath { get; set; }

        public string UserId { get; set; }
        public bool IsPrivateProfile { get; set; }
    }
}
