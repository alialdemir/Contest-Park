using System;

namespace ContestPark.Mobile.Models.Chat
{
    public class ChatDetailModel : BaseModel
    {
        private DateTime date;
        private string message;

        private string senderId;

        public DateTime Date
        {
            get { return date; }
            set
            {
                date = value;

                RaisePropertyChanged(() => Date);
            }
        }

        public bool IsIncoming { get; set; }

        public string Message
        {
            get { return message; }
            set
            {
                message = value;

                RaisePropertyChanged(() => Message);
            }
        }

        public string SenderId
        {
            get { return senderId; }
            set
            {
                senderId = value;

                RaisePropertyChanged(() => SenderId);
            }
        }
    }
}
