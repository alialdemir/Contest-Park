using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Base;
using Newtonsoft.Json;
using System.ComponentModel;

namespace ContestPark.Mobile.Models.Blocking
{
    public class UserBlocking : IModelBase, INotifyPropertyChanged
    {
        private string _userId;

        public string UserId
        {
            get { return _userId; }
            set
            {
                _userId = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UserId)));
            }
        }

        private string _fullName;

        public string FullName
        {
            get { return _fullName; }
            set
            {
                _fullName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FullName)));
            }
        }


        private string _userName;

        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UserName)));
            }
        }


        private bool _isBlocked;

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

        public event PropertyChangedEventHandler PropertyChanged;
    }
}