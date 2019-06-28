using System.Threading.Tasks;

namespace ContestPark.Post.API.Infrastructure.Repositories.PostRepository
{
    public interface IPostRepository
    {
        Task<bool> AddPost(Documents.Post post);
    }
}