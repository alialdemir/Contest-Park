using ContestPark.Chat.API.Infrastructure.Repositories.Block;
using ContestPark.Chat.API.IntegrationEvents.Events;
using ContestPark.Chat.API.Model;
using ContestPark.Chat.API.Resources;
using ContestPark.EventBus.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;

namespace ContestPark.Chat.API.Controllers
{
    public class ChatController : Core.Controllers.ControllerBase
    {
        #region Private Variables

        private readonly IBlockRepository _blockRepository;

        private readonly IEventBus _eventBus;

        #endregion Private Variables

        #region Constructor

        public ChatController(ILogger<ChatController> logger,
                              IBlockRepository blockRepository,
                              IEventBus eventBus) : base(logger)
        {
            _blockRepository = blockRepository;
            _eventBus = eventBus;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Mesaj gönder
        /// </summary>
        /// <param name="message">Mesaj bilgisi</param>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IActionResult Post([FromBody]Message message)
        {
            if (UserId == message.ReceiverUserId)
                return BadRequest(ChatResource.YouCanNotMendMessagesToYourself);

            if (_blockRepository.BlockingStatus(UserId, message.ReceiverUserId))
                return BadRequest(ChatResource.ThereAreBetweenYouAndBlockThisUser_YouOrHeMayHaveBlockedYou);

            var @event = new SendMessageIntegrationEvent(UserId, message.ReceiverUserId, message.Text);
            _eventBus.Publish(@event);

            return Ok();
        }

        /// <summary>
        /// Kullanıcı engelle
        /// </summary>
        /// <param name="deterredUserId">Engellenen kullanıcı id</param>
        [HttpPost("block/{deterredUserId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> BlockedAsync([FromRoute]string deterredUserId)
        {
            if (UserId == deterredUserId)
                return BadRequest();

            if (_blockRepository.BlockingStatus(UserId, deterredUserId))
                return BadRequest(ChatResource.ThisIserIsAlreadyBlocked);

            bool isSuccess = await _blockRepository.BlockedAsync(UserId, deterredUserId);
            if (!isSuccess)
                return BadRequest(ChatResource.AnUnexpectedErrorHasOccurredPleaseTryAgain);

            return Ok();
        }

        /// <summary>
        /// Kullanıcı engellini kaldır
        /// </summary>
        /// <param name="deterredUserId">Engellenen kullanıcı id</param>
        [HttpDelete("unblock/{deterredUserId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UnBlockedAsync([FromRoute]string deterredUserId)
        {
            if (UserId == deterredUserId)
                return BadRequest();

            if (!_blockRepository.BlockingStatus(UserId, deterredUserId))
                return BadRequest(ChatResource.YouHaveNotBlockedThisUser);

            bool isSuccess = await _blockRepository.UnBlockAsync(UserId, deterredUserId);
            if (!isSuccess)
                return BadRequest(ChatResource.AnUnexpectedErrorHasOccurredPleaseTryAgain);

            return Ok();
        }

        /// <summary>
        /// Kullanıcı ile arasında engelleme durumunu döndürür
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <returns></returns>
        [HttpGet("BlockedStatus/{userId}")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult BlockedStatus([FromRoute]string userId)
        {
            if (UserId == userId)
                return BadRequest();

            return Ok(new
            {
                isBlocked = _blockRepository.BlockingStatus(UserId, userId)
            });
        }

        #endregion Methods
    }
}