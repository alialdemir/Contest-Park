using System.ComponentModel.DataAnnotations;

namespace ContestPark.Chat.API.Model
{
    public class Message
    {
        [Required]
        public string ReceiverUserId { get; set; }

        [Required]
        public string Text { get; set; }
    }
}