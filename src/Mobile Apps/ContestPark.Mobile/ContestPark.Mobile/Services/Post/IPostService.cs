using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.Post;
using ContestPark.Mobile.Models.Post.PostLikes;
using ContestPark.Mobile.Models.ServiceModel;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Post
{
    public interface IPostService
    {
        Task<bool> DisLikeAsync(int postId);

        Task<PostDetailModel> GetPostByPostIdAsync(int postId, PagingModel pagingModel);

        Task<ServiceModel<PostModel>> GetPostsBySubCategoryIdAsync(short subCategoryId, PagingModel pagingModel, bool isForceCache = false);

        Task<ServiceModel<PostModel>> GetPostsByUserIdAsync(string userId, PagingModel pagingModel, bool isForceCache = false);

        Task<bool> LikeAsync(int postId);

        Task<ServiceModel<PostLikeModel>> PostLikesAsync(int postId, PagingModel pagingModel);

        Task<bool> SendCommentAsync(int postId, string comment);
    }
}
