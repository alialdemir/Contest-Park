using ContestPark.Post.API.Infrastructure.Repositories.Like;
using ContestPark.Post.API.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;

namespace ContestPark.Post.API.Controllers
{
    public class PostController : Core.Controllers.ControllerBase
    {
        #region Private Variables

        private readonly ILikeRepository _likeRepository;

        #endregion Private Variables

        #region Constructor

        public PostController(ILogger<PostController> logger,
                              ILikeRepository likeRepository) : base(logger)
        {
            _likeRepository = likeRepository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Postu beğen
        /// </summary>
        /// <returns>Başarılı ise OK değilse BadRequest mesajı döndürür.</returns>
        [HttpPost("{postId}/Like")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Post([FromRoute]string postId)
        {
            if (string.IsNullOrEmpty(postId))
                return BadRequest();

            if (_likeRepository.CheckLikeStatus(UserId, postId))
                return BadRequest(PostResource.YouAlreadyLikedThisPost);

            bool isSuccess = await _likeRepository.LikeAsync(UserId, postId);
            if (!isSuccess)
                return BadRequest();

            return Ok();
        }

        #endregion Methods
    }
}
