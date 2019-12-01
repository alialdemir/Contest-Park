using ContestPark.Admin.API.Enums;
using ContestPark.Admin.API.IntegrationEvents.Events;
using ContestPark.Admin.API.Model.Question;
using ContestPark.EventBus.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net;

namespace ContestPark.Duel.API.Controllers
{
    [Authorize(Policy = "AdminPolicy")]
    public class QuestionController : Core.Controllers.ControllerBase
    {
        #region Private Variables

        private readonly IEventBus _eventBus;

        #endregion Private Variables

        #region Constructor

        public QuestionController(ILogger<QuestionController> logger,
                                  IEventBus eventBus) : base(logger)
        {
            _eventBus = eventBus;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Spotify id'ye ait şarkı veya playlistlerden sorular oluşturur
        /// </summary>
        /// <param name="subCategoryId">Alt kategori id</param>
        /// <param name="spotifyId">Spotify playlist id veya spotify artis id</param>
        /// <param name="spotifyQuestionType">Playlist veya artis soruları</param>
        [HttpPost("Spotify/{spotifyId}")]
        public IActionResult Spotify([FromQuery]short subCategoryId, [FromRoute]string spotifyId, [FromQuery]SpotifyQuestionTypes spotifyQuestionType)
        {
            if (string.IsNullOrEmpty(spotifyId) || subCategoryId <= 0)
                return BadRequest();

            var @event = new CreateSpotifyQuestionIntegrationEvent(subCategoryId, spotifyId, spotifyQuestionType);

            _eventBus.Publish(@event);

            return Ok();
        }

        /// <summary>
        /// Oyuna yeni sorular ekler
        /// </summary>
        /// <param name="questions">Sorular</param>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult AddQuestion([FromBody]List<QuestionSaveModel> questions)// Oyunucunun karşısına rakip ekler
        {
            if (questions == null || questions.Count == 0)
                return BadRequest();

            var @event = new CreateQuestionIntegrationEvent(questions);

            _eventBus.Publish(@event);

            return Ok();
        }

        #endregion Methods
    }
}
