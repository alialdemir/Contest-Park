﻿using ContestPark.Admin.API.Enums;
using ContestPark.Admin.API.IntegrationEvents.Events;
using ContestPark.Admin.API.Model.Question;
using ContestPark.Admin.API.Services.QuestionService;
using ContestPark.EventBus.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Net;

namespace ContestPark.Duel.API.Controllers
{
    [Authorize(Policy = "AdminPolicy")]
    public class QuestionController : Core.Controllers.ControllerBase
    {
        #region Private Variables

        private readonly IQuestionService _questionService;

        private readonly IEventBus _eventBus;

        #endregion Private Variables

        #region Constructor

        public QuestionController(ILogger<QuestionController> logger,
                                  IQuestionService questionService,
                                  IEventBus eventBus) : base(logger)
        {
            _questionService = questionService;
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
        /// <param name="configModel">Soru ayarları</param>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [Consumes("multipart/form-data")]
        public IActionResult AddQuestion([FromForm]QuestionConfigModel configModel)// Oyunucunun karşısına rakip ekler
        {
            if (configModel == null
                || configModel.SubCategoryId <= 0
                || configModel.File == null
                || configModel.File.Length == 0
                || string.IsNullOrEmpty(configModel.Question)
                || string.IsNullOrEmpty(configModel.AnswerKey))
                return BadRequest();

            Logger.LogInformation("Question size", configModel.File.Length, configModel.File.ContentDisposition);

            var questions = _questionService.GenerateQuestion(configModel);
            if (questions == null || !questions.Any())
            {
                Logger.LogError("Json ile soru oluşturulurken sorular boş geldi");

                return BadRequest();
            }

            var @eventQuestion = new CreateQuestionIntegrationEvent(questions);

            _eventBus.Publish(@eventQuestion);
            //var @event = new QuestionConfigIntegrationEvent(configModel);

            //_eventBus.Publish(@event);

            return Ok();
        }

        #endregion Methods
    }
}
