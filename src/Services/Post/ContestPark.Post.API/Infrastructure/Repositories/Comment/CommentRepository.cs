using ContestPark.Core.Database.Interfaces;
using ContestPark.Post.API.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ContestPark.Post.API.Infrastructure.Repositories.Comment
{
    public class CommentRepository : ICommentRepository
    {
        #region Private Variables

        private readonly ILogger<CommentRepository> _logger;
        private readonly IRepository<Tables.Comment> _commentRepository;

        #endregion Private Variables

        #region Constructor

        public CommentRepository(IRepository<Tables.Comment> commentRepository,
                                 ILogger<CommentRepository> logger)
        {
            _commentRepository = commentRepository;
            _logger = logger;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Posta Comment ekle
        /// </summary>
        /// <param name="comment">Yapılan yorum</param>
        /// <returns>İşlem başarılı ise true değilse false döner</returns>
        public Task<bool> AddCommentAsync(CommentModel comment)
        {
            return _commentRepository.ExecuteAsync("SP_AddComment", new
            {
                userId = comment.UserId,
                postId = comment.PostId,
                text = comment.Text
            }, System.Data.CommandType.StoredProcedure);
        }

        #endregion Methods
    }
}
