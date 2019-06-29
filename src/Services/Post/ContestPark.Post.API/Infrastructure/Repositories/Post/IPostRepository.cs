using System.Threading.Tasks;

namespace ContestPark.Post.API.Infrastructure.Repositories.Post
{
    public interface IPostRepository
    {
        Task<bool> AddPost(Documents.Post post);
        Core.CosmosDb.Models.ServiceModel<Models.Post.PostModel> GetPostsBySubcategoryId(string subCategoryId, Core.CosmosDb.Models.PagingModel paging);
    }
}
