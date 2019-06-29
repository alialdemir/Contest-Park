using System.Threading.Tasks;

namespace ContestPark.Post.API.Infrastructure.Repositories.Like
{
    public interface ILikeRepository
    {
        bool CheckLikeStatus(string userId, string postId);
        Task<bool> LikeAsync(string userId, string postId);
        Core.CosmosDb.Models.ServiceModel<string> PostLikes(string postId, Core.CosmosDb.Models.PagingModel paging);
        Task<bool> UnLikeAsync(string userId, string postId);
    }
}
