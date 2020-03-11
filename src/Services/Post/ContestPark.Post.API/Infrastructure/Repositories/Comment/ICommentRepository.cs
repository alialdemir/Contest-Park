using ContestPark.Core.Database.Models;
using ContestPark.Post.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Post.API.Infrastructure.Repositories.Comment
{
    public interface ICommentRepository
    {
        Task<bool> AddCommentAsync(CommentModel comment);

        ServiceModel<PostCommentModel> GetCommentByPostId(int postId, PagingModel pagingModel);

        IEnumerable<PostCommentedModel> PostCommented(int postId);
    }
}
