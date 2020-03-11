using ContestPark.Core.Database.Interfaces;
using ContestPark.Core.Database.Models;
using ContestPark.Post.API.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
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

        /// <summary>
        /// Posta yorum yapanları ve post sahiplerini döndürür
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        public IEnumerable<PostCommentedModel> PostCommented(int postId)
        {
            string sql = @"SELECT
                           c.UserId,
                           p.OwnerUserId,
                           p.CompetitorUserId,
                           p.PicturePath
                           FROM Posts p
                           INNER JOIN Comments c ON c.PostId = p.PostId
                           WHERE p.PostId = @postId";

            return _commentRepository.QueryMultiple<PostCommentedModel>(sql, new
            {
                postId
            });
        }

        #endregion Methods
    }
}
