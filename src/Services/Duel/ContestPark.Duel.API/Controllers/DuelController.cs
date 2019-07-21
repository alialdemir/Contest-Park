using ContestPark.Category.API.IntegrationEvents.Events;
using ContestPark.Duel.API.Models;
using ContestPark.EventBus.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;

namespace ContestPark.Duel.API.Controllers
{
    public class DuelController : Core.Controllers.ControllerBase
    {
        #region Private Variables

        private readonly IEventBus _eventBus;

        #endregion Private Variables

        #region Constructor

        public DuelController(ILogger<DuelController> logger,
                              IEventBus eventBus) : base(logger)
        {
            _eventBus = eventBus;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Duello bekleme moduna al
        /// </summary>
        /// <param name="standbyModeModel">Bekleme modu bilgileri</param>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult Post([FromBody]StandbyModeModel standbyModeModel)
        {
            if (standbyModeModel.Bet < 0 || standbyModeModel.SubCategoryId <= 0 || string.IsNullOrEmpty(standbyModeModel.ConnectionId))
            {
                return BadRequest();
            }

            var @event = new WaitingOpponentIntegrationEvent(UserId,
                                                             standbyModeModel.ConnectionId,
                                                             standbyModeModel.SubCategoryId,
                                                             standbyModeModel.Bet,
                                                             standbyModeModel.BalanceType);

            _eventBus.Publish(@event);

            return Ok();
        }

        #endregion Methods
    }
}
