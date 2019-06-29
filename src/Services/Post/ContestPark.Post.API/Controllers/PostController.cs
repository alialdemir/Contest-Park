using ContestPark.EventBus.Abstractions;
using ContestPark.Post.API.Infrastructure.Repositories.Like;
using ContestPark.Post.API.IntegrationEvents.Events;
using ContestPark.Post.API.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace ContestPark.Post.API.Controllers
{
    public class PostController : Core.Controllers.ControllerBase
    {
        #region Private Variables

        private readonly ILikeRepository _likeRepository;
        private readonly IEventBus _eventBus;

        #endregion Private Variables

        #region Constructor

        public PostController(ILogger<PostController> logger,
                              IEventBus eventBus,
                              ILikeRepository likeRepository) : base(logger)
        {
            _eventBus = eventBus;
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

        #endregion Methods
    }
}
