namespace ContestPark.Post.API.Models
{
    public class CommentModel
    {
        public string Text { get; set; }
        public string UserId { get; set; }
        public int PostId { get; set; }
    }
}
