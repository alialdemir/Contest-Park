using System;

namespace ContestPark.Chat.API.Model
{
    public class ConversationDetailModel
    {
        public DateTime Date { get; set; }

        public string Message { get; set; }

        public string SenderId { get; set; }
        public bool IsIncoming { get; set; }
    }
}
