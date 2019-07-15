using ContestPark.Core.Database.Models;
using ContestPark.Post.API.Models.Post;

namespace ContestPark.Post.API.Models
{
    public class PostDetailModel
    {
        public PostModel Post { get; set; }

        public ServiceModel<PostCommentModel> Comments { get; set; }
    }
}
