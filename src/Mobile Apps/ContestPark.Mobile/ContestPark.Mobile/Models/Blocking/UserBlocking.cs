using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Base;
using Newtonsoft.Json;
using System.ComponentModel;

namespace ContestPark.Mobile.Models.Blocking
{
    public class UserBlocking : IModelBase, INotifyPropertyChanged
    {
        public string UserId { get; set; }

        public string FullName { get; set; }

        public string UserName { get; set; }

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