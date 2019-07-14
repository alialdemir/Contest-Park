using ContestPark.Core.Database.Models;
using ContestPark.Core.Models;
using ContestPark.Core.Services.Identity;
using ContestPark.EventBus.Abstractions;
using ContestPark.Post.API.Infrastructure.MySql.Post;
using ContestPark.Post.API.Infrastructure.Repositories.Like;
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
        private readonly IIdentityService _identityService;

        #endregion Private Variables

        #region Constructor

        public PostController(ILogger<PostController> logger,
                              IEventBus eventBus,
                              IPostRepository postRepository,
                              IIdentityService identityService,
                              ILikeRepository likeRepository) : base(logger)
        {
            _eventBus = eventBus;
            _postRepository = postRepository;
            _identityService = identityService;
            _likeRepository = likeRepository;
        }

        #endregion Constructor

        #region Methods

        [HttpGet("subcategory/{subcategoryId}")]
        [ProducesResponseType(typeof(ServiceModel<PostModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetPostsBySubcategoryIdAsync(int subcategoryId, [FromQuery]PagingModel pagingModel)
        {
            if (subcategoryId <= 0)
                return BadRequest();

            ServiceModel<PostModel> posts = _postRepository.GetPostsBySubcategoryId(UserId, subcategoryId, pagingModel);
            if (posts.Items.Count() == 0)
                return NotFound();

            IEnumerable<string> postUserIds = posts
                                                .Items
                                                .Select(x => x.UserIds.Distinct())
                                                .FirstOrDefault();

            IEnumerable<UserModel> postUsers = await _identityService.GetUserInfosAsync(postUserIds);

            // postları beğenen kullanıcılar

            return Ok(GetPostModel(posts, postUserIds, postUsers));
        }

        /// <summary>
        /// Postu beğen
        /// </summary>
        /// <param name="postId">Post id</param>
        /// <returns>Başarılı ise OK değilse BadRequest mesajı döndürür.</returns>
        [HttpPost("{postId}/Like")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationResult), (int)HttpStatusCode.BadRequest)]
        public IActionResult PostLike([FromRoute]int postId)
        {
            if (postId <= 0)
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
        public IActionResult DeleteUnLike([FromRoute]int postId)
        {
            if (postId <= 0)
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
        public async Task<IActionResult> GetPostLikesAsync([FromRoute]int postId, [FromQuery]PagingModel pagingModel)
        {
            if (postId <= 0)
                return BadRequest();

            ServiceModel<string> postLikes = _likeRepository.PostLikes(postId, pagingModel);
            if (postLikes.Items.Count() == 0)
                return NotFound();

            IEnumerable<UserModel> likedUsers = await _identityService.GetUserInfosAsync(postLikes.Items);

            return Ok(GetPostLikeModel(postLikes, likedUsers, pagingModel));
        }

        private ServiceModel<PostLikeModel> GetPostLikeModel(ServiceModel<string> postLikes,
                                                             IEnumerable<UserModel> likedUsers,
                                                             PagingModel pagingModel)
        {
            return new ServiceModel<PostLikeModel>
            {
                HasNextPage = postLikes.HasNextPage,
                PageNumber = pagingModel.PageNumber,
                PageSize = pagingModel.PageSize,
                Items = from follower in postLikes.Items
                        join followingUser in likedUsers on follower equals followingUser.UserId
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
                                                     IEnumerable<UserModel> postUsers)
        {
            return new ServiceModel<PostModel>
            {
                HasNextPage = posts.HasNextPage,
                PageNumber = posts.PageNumber,
                PageSize = posts.PageSize,
                Items = from post in posts.Items
                        join ownerUser in postUsers on post.OwnerUserId equals ownerUser.UserId
                        join founderUser in postUsers on post.FounderUserId equals founderUser.UserId
                        join competitorUser in postUsers on post.CompetitorUserId equals competitorUser.UserId
                        select new PostModel
                        {
                            // Post bilgileri
                            Date = post.Date,
                            CommentCount = post.CommentCount,
                            LikeCount = post.LikeCount,
                            IsLike = post.IsLike,
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
                            OwnerUserId = ownerUser.UserId,
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

        #endregion Methods
    }
}
