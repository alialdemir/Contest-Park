using ContestPark.Mobile.Helpers;
using Newtonsoft.Json;

namespace ContestPark.Mobile.Models.Blocking
{
    public class BlockModel : BaseModel
    {
        private string _fullName;
        private bool _isBlocked;
        private string _userId;

        public string FullName
        {
            get { return _fullName; }
            set
            {
                _fullName = value;
                RaisePropertyChanged(() => FullName);
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

        [JsonIgnore]
        public string ProfilePicturePath
        {
            get
            {
                return DefaultImages.DefaultProfilePicture;
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

        private string _userName;

        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                RaisePropertyChanged(() => UserName);
            }
        }
    }
}
