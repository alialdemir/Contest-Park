using ContestPark.Core.CosmosDb.Interfaces;
using ContestPark.Core.CosmosDb.Models;
using ContestPark.EventBus.Abstractions;
using ContestPark.Post.API.Infrastructure.Documents;
using ContestPark.Post.API.Infrastructure.Repositories.Like;
using ContestPark.Post.API.IntegrationEvents.Events;
using ContestPark.Post.API.Models;
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
        private readonly IDocumentDbRepository<User> _userRepository;

        #endregion Private Variables

        #region Constructor

        public PostController(ILogger<PostController> logger,
                              IEventBus eventBus,
                              IDocumentDbRepository<User> userRepository,
                              ILikeRepository likeRepository) : base(logger)
        {
            _eventBus = eventBus;
            _userRepository = userRepository;
            _likeRepository = likeRepository;
        }

        #endregion Constructor

        #region Methods

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
        /// <param name="postId">Post id</param>
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
            Task.Factory.StartNew(() =>
            {
                // User tablosunda olmayan kullanıcıları identity den istemek için event publish ettik
                var notFoundUserIds = postLikes.Items.Where(u => !likedUsers.Any(x => x.Id == u)).AsEnumerable();
                if (notFoundUserIds.Count() > 0)
                {
                    var @event = new UserNotFoundIntegrationEvent(notFoundUserIds);
                    _eventBus.Publish(@event);
                }
            });

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

        #endregion Methods
    }
}
