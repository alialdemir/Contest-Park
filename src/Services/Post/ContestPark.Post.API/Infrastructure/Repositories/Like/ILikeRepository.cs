using ContestPark.Core.Database.Models;
using System.Threading.Tasks;

namespace ContestPark.Post.API.Infrastructure.Repositories.Like
{
    public interface ILikeRepository
    {
        bool CheckLikeStatus(string userId, int postId);

        Task<bool> LikeAsync(string userId, int postId);

        ServiceModel<string> PostLikes(int postId, PagingModel paging);

        Task<bool> UnLikeAsync(string userId, int postId);
    }
}
