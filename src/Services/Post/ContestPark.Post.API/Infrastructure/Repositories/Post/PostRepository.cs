using ContestPark.Core.Database.Interfaces;
using ContestPark.Core.Database.Models;
using ContestPark.Core.Enums;
using ContestPark.Post.API.Infrastructure.MySql.Post;
using ContestPark.Post.API.Models.Post;
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
        public Task<bool> AddPost(Tables.Post.Post post)
        {
            return _postRepository.AddAsync(post);
        }

        /// <summary>
        /// Post id ait post
        /// </summary>
        /// <param name="userId">Login olan kullanıcı id</param>
        /// <param name="postId">Post id</param>
        /// <returns>Post nesnesi</returns>
        public PostModel GetPostDetailByPostId(string userId, int postId, Languages language)
        {
            string sql = GetSql("WHERE p.PostId = @postId");

            return _postRepository.QuerySingleOrDefault<PostModel>(sql, new
            {
                userId,
                postId,
                language = (byte)language
            });
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
            string sql = GetSql("WHERE p.SubCategoryId = @subCategoryId");

            return _postRepository.ToServiceModel<PostModel>(sql, new
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
            string sql = GetSql("WHERE p.OwnerUserId = @profileUserId OR p.FounderUserId = @profileUserId OR p.CompetitorUserId = @profileUserId");

            return _postRepository.ToServiceModel<PostModel>(sql, new
            {
                userId,
                profileUserId,
                language = (byte)language
            }, pagingModel: paging);
        }

        /// <summary>
        /// Alt kategori detayı, profil sayfası ve post detay sayfasındaki sql sorgusu aynı sadece where koşulu değişiyor
        /// bu yüzden sorguyu tek bir yerden alıp where kısmını parametre olarak geçtik
        /// Buradaki sorguyu değiştirirseniz 3 sayfada etkilenecektir...
        /// </summary>
        /// <param name="where">Koşul</param>
        /// <returns>Sql query</returns>
        private string GetSql(string where)
        {
            string sql = $@"SELECT
                             p.CreatedDate AS Date,
                             p.LikeCount,
                             p.CommentCount,
                             p.OwnerUserId,
                             p.PostId,
                             p.PostType,
                             p.Bet,
                             scl.SubCategoryName,
                             sc.PicturePath AS SubCategoryPicturePath,
                             p.Bet,

                             p.DuelId,
                             p.SubCategoryId,
                             p.CompetitorUserId,
                             p.CompetitorTrueAnswerCount,
                             p.FounderUserId,
                             p.FounderTrueAnswerCount,
                             FNC_PostIsLike(@userId, p.PostId) AS IsLike,

                            p.PostImageType,
                            p.PicturePath,

                            p.Description

                           FROM Posts p
                           LEFT JOIN ContestParkCategory.SubCategories sc ON sc.SubCategoryId = p.SubCategoryId
                           LEFT JOIN ContestParkCategory.SubCategoryLangs scl ON scl.SubCategoryId = p.SubCategoryId AND scl.`Language`= @language
                           {where}
                           ORDER BY p.CreatedDate desc";

            return sql;
        }

        #endregion Methods
    }
}
