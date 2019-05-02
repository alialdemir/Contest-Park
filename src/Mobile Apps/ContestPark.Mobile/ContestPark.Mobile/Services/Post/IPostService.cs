using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.Post;
using ContestPark.Mobile.Models.ServiceModel;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Post
{
    public interface IPostService
    {
        Task<bool> DisLikeAsync(string postId);

        Task<bool> LikeAsync(string postId);

        Task<ServiceModel<PostModel>> SubCategoryPostsAsync(short subCategoryId, PagingModel pagingModel);
    }
}