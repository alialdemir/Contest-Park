using ContestPark.Post.API.Infrastructure.Repositories.Comment;
using ContestPark.Post.API.Models;
using ContestPark.Post.API.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;

namespace ContestPark.Post.API.Controllers
{
    [Route("api/v1/Post")]
    public class CommentController : Core.Controllers.ControllerBase
    {
        #region Private Variables

        private readonly ICommentRepository _commentRepository;

        #endregion Private Variables

        #region Constructor

        public CommentController(ILogger<CommentController> logger,
                                 ICommentRepository commentRepository) : base(logger)
        {
            _commentRepository = commentRepository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Posta yorum yap
        /// </summary>
        /// <param name="postId">Yorum yapılan post id</param>
        /// <param name="comment">Yorum</param>
        /// <returns>Başarılı ise OK değilse BadRequest mesajı döndürür.</returns>
        [HttpPost("{postId}/Comment")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddCommentAsync([FromRoute]int postId, [FromBody]CommentTextModel commentModel)
        {
            if (postId <= 0 || string.IsNullOrEmpty(commentModel.Comment))
                return BadRequest();

            bool isSuccess = await _commentRepository.AddCommentAsync(new Models.CommentModel
            {
                PostId = postId,
                UserId = UserId,
                Text = commentModel.Comment
            });
            if (!isSuccess)
            {
                Logger.LogCritical("CRITICAL: Posta yorum eklenirken hata oluştu.", postId, commentModel.Comment, UserId);

                return BadRequest(PostResource.AddingCommentFailed);
            }

            return Ok();
        }

        #endregion Methods
    }
}
