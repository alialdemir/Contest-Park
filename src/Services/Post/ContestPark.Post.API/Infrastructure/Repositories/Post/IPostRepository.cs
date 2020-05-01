using ContestPark.Core.Database.Models;
using ContestPark.Core.Enums;
using ContestPark.Post.API.Models;
using ContestPark.Post.API.Models.Post;
using System.Threading.Tasks;

namespace ContestPark.Post.API.Infrastructure.MySql.Post
{
    public interface IPostRepository
    {
        Task<bool> AddPost(Tables.Post.Post post);
        Task<bool> ArchiveAsync(string userId, int postId);
        ServiceModel<PostModel> GetPostByUserId(string profileUserId, string userId, Languages language, PagingModel paging);

        PostModel GetPostDetailByPostId(string userId, int postId, Languages language);
        PostOwnerModel GetPostOwnerInfo(int postId);
        ServiceModel<PostModel> GetPostsBySubcategoryId(string userId, int subCategoryId, Languages language, PagingModel paging);
        Task<bool> TurnOffToggleCommentAsync(string userId, int postId);
    }
}
