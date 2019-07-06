using ContestPark.Core.Database.Interfaces;
using ContestPark.Core.Database.Models;
using ContestPark.EventBus.Abstractions;
using ContestPark.Post.API.Infrastructure.Documents;
using ContestPark.Post.API.Infrastructure.Repositories.Like;
using ContestPark.Post.API.Infrastructure.Repositories.Post;
using ContestPark.Post.API.IntegrationEvents.Events;
using ContestPark.Post.API.Models;
using ContestPark.Post.API.Models.Post;
using ContestPark.Post.API.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ContestPark.Post.API.Controllers
{
    public class PostController : Core.Controllers.ControllerBase
    {
        #region Private Variables

        private readonly ILikeRepository _likeRepository;
        private readonly IEventBus _eventBus;
        private readonly IPostRepository _postRepository;
        private readonly IRepository<User> _userRepository;

        #endregion Private Variables

        #region Constructor

        public PostController(ILogger<PostController> logger,
                              IEventBus eventBus,
                              IPostRepository postRepository,
                              IRepository<User> userRepository,
                              ILikeRepository likeRepository) : base(logger)
        {
            _eventBus = eventBus;
            _postRepository = postRepository;
            _userRepository = userRepository;
            _likeRepository = likeRepository;
        }

        #endregion Constructor

        #region Methods

        [HttpGet("subcategory/{subcategoryId}")]
        [ProducesResponseType(typeof(ServiceModel<PostModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult GetPostsBySubcategoryId(string subcategoryId, [FromQuery]PagingModel pagingModel)
        {
            if (string.IsNullOrEmpty(subcategoryId))
                return BadRequest();

            ServiceModel<PostModel> posts = _postRepository.GetPostsBySubcategoryId(subcategoryId, pagingModel);
            if (posts.Items.Count() == 0)
                return NotFound();

            IEnumerable<string> postUserIds = posts
                                                .Items
                                                .Select(x => x.UserIds.Distinct())
                                                .FirstOrDefault();

            IEnumerable<User> postUsers = _userRepository.FindByIds(postUserIds);

            // postları beğenen kullanıcılar
            List<CheckLikeModel> likedPosts = _likeRepository.CheckLikeStatus(posts.Items.Select(p => p.PostId).ToArray(), UserId).ToList();

            return Ok(GetPostModel(posts, postUserIds, postUsers, likedPosts));
        }

        /// <summary>
        /// Postu beğen
        /// </summary>
        /// <param name="postId">Post id</param>
        /// <returns>Başarılı ise OK değilse BadRequest mesajı döndürür.</returns>
        [HttpPost("{postId}/Like")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationResult), (int)HttpStatusCode.BadRequest)]
        public IActionResult PostLike([FromRoute]string postId)
        {
            if (string.IsNullOrEmpty(postId))
                return BadRequest();

            if (_likeRepository.CheckLikeStatus(UserId, postId))
                return BadRequest(PostResource.YouAlreadyLikedThisPost);

            var @event = new PostLikeIntegrationEvent(UserId, postId);
            _eventBus.Publish(@event);

            return Ok();
        }

        /// <summary>
        /// Post beğenmekten vazgeç
        /// </summary>
        /// <param name="postId">Post id</param>
        /// <returns>Başarılı ise OK değilse BadRequest mesajı döndürür.</returns>
        [HttpDelete("{postId}/UnLike")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationResult), (int)HttpStatusCode.BadRequest)]
        public IActionResult DeleteUnLike([FromRoute]string postId)
        {
            if (string.IsNullOrEmpty(postId))
                return BadRequest();

            if (!_likeRepository.CheckLikeStatus(UserId, postId))
                return BadRequest(PostResource.YouHaveToLikeThisPostBeforeToRemoveLiking);

            var @event = new PostUnLikeIntegrationEvent(UserId, postId);
            _eventBus.Publish(@event);

            return Ok();
        }

        /// <summary>
        /// Postu beğenen kullancı listesi
        /// </summary>
        /// <param name="subcategoryId">Post id</param>
        /// <param name="pagingModel">Sayfalama</param>
        /// <returns>Postu beğenenler</returns>
        [HttpGet("{postId}/Like")]
        [ProducesResponseType(typeof(ServiceModel<PostLikeModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult GetPostLikes(string postId, [FromQuery]PagingModel pagingModel)
        {
            if (string.IsNullOrEmpty(postId))
                return BadRequest();

            ServiceModel<string> postLikes = _likeRepository.PostLikes(postId, pagingModel);
            if (postLikes.Items.Count() == 0)
                return NotFound();

            IEnumerable<User> likedUsers = _userRepository.FindByIds(postLikes.Items);

            return Ok(GetPostLikeModel(postLikes, likedUsers, pagingModel));
        }

        private ServiceModel<PostLikeModel> GetPostLikeModel(ServiceModel<string> postLikes,
                                                             IEnumerable<User> likedUsers,
                                                             PagingModel pagingModel)
        {
            CheckUserIdsRegistred(postLikes.Items.ToList(), likedUsers.Select(x => x.Id).ToList());

            return new ServiceModel<PostLikeModel>
            {
                HasNextPage = postLikes.HasNextPage,
                PageNumber = pagingModel.PageNumber,
                PageSize = pagingModel.PageSize,
                Items = from follower in postLikes.Items
                        join followingUser in likedUsers on follower equals followingUser.Id
                        select new PostLikeModel
                        {
                            UserId = follower,
                            FullName = followingUser.FullName,
                            ProfilePicturePath = followingUser.ProfilePicturePath,
                            UserName = followingUser.UserName,
                        }
            };
        }

        /// <summary>
        /// Post verilerini birleştirme
        /// </summary>
        /// <param name="posts"></param>
        /// <param name="postUserIds"></param>
        /// <param name="postUsers"></param>
        /// <param name="likedPosts"></param>
        /// <returns></returns>
        private ServiceModel<PostModel> GetPostModel(ServiceModel<PostModel> posts,
                                                     IEnumerable<string> postUserIds,
                                                     IEnumerable<User> postUsers,
                                                     List<CheckLikeModel> likedPosts)
        {
            CheckUserIdsRegistred(postUserIds.ToList(), postUsers.Select(x => x.Id).ToList());

            return new ServiceModel<PostModel>
            {
                HasNextPage = posts.HasNextPage,
                PageNumber = posts.PageNumber,
                PageSize = posts.PageSize,
                Items = from post in posts.Items
                        join ownerUser in postUsers on post.OwnerUserId equals ownerUser.Id
                        join founderUser in postUsers on post.FounderUserId equals founderUser.Id
                        join competitorUser in postUsers on post.CompetitorUserId equals competitorUser.Id
                        select new PostModel
                        {
                            // Post bilgileri
                            Date = post.Date,
                            CommentCount = post.CommentCount,
                            LikeCount = post.LikeCount,
                            IsLike = likedPosts.Any(x => x.PostId == post.PostId && x.UserId == UserId),
                            PostId = post.PostId,
                            PostType = post.PostType,

                            // Düello bilgileri
                            Bet = post.Bet,
                            SubCategoryId = post.SubCategoryId,
                            Description = post.Description,
                            DuelId = post.DuelId,

                            // TODO:  subcategory resmi ve adı category servisinden alınmalı
                            SubCategoryName = post.SubCategoryName,
                            SubCategoryPicturePath = post.SubCategoryPicturePath,

                            // Post resim paylaşımı ise
                            PicturePath = post.PicturePath,

                            // Postun sahibi eğer düello ise kurucu postun sahibi olur
                            OwnerUserId = ownerUser.Id,
                            OwnerFullName = ownerUser.FullName,
                            OwnerProfilePicturePath = ownerUser.ProfilePicturePath,
                            OwnerUserName = ownerUser.UserName,

                            // Düelloyu kuran
                            FounderFullName = founderUser.FullName,
                            FounderProfilePicturePath = founderUser.ProfilePicturePath,
                            FounderUserName = founderUser.FullName,
                            FounderUserId = post.FounderUserId,
                            FounderTrueAnswerCount = post.FounderTrueAnswerCount,

                            // Rakip
                            CompetitorFullName = competitorUser.FullName,
                            CompetitorProfilePicturePath = competitorUser.ProfilePicturePath,
                            CompetitorTrueAnswerCount = post.CompetitorTrueAnswerCount,
                            CompetitorUserId = post.CompetitorUserId,
                            CompetitorUserName = competitorUser.UserName
                        }
            };
        }

        /// <summary>
        /// List 1 içinde olmayan list 2 user idlerini  identity apiden alması için event yayınlar
        /// </summary>
        /// <param name="userIds1">Bu serviste olan kullanıcılar</param>
        /// <param name="userIds2">Bu servisin user tablosunda olan kullanıcılar</param>
        private void CheckUserIdsRegistred(List<string> userIds1, List<string> userIds2)
        {
            Task.Factory.StartNew(() =>
            {
                // User tablosunda olmayan kullanıcıları identity den istemek için event publish ettik
                var notFoundUserIds = userIds1.Where(u => !userIds2.Any(x => x == u)).AsEnumerable();
                if (notFoundUserIds.Count() > 0)
                {
                    var @event = new UserNotFoundIntegrationEvent(notFoundUserIds);
                    _eventBus.Publish(@event);
                }
            });
        }

        #endregion Methods
    }
}
