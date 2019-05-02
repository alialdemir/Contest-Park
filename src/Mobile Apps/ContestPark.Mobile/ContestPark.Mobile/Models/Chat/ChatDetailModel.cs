using ContestPark.Mobile.Models.Base;
using System;
using System.ComponentModel;

namespace ContestPark.Mobile.Models.Chat
{
    public class ChatDetailModel : IModelBase, INotifyPropertyChanged
    {
        private DateTime date;
        private string message;

        private string senderId;

        public event PropertyChangedEventHandler PropertyChanged;

        public DateTime Date
        {
            get { return date; }
            set
            {
                date = value;
                NotifyPropertyChanged(nameof(Date));
            }
        }

        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                NotifyPropertyChanged(nameof(Message));
            }
        }

        public string SenderId
        {
            get { return senderId; }
            set
            {
                senderId = value;
                NotifyPropertyChanged(nameof(SenderId));
            }
        }

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}