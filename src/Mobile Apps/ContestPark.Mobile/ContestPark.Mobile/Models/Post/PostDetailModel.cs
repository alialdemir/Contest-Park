using ContestPark.Mobile.Models.ServiceModel;

namespace ContestPark.Mobile.Models.Post
{
    public class PostDetailModel
    {
        public ServiceModel<PostCommentModel> Comments { get; set; }
        public PostModel Post { get; set; }
    }
}