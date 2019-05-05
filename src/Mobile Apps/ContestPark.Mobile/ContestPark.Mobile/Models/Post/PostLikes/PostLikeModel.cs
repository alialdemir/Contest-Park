using ContestPark.Mobile.Models.Base;

namespace ContestPark.Mobile.Models.Post.PostLikes
{
    public class PostLikeModel : IModelBase
    {
        public string FullName { get; set; }
        public bool IsFollowing { get; set; }
        public string ProfilePicturePath { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}