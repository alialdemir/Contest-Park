using ContestPark.Mobile.Helpers;

namespace ContestPark.Mobile.Models.Profile
{
    public class ProfileInfoModel : BaseModel
    {
        private string _fullName;
        private bool _isBlocked;
        private string _userId;
        private string coverPicture = DefaultImages.DefaultCoverPicture;
        private string followersCount;
        private string followUpCount;
        private string gameCount;

        private bool isFollowing;
        private string profilePicturePath = DefaultImages.DefaultCoverPicture;

        public string CoverPicture
        {
            get { return coverPicture; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    coverPicture = value;

                    RaisePropertyChanged(() => CoverPicture);
                }
            }
        }

        public string FollowersCount
        {
            get { return followersCount; }
            set
            {
                followersCount = value;

                RaisePropertyChanged(() => FollowersCount);
            }
        }

        public string FollowUpCount
        {
            get { return followUpCount; }
            set
            {
                followUpCount = value;

                RaisePropertyChanged(() => FollowUpCount);
            }
        }

        public string FullName
        {
            get { return _fullName; }
            set
            {
                _fullName = value;

                RaisePropertyChanged(() => FullName);
            }
        }

        public string GameCount
        {
            get { return gameCount; }
            set
            {
                gameCount = value;

                RaisePropertyChanged(() => GameCount);
            }
        }

        public bool IsBlocked
        {
            get { return _isBlocked; }
            set
            {
                _isBlocked = value;

                RaisePropertyChanged(() => IsBlocked);
            }
        }

        public bool IsFollowing
        {
            get { return isFollowing; }
            set
            {
                isFollowing = value;

                RaisePropertyChanged(() => IsFollowing);
            }
        }

        public string ProfilePicturePath
        {
            get { return profilePicturePath; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    profilePicturePath = value;

                    RaisePropertyChanged(() => ProfilePicturePath);
                }
            }
        }

        public string UserId
        {
            get { return _userId; }
            set
            {
                _userId = value;

                RaisePropertyChanged(() => UserId);
            }
        }

        public bool IsPrivateProfile { get; set; }
    }
}
