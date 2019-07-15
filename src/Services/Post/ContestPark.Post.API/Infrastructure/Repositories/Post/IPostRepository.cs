using ContestPark.Core.Database.Models;
using ContestPark.Core.Enums;
using ContestPark.Post.API.Models.Post;
using System.Threading.Tasks;

namespace ContestPark.Post.API.Infrastructure.MySql.Post
{
    public interface IPostRepository
    {
        Task<bool> AddPost(Tables.Post.Post post);

        ServiceModel<PostModel> GetPostByUserName(string profileUserId, string userId, Languages language, PagingModel paging);

        PostModel GetPostDetailByPostId(string userId, int postId, Languages language);

        ServiceModel<PostModel> GetPostsBySubcategoryId(string userId, int subCategoryId, Languages language, PagingModel paging);
    }
}
