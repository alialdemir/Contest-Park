using ContestPark.Core.Database.Models;
using ContestPark.Post.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Post.API.Infrastructure.Repositories.Like
{
    public interface ILikeRepository
    {
        bool CheckLikeStatus(string userId, string postId);

        IEnumerable<CheckLikeModel> CheckLikeStatus(string[] postIds, string userId);

        Task<bool> LikeAsync(string userId, string postId);

        ServiceModel<string> PostLikes(string postId, Core.Database.Models.PagingModel paging);

        Task<bool> UnLikeAsync(string userId, string postId);
    }
}
