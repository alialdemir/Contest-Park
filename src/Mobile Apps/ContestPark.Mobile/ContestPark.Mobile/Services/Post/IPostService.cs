using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.Post;
using ContestPark.Mobile.Models.Post.PostLikes;
using ContestPark.Mobile.Models.ServiceModel;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Post
{
    public interface IPostService
    {
        Task<bool> DisLikeAsync(string postId);

        Task<PostDetailModel> GetPostByPostIdAsync(string postId);

        Task<ServiceModel<PostModel>> GetPostsBySubCategoryIdAsync(short subCategoryId, PagingModel pagingModel);

        Task<ServiceModel<PostModel>> GetPostsByUserIdAsync(string userId, PagingModel pagingModel);

        Task<bool> LikeAsync(string postId);

        Task<ServiceModel<PostLikeModel>> PostLikesAsync(string postId, PagingModel pagingModel);

        Task<bool> SendCommentAsync(string postId, string comment);
    }
}