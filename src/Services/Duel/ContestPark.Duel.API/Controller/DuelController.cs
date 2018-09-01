using ContestPark.Core.Domain.Model;
using ContestPark.Domain.Duel.Model.Request;
using ContestPark.Domain.Identity.Interfaces;
using ContestPark.Domain.Signalr.Interfaces;
using ContestPark.Domain.Signalr.Model.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Orleans;
using System.Net;
using System.Threading.Tasks;

namespace ContestPark.Duel.API.Controller
{
    public class DuelController : Core.Service.ControllerBase
    {
        #region Private Variables

        private readonly ILogger<DuelController> _logger;
        private readonly IClusterClient _clusterClient;

        #endregion Private Variables

        #region Constructor

        public DuelController(
            ILogger<DuelController> logger,
            IClusterClient clusterClient
        ) : base(logger)
        {
            _logger = logger;
            _clusterClient = clusterClient;
        }

        #endregion Constructor

        #region Services

        /// <summary>
        /// Random kullanici resim listesi döndürür
        /// </summary>
        /// <param name="pagingModel">Sayfalama</param>
        /// <returns>Profil resimleri service modeli</returns>
        [HttpGet]
        [Route("RandomProfilePictures")]
        [ProducesResponseType(typeof(ServiceResponse<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Get([FromQuery]Paging pagingModel)
        {
            var randomPictures = await _clusterClient
                .GetGrain<IUserGrain>(0)
                .RandomUserProfilePictures(UserId, pagingModel);

            return Ok(randomPictures);
        }

        /// <summary>
        /// Düello için sıraya alır
        /// </summary>
        /// <param name="standbyModeModel">Hangi kategoride oynamak istediği bilgisi</param>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult Post([FromBody]StandbyMode standbyModeModel)
        {
            if (standbyModeModel.Bet < 0 || standbyModeModel.SubCategoryId < 0)
            {
                _logger.LogInformation($"Düello sıraya girme işlemi başarısız oldu. Bet: {standbyModeModel.Bet} SubcategoryId: {standbyModeModel.SubCategoryId}");
                return NotFound();
            }

            var waitingOpponent = new WaitingOpponent(UserId, standbyModeModel.ConnectionId, standbyModeModel.SubCategoryId, standbyModeModel.Bet, CurrentUserLanguage);

            _clusterClient
                .GetGrain<IDuelSignalrGrain>(0)
                .WaitingOpponentAsync(waitingOpponent);

            return Ok();
        }

        /// <summary>
        /// Düellodan çıkınca bekleme modundan çıkartır
        /// </summary>
        /// <param name="standbyModeModel"></param>
        /// <returns></returns>
        [HttpPost("exit")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult Exit([FromBody]StandbyMode standbyModeModel)
        {
            if (standbyModeModel.Bet < 0 || standbyModeModel.SubCategoryId < 0)
            {
                _logger.LogInformation($"Düellodan bekleme modundan çıkma işlemi başarısız oldu. Bet: {standbyModeModel.Bet} SubcategoryId: {standbyModeModel.SubCategoryId}");
                return NotFound();
            }

            var opponentRemove = new WaitingOpponentRemove(UserId, standbyModeModel.ConnectionId, standbyModeModel.SubCategoryId, standbyModeModel.Bet);

            _clusterClient
                .GetGrain<IDuelSignalrGrain>(0)
                .WaitingOpponentRemoveAsync(opponentRemove);

            return Ok();
        }

        /// <summary>
        /// Bot devreye girer
        /// </summary>
        /// <param name="botStandbyMode">Hangi kategoride oynamak istediği bilgisi</param>

        [HttpPost("bot")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Post([FromBody]BotStandbyMode botStandbyMode)
        {
            if (botStandbyMode.Bet < 0 || botStandbyMode.SubCategoryId < 0)
            {
                _logger.LogInformation($"Düello sıraya girme işlemi başarısız oldu. Bet: {botStandbyMode.Bet} SubcategoryId: {botStandbyMode.SubCategoryId}");
                return NotFound();
            }

            string randomBotUserId = await _clusterClient
                .GetGrain<IUserGrain>(0)
                .GetRandomBotUserId();

            var waitingOpponent = new WaitingOpponent(randomBotUserId, string.Empty, botStandbyMode.SubCategoryId, botStandbyMode.Bet, CurrentUserLanguage);

            await _clusterClient
                  .GetGrain<IDuelSignalrGrain>(0)
                  .WaitingOpponentAsync(waitingOpponent);

            return Ok();
        }

        #endregion Services
    }
}