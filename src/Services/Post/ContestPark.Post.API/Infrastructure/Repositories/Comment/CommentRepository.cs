using ContestPark.Core.Database.Interfaces;
using ContestPark.Core.Database.Models;
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

        /// <summary>
        /// Post id'ye ait yorumlar
        /// </summary>
        /// <param name="postId">Post id</param>
        /// <returns>Yorum listesi</returns>
        public ServiceModel<PostCommentModel> GetCommentByPostId(int postId, PagingModel pagingModel)
        {
            string sql = @"SELECT c.TEXT AS Comment, c.UserId, c.CreatedDate AS Date
                           FROM Comments c
                           WHERE c.PostId = @postId
                           ORDER BY c.CreatedDate DESC";

            return _commentRepository.ToServiceModel<PostCommentModel>(sql, new
            {
                postId
            }, pagingModel: pagingModel);
        }

        #endregion Methods
    }
}
