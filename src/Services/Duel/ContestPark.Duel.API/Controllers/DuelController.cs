using ContestPark.Duel.API.Infrastructure.Repositories.Question;
using ContestPark.Duel.API.IntegrationEvents.Events;
using ContestPark.Duel.API.Models;
using ContestPark.EventBus.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;

namespace ContestPark.Duel.API.Controllers
{
    public class DuelController : Core.Controllers.ControllerBase
    {
        private readonly IQuestionRepository questionRepository;

        #region Private Variables

        private readonly IEventBus _eventBus;

        #endregion Private Variables

        #region Constructor

        public DuelController(ILogger<DuelController> logger,
            IQuestionRepository questionRepository,
                              IEventBus eventBus) : base(logger)
        {
            this.questionRepository = questionRepository;
            _eventBus = eventBus;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Duello rakip bekleme moduna al
        /// </summary>
        /// <param name="standbyModeModel">Bekleme modu bilgileri</param>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult AddStandbyMode([FromBody]StandbyModeModel standbyModeModel)
        {
            if (standbyModeModel.Bet < 0 || standbyModeModel.SubCategoryId <= 0 || string.IsNullOrEmpty(standbyModeModel.ConnectionId))
            {
                return BadRequest();
            }

            var @event = new WaitingOpponentIntegrationEvent(UserId,
                                                             standbyModeModel.ConnectionId,
                                                             standbyModeModel.SubCategoryId,
                                                             standbyModeModel.Bet,
                                                             standbyModeModel.BalanceType,
                                                             CurrentUserLanguage);

            _eventBus.Publish(@event);

            return Ok();
        }

        /// <summary>
        /// Rakip bekleme modundan çıkar
        /// </summary>
        /// <param name="standbyModeModel">Bekleme modu bilgileri</param>
        [HttpPost("DeleteStandbyMode")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult DeleteStandbyMode([FromBody]StandbyModeModel standbyModeModel)
        {
            if (standbyModeModel.Bet < 0 || standbyModeModel.SubCategoryId <= 0 || string.IsNullOrEmpty(standbyModeModel.ConnectionId))
            {
                return BadRequest();
            }

            // TODO: öncesinde kullanıcı gerçekten bekleme modundamı redisden kontrol edilebilir

            var @event = new RemoveWaitingOpponentIntegrationEvent(UserId,
                                                                   standbyModeModel.ConnectionId,
                                                                   standbyModeModel.SubCategoryId,
                                                                   standbyModeModel.Bet,
                                                                   standbyModeModel.BalanceType);

            _eventBus.Publish(@event);

            return Ok();
        }

        [HttpGet("test")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Test()
        {
            return Ok(await questionRepository.DuelQuestions(1, UserId, "2222-2222-2222-2222", Core.Enums.Languages.English, Core.Enums.Languages.Turkish));
        }

        #endregion Methods
    }
}
