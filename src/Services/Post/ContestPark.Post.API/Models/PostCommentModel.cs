using System;

namespace ContestPark.Post.API.Models
{
    public class PostCommentModel
    {
        public string Comment { get; set; }

        public DateTime Date { get; set; }
        public string UserId { get; set; }

        public string FullName { get; set; }

        public string ProfilePicturePath { get; set; }

        public string UserName { get; set; }
    }
}
