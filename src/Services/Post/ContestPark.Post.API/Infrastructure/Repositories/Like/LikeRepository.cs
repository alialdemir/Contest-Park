using ContestPark.Core.CosmosDb.Extensions;
using ContestPark.Core.CosmosDb.Interfaces;
using ContestPark.Core.CosmosDb.Models;
using ContestPark.Post.API.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
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
            return CheckLikeStatus(postId.Split(""), userId)?.Count() == 1;
        }

        /// <summary>
        /// Kullanıcılar postları beğenmişmi kontrol eder
        /// </summary>
        /// <param name="postIds">Post id</param>
        /// <param name="userIds">Kullanıcı idleri</param>
        /// <returns>Beğenme etme durumları</returns>
        public IEnumerable<CheckLikeModel> CheckLikeStatus(string[] postIds, string userId)
        {
            string sql = @"SELECT
                            c.UserId,
                            c.PostId
                           FROM c
                           WHERE c.c.UserId=@userId AND ARRAY_CONTAINS(@postIds, c.PostId)";
            return _likeRepository.QueryMultiple<CheckLikeModel>(sql,
                     new
                     {
                         postIds,
                         userId
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

        /// <summary>
        /// Post id'ye göre postu beğenen kullanıcı idlerini verir
        /// </summary>
        /// <param name="postId">Post id</param>
        /// <param name="paging">Sayfalama</param>
        /// <returns>Postu beğenen kullanıcı Listesi</returns>
        public ServiceModel<string> PostLikes(string postId, PagingModel paging)
        {
            string sql = "SELECT VALUE c.UserId FROM c WHERE c.PostId=@postId";
            return _likeRepository.ToServiceModel<Documents.Like, string>(sql, new
            {
                postId,
            }, paging);
        }

        #endregion Methods
    }
}
