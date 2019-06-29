using ContestPark.Core.Database.Models;
using System.Threading.Tasks;

namespace ContestPark.Post.API.Infrastructure.Repositories.Post
{
    public interface IPostRepository
    {
        Task<bool> AddPost(Documents.Post post);
        ServiceModel<Models.Post.PostModel> GetPostsBySubcategoryId(string subCategoryId, PagingModel paging);
    }
}
