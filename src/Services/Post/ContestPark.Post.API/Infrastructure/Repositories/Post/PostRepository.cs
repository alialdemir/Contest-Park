using ContestPark.Core.Database.Interfaces;
using ContestPark.Core.Database.Models;
using ContestPark.Core.Enums;
using ContestPark.Post.API.Infrastructure.MySql.Post;
using ContestPark.Post.API.Models;
using ContestPark.Post.API.Models.Post;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Post.API.Infrastructure.MySql
{
    public class PostRepository : IPostRepository
    {
        #region Private Variables

        private readonly IRepository<Tables.Post.Post> _postRepository;

        #endregion Private Variables

        #region Constructor

        public PostRepository(IRepository<Tables.Post.Post> postRepository)
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
        public async Task<bool> AddPost(Tables.Post.Post post)
        {
            int? postId = await _postRepository.AddAsync(post);

            return postId.HasValue;
        }

        /// <summary>
        /// Post id ait post
        /// </summary>
        /// <param name="userId">Login olan kullanıcı id</param>
        /// <param name="postId">Post id</param>
        /// <returns>Post nesnesi</returns>
        public PostModel GetPostDetailByPostId(string userId, int postId, Languages language)
        {
            var result = _postRepository.ToSpServiceModel<PostModel>("SP_GetPostDetailByPostId", new
            {
                userId,
                postId,
                language = (byte)language
            });

            return result.Items.FirstOrDefault();
        }

        /// <summary>
        /// Subcategory id'ye ait olan postları getirir
        /// </summary>
        /// <param name="userId">Login olan kullanıcı id</param>
        /// <param name="subCategoryId">Subcategory id</param>
        /// <param name="paging">Sayfalama</param>
        /// <returns>Subcategory posts</returns>
        public ServiceModel<PostModel> GetPostsBySubcategoryId(string userId, int subCategoryId, Languages language, PagingModel paging)
        {
            return _postRepository.ToSpServiceModel<PostModel>("SP_GetPostsBySubcategoryId", new
            {
                userId,
                subCategoryId,
                language = (byte)language
            }, pagingModel: paging);
        }

        /// <summary>
        /// Kullanıcının postlarını döndürür
        /// </summary>
        /// <param name="profileUserId">Postları alınacak kullanıcı id</param>
        /// <param name="userId">Login olan kullanıcı id</param>
        /// <param name="language">Hangi dilde dönecek</param>
        /// <param name="paging">Sayfalama</param>
        /// <returns>Post listesi</returns>
        public ServiceModel<PostModel> GetPostByUserId(string profileUserId, string userId, Languages language, PagingModel paging)
        {
            return _postRepository.ToSpServiceModel<PostModel>("SP_GetPostByUserId", new
            {
                userId,
                profileUserId,
                language = (byte)language
            }, pagingModel: paging);
        }

        /// <summary>
        /// Post id'sine ait owner ve rakip kullanıcı id'lerini verir
        /// </summary>
        /// <param name="postId">Post id</param>
        /// <returns>Owner ve rakip kullanıcı id ayrıca resim varsa onu verir</returns>
        public PostOwnerModel GetPostOwnerInfo(int postId)
        {
            string sql = @"SELECT
                           p.OwnerUserId,
                           p.CompetitorUserId,
                           p.PicturePath
                           FROM Posts p
                           WHERE p.PostId = @postId";

            return _postRepository.QuerySingleOrDefault<PostOwnerModel>(sql, new
            {
                postId
            });
        }

        /// <summary>
        /// Post arşivleme
        /// </summary>
        /// <param name="userId">Postun sahibi olan kullanıcı id</param>
        /// <param name="postId">Post id</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public Task<bool> ArchiveAsync(string userId, int postId)
        {
            Tables.Post.Post post = _postRepository.FindById(postId);
            if (post == null || post.OwnerUserId != userId)
                return Task.FromResult(false);

            post.IsArchive = true;

            return _postRepository.UpdateAsync(post);
        }

        /// <summary>
        /// Posta yorum yapılmasını kapatır
        /// </summary>
        /// <param name="userId">Postun sahibi olan kullanıcı id</param>
        /// <param name="postId">Post id</param>
        /// <returns>Başarılı ise true değilsa falase</returns>
        public Task<bool> TurnOffToggleCommentAsync(string userId, int postId)
        {
            Tables.Post.Post post = _postRepository.FindById(postId);
            if (post == null || post.OwnerUserId != userId)
                return Task.FromResult(false);

            post.IsCommentOpen = !post.IsCommentOpen;

            return _postRepository.UpdateAsync(post);
        }

        #endregion Methods
    }
}
