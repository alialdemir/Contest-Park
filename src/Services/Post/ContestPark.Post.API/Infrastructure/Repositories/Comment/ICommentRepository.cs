using ContestPark.Post.API.Models;
using System.Threading.Tasks;

namespace ContestPark.Post.API.Infrastructure.Repositories.Comment
{
    public interface ICommentRepository
    {
        Task<bool> AddCommentAsync(CommentModel comment);
        Core.Database.Models.ServiceModel<PostCommentModel> GetCommentByPostId(int postId, Core.Database.Models.PagingModel pagingModel);
    }
}
