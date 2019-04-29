using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Base;
using Newtonsoft.Json;
using System.ComponentModel;

namespace ContestPark.Mobile.Models.Blocking
{
    public class BlockModel : IModelBase, INotifyPropertyChanged
    {
        private string _fullName;
        private bool _isBlocked;
        private string _userId;

        private string _userName;

        public event PropertyChangedEventHandler PropertyChanged;

        public string FullName
        {
            get { return _fullName; }
            set
            {
                _fullName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FullName)));
            }
        }

        public bool IsBlocked
        {
            get { return _isBlocked; }
            set
            {
                _isBlocked = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsBlocked)));
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UserId)));
            }
        }

        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UserName)));
            }
        }
    }
}