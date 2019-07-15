using ContestPark.Post.API.Models;
using System.Threading.Tasks;

namespace ContestPark.Post.API.Infrastructure.Repositories.Comment
{
    public interface ICommentRepository
    {
        Task<bool> AddCommentAsync(CommentModel comment);
    }
}
