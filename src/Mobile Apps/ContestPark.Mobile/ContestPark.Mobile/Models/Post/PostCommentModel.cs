using System;

namespace ContestPark.Mobile.Models.Post
{
    public class PostCommentModel : BaseModel
    {
        public string Comment { get; set; }
        public DateTime Date { get; set; }
        public string FullName { get; set; }
        public string ProfilePicturePath { get; set; }
        public string UserName { get; set; }
    }
}