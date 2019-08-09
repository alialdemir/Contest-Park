using Newtonsoft.Json;
using System;

namespace ContestPark.Chat.API.Model
{
    public class MessageModel
    {
        public string SenderUserId { get; set; }
        public string Message { get; set; }

        public DateTime Date { get; set; }

        public string UserProfilePicturePath { get; set; }

        public string UserFullName { get; set; }

        public bool VisibilityStatus { get; set; }

        public string UserName { get; set; }

        public long ConversationId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string LastWriterUserId { get; set; }
    }
}
