using ContestPark.Mobile.Helpers;
using System;

namespace ContestPark.Mobile.Models.Chat
{
    public class ChatModel : BaseModel
    {
        private string _senderUserId;

        public string SenderUserId
        {
            get
            {
                return _senderUserId;
            }
            set
            {
                _senderUserId = value;
                RaisePropertyChanged(() => SenderUserId);
            }
        }

        private string _userProfilePicturePath = DefaultImages.DefaultProfilePicture;

        public string UserProfilePicturePath
        {
            get { return _userProfilePicturePath; }
            set
            {
                if (value != null)
                {
                    _userProfilePicturePath = value;
                    RaisePropertyChanged(() => UserProfilePicturePath);
                }
            }
        }

        private string _message;

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                RaisePropertyChanged(() => Message);
            }
        }

        private DateTime _date;

        public DateTime Date
        {
            get { return _date; }
            set
            {
                _date = value;
                RaisePropertyChanged(() => Date);
            }
        }

        private string _userFullName;

        public string UserFullName
        {
            get { return _userFullName; }
            set
            {
                _userFullName = value;
                RaisePropertyChanged(() => UserFullName);
            }
        }

        private bool _visibilityStatus = false;

        public bool VisibilityStatus
        {
            get { return _visibilityStatus; }
            set
            {
                _visibilityStatus = value;
                RaisePropertyChanged(() => VisibilityStatus);
            }
        }

        private string userName;

        public string UserName
        {
            get { return userName; }
            set
            {
                userName = value;
                RaisePropertyChanged(() => UserName);
            }
        }

        public long ConversationId { get; set; }
    }
}
