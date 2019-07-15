using ContestPark.Core.Database.Interfaces;
using ContestPark.Core.Database.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ContestPark.Post.API.Infrastructure.Repositories.Like
{
    public class LikeRepository : ILikeRepository
    {
        #region Private Variables

        private readonly ILogger<LikeRepository> _logger;
        private readonly IRepository<Tables.Post.Post> _postRepository;
        private readonly IRepository<Tables.Like> _likeRepository;

        #endregion Private Variables

        #region Constructor

        public LikeRepository(IRepository<Tables.Post.Post> postRepository,
                              IRepository<Tables.Like> likeRepository,
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
        public bool CheckLikeStatus(string userId, int postId)
        {
            return _postRepository.QuerySingleOrDefault<bool>(@"SELECT FNC_PostIsLike(@userId, @PostId)", new
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
        public Task<bool> LikeAsync(string userId, int postId)
        {
            if (string.IsNullOrEmpty(userId) || postId <= 0)
                return Task.FromResult(false);

            return _postRepository.ExecuteAsync("SP_PostLike", new
            {
                userId,
                postId
            }, System.Data.CommandType.StoredProcedure);
        }

        /// <summary>
        /// Postu beğenmekten vazgeç
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <param name="postId">Post id</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public Task<bool> UnLikeAsync(string userId, int postId)
        {
            if (string.IsNullOrEmpty(userId) || postId <= 0)
                return Task.FromResult(false);

            return _postRepository.ExecuteAsync("SP_PostUnLike", new
            {
                userId,
                postId
            }, System.Data.CommandType.StoredProcedure);
        }

        /// <summary>
        /// Post id'ye göre postu beğenen kullanıcı idlerini verir
        /// </summary>
        /// <param name="postId">Post id</param>
        /// <param name="paging">Sayfalama</param>
        /// <returns>Postu beğenen kullanıcı Listesi</returns>
        public ServiceModel<string> PostLikes(int postId, PagingModel paging)
        {
            string sql = "SELECT l.UserId FROM Likes l WHERE l.PostId=@postId";
            return _likeRepository.ToServiceModel<string>(sql, new
            {
                postId,
            }, pagingModel: paging);
        }

        #endregion Methods
    }
}
