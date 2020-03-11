namespace ContestPark.Post.API.Models
{
    public class PostCommentedModel
    {
        public string UserId { get; set; }
        public string OwnerUserId { get; set; }
        public string CompetitorUserId { get; set; }
        public string PicturePath { get; set; }
    }
}
