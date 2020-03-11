using ContestPark.EventBus.Abstractions;
using ContestPark.Post.API.Enums;
using ContestPark.Post.API.Infrastructure.Repositories.Comment;
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
    [Route("api/v1/Post")]
    public class CommentController : Core.Controllers.ControllerBase
    {
        #region Private Variables

        private readonly ICommentRepository _commentRepository;
        private readonly IEventBus _eventBus;

        #endregion Private Variables

        #region Constructor

        public CommentController(ILogger<CommentController> logger,
                                 IEventBus eventBus,
                                 ICommentRepository commentRepository) : base(logger)
        {
            _eventBus = eventBus;
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

            AddPostCommentNotificatiın(postId);

            return Ok();
        }

        /// <summary>
        /// Posta yorum yapanları ve post sahiplerine bildirim gönderir
        /// </summary>
        /// <param name="postId">Bildirim gönderilecek post id</param>
        private void AddPostCommentNotificatiın(int postId)
        {
            IEnumerable<PostCommentedModel> postCommentedUserIds = _commentRepository.PostCommented(postId);
            if (postCommentedUserIds == null || !postCommentedUserIds.Any())
                return;

            IEnumerable<string> userIds = GetPostCommentedUserIds(postCommentedUserIds);

            // Postu beğendiği bildirimi post ile ilişkisi olanlara gönderildi
            var @eventNotification = new AddNotificationIntegrationEvent(UserId,
                                                                         userIds,
                                                                         NotificationTypes.PostComment,
                                                                         postId,
                                                                         postCommentedUserIds.FirstOrDefault().PicturePath);
            _eventBus.Publish(@eventNotification);
        }

        /// <summary>
        /// Posta yorum yapanları ve post sahiplerinin kullanıcı idleri
        /// </summary>
        /// <param name="postCommenteds"></param>
        /// <returns>Kullanıcı idleri</returns>
        private IEnumerable<string> GetPostCommentedUserIds(IEnumerable<PostCommentedModel> postCommenteds)
        {
            if (postCommenteds == null || !postCommenteds.Any())
                return null;

            List<string> commentedUserIds = new List<string>();

            // Burada aynı user id iki defa dönmesin diye kontrol koydum
            foreach (var postCommented in postCommenteds)
            {
                if (postCommented.UserId != postCommented.OwnerUserId
                    && !string.IsNullOrEmpty(postCommented.OwnerUserId))
                {
                    commentedUserIds.Add(postCommented.OwnerUserId);
                }

                if (postCommented.UserId != postCommented.CompetitorUserId
                    && !string.IsNullOrEmpty(postCommented.CompetitorUserId))
                {
                    commentedUserIds.Add(postCommented.CompetitorUserId);
                }

                if (postCommented.UserId != postCommented.OwnerUserId
                    && postCommented.UserId != postCommented.CompetitorUserId
                    && !string.IsNullOrEmpty(postCommented.UserId))
                {
                    commentedUserIds.Add(postCommented.UserId);
                }
            }

            return commentedUserIds;
        }

        #endregion Methods
    }
}
