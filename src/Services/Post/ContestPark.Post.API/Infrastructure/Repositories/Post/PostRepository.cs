using ContestPark.Core.Database.Interfaces;
using ContestPark.Core.Database.Models;
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
        /// Subcategory id'ye ait olan postları getirir
        /// </summary>
        /// <param name="subCategoryId">Subcategory</param>
        /// <param name="paging">Sayfalama</param>
        /// <returns>Subcategory posts</returns>
        public ServiceModel<PostModel> GetPostsBySubcategoryId(string userId, int subCategoryId, PagingModel paging)
        {
            string sql = @"SELECT
                             p.CreatedDate AS Date,
                             p.LikeCount,
                             p.CommentCount,
                             p.OwnerUserId,
                             p.PostId,
                             p.PostType,
                             p.Bet,
                             p.DuelId,
                             p.SubCategoryId,
                             p.CompetitorUserId,
                             p.CompetitorTrueAnswerCount,
                             p.FounderUserId,
                             p.FounderTrueAnswerCount,
                             (SELECT (CASE
                             WHEN EXISTS(
                             SELECT NULL
                             FROM Likes l WHERE l.UserId = @userId AND l.PostID = p.PostId)
                             THEN 1
                             ELSE 0
                             END)) AS IsLike
                            FROM  Posts p
                            WHERE p.SubCategoryId=@subCategoryId
                            ORDER BY p.CreatedDate DESC";

            return _postRepository.ToServiceModel<PostModel>(sql, new
            {
                userId,
                subCategoryId,
            }, pagingModel: paging);
        }

        #endregion Methods
    }
}
