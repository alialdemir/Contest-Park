using ContestPark.Core.CosmosDb.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ContestPark.Post.API.Infrastructure.Repositories.Like
{
    public class LikeRepository : ILikeRepository
    {
        #region Private Variables

        private readonly ILogger<LikeRepository> _logger;
        private readonly IDocumentDbRepository<Documents.Post> _postRepository;
        private readonly IDocumentDbRepository<Documents.Like> _likeRepository;

        #endregion Private Variables

        #region Constructor

        public LikeRepository(IDocumentDbRepository<Documents.Post> postRepository,
                                IDocumentDbRepository<Documents.Like> likeRepository,
                                ILogger<LikeRepository> logger)
        {
            _postRepository = postRepository;
            _likeRepository = likeRepository;
            _logger = logger;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Kullanıcı ilgili postu beğendimi
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <param name="postId">Beğenilen post</param>
        /// <returns>Beğenmiş ise true beğenmemiş ise false</returns>
        public bool CheckLikeStatus(string userId, string postId)
        {
            string sql = @"SELECT VALUE true
                           FROM c
                           WHERE c.PostId=@postId AND c.UserId=@userId";
            return _likeRepository.QuerySingle<bool>(sql,
                     new
                     {
                         userId,
                         postId
                     });
        }

        /// <summary>
        /// Post beğen
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <param name="postId">Beğenilen post</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public async Task<bool> LikeAsync(string userId, string postId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(postId))
                return false;

            Documents.Post post = _postRepository.FindById(postId);
            if (post == null)
                return false;

            bool isSuccess = await _likeRepository.AddAsync(new Documents.Like
            {
                UserId = userId,
                PostId = postId
            });

            if (!isSuccess)
            {
                _logger.LogCritical("CRITICAL: beğeni işlemi gerçekleşemedi.", userId, postId);

                return false;
            }

            post.LikeCount = (post.LikeCount ?? 0) + 1;

            isSuccess = await _postRepository.UpdateAsync(post);
            if (!isSuccess)
            {
                _logger.LogCritical("CRITICAL: beğeni kayıdı eklendi ancak post beğeni sayısı güncelleme hatası.", userId, postId);
                return true;
            }

            return isSuccess;
        }

        /// <summary>
        /// Postu beğenmekten vazgeç
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <param name="postId">Post id</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public async Task<bool> UnLikeAsync(string userId, string postId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(postId))
                return false;

            Documents.Post post = _postRepository.FindById(postId);
            if (post == null)
                return false;

            bool isSuccess = await _likeRepository.RemoveAsync(x => x.UserId == userId && x.PostId == postId);
            if (!isSuccess)
            {
                _logger.LogCritical("CRITICAL: beğeni silme sırasında hata.", userId, postId);
                return false;
            }

            post.LikeCount -= 1;
            isSuccess = await _postRepository.UpdateAsync(post);
            if (!isSuccess)
            {
                _logger.LogInformation("Uyarı! beğeni documentinden beğeni kaldırıldı fakat. post documenti üzerinden likeCount azaltılamadı.", userId, postId);
            }

            return isSuccess;
        }

        #endregion Methods
    }
}
