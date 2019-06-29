using ContestPark.Core.CosmosDb.Interfaces;
using System.Threading.Tasks;

namespace ContestPark.Post.API.Infrastructure.Repositories.Post
{
    public class PostRepository : IPostRepository
    {
        #region Private Variables

        private readonly IDocumentDbRepository<Documents.Post> _postRepository;

        #endregion Private Variables

        #region Constructor

        public PostRepository(IDocumentDbRepository<Documents.Post> postRepository)
        {
            _postRepository = postRepository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Post ekle
        /// </summary>
        /// <param name="post">Post detayı</param>
        /// <returns>İşlem başarılı ise true değilse false</returns>
        public Task<bool> AddPost(Documents.Post post)
        {
            return _postRepository.AddAsync(post);
        }

        #endregion Methods
    }
}