using ContestPark.Core.Database.Models;
using ContestPark.Post.API.Models.Post;
using System.Threading.Tasks;

namespace ContestPark.Post.API.Infrastructure.MySql.Post
{
    public interface IPostRepository
    {
        Task<bool> AddPost(Tables.Post.Post post);

        ServiceModel<PostModel> GetPostsBySubcategoryId(string userId, int subCategoryId, PagingModel paging);
    }
}
