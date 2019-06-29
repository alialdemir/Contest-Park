using ContestPark.Core.CosmosDb.Extensions;
using ContestPark.Core.CosmosDb.Interfaces;
using ContestPark.Core.CosmosDb.Models;
using ContestPark.Post.API.Models.Post;
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

        /// <summary>
        /// Subcategory id'ye ait olan postları getirir
        /// </summary>
        /// <param name="subCategoryId">Subcategory</param>
        /// <param name="paging">Sayfalama</param>
        /// <returns>Subcategory posts</returns>
        public ServiceModel<PostModel> GetPostsBySubcategoryId(string subCategoryId, PagingModel paging)
        {
            string sql = @"SELECT
                             c.CreatedDate AS Date,
                             c.LikeCount ?? 0 AS LikeCount,
                             c.CommentCount ?? 0 AS CommentCount,
                             c.OwnerUserId,
                             c.id AS PostId,
                             c.PostType,

                             c.Bet,
                             c.DuelId,
                             c.SubCategoryId,
                             c.CompetitorUserId,
                             c.CompetitorTrueAnswerCount,
                             c.FounderUserId,
                             c.FounderTrueAnswerCount,

                             [c.CompetitorUserId, c.OwnerUserId,  c.FounderUserId] as UserIds
                          FROM c
                          WHERE c.SubCategoryId=@subCategoryId
                          ORDER BY c.CreatedDate DESC";
            return _postRepository.ToServiceModel<Documents.Post, PostModel>(sql, new
            {
                subCategoryId,
            }, paging);
        }

        #endregion Methods
    }
}
