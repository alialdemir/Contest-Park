using ContestPark.Core.Models;
using ContestPark.Core.Services.Identity;
using ContestPark.Duel.API.Infrastructure.Repositories.Duel;
using ContestPark.Duel.API.IntegrationEvents.Events;
using ContestPark.Duel.API.Models;
using ContestPark.Duel.API.Resources;
using ContestPark.Duel.API.Services.SubCategory;
using ContestPark.EventBus.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ContestPark.Duel.API.Controllers
{
    public class DuelController : Core.Controllers.ControllerBase
    {
        #region Private Variables

        private readonly IEventBus _eventBus;
        private readonly IDuelRepository _duelRepository;
        private readonly IIdentityService _identityService;
        private readonly ISubCategoryService _subCategoryService;

        #endregion Private Variables

        #region Constructor

        public DuelController(ILogger<DuelController> logger,
                              IEventBus eventBus,
                              IIdentityService identityService,
                              ISubCategoryService subCategoryService,
                              IDuelRepository duelRepository) : base(logger)
        {
            _eventBus = eventBus;
            _identityService = identityService;
            _subCategoryService = subCategoryService;
            _duelRepository = duelRepository;
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

        /// <summary>
        /// Düellodan çık
        /// </summary>
        [HttpPost("DuelEscape")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult DuelEscape([FromBody]DuelEscapeModel duelEscapeModel)
        {
            if (_duelRepository.IsDuelFinish(duelEscapeModel.DuelId))
            {
                return BadRequest(DuelResource.YouCantLeaveTheFinishedDuel);
            }

            var @event = new DuelEscapeIntegrationEvent(duelEscapeModel.DuelId,
                                                        UserId,
                                                        duelEscapeModel.FounderUserId,
                                                        duelEscapeModel.OpponentUserId,
                                                        duelEscapeModel.Questions);

            _eventBus.Publish(@event);

            return Ok();
        }

        /// <summary>
        /// Düello sonuç ekranını verir
        /// </summary>
        /// <param name="duelId">Düello id</param>
        /// <returns>Düello sonuç ekranı</returns>
        [HttpGet("{duelId}")]
        [ProducesResponseType(typeof(DuelResultModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DuelResult([FromRoute]int duelId)
        {
            if (duelId <= 0)
                return NotFound();

            DuelResultModel result = _duelRepository.DuelResultByDuelId(duelId, UserId);
            if (result == null)
                return NotFound();

            IEnumerable<UserModel> users = await _identityService.GetUserInfosAsync(new List<string>
            {
                result.FounderUserId,
                result.OpponentUserId
            });
            if (users == null || users.Count() < 2)
                return NotFound();

            UserModel founderUser = users.FirstOrDefault(x => x.UserId == result.FounderUserId);
            UserModel opponentUser = users.FirstOrDefault(x => x.UserId == result.OpponentUserId);

            result.FounderFullName = founderUser.FullName;
            result.FounderProfilePicturePath = founderUser.ProfilePicturePath;
            result.FounderUserName = founderUser.UserName;

            result.IsFounder = result.FounderUserId == UserId;

            result.OpponentFullName = opponentUser.FullName;
            result.OpponentProfilePicturePath = opponentUser.ProfilePicturePath;
            result.OpponentUserName = opponentUser.UserName;

            SubCategoryModel subCategoryModel = await _subCategoryService.GetSubCategoryInfo(result.SubCategoryId, CurrentUserLanguage);
            if (subCategoryModel != null)
            {
                result.SubCategoryName = subCategoryModel.SubCategoryName;
                result.SubCategoryPicturePath = subCategoryModel.SubCategoryPicturePath;
            }

            return Ok(result);
        }

        #endregion Methods
    }
}
