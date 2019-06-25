using ContestPark.Chat.API.Infrastructure.Documents;
using ContestPark.Chat.API.Infrastructure.Repositories.Block;
using ContestPark.Chat.API.Infrastructure.Repositories.Conversation;
using ContestPark.Chat.API.IntegrationEvents.Events;
using ContestPark.Chat.API.Model;
using ContestPark.Chat.API.Resources;
using ContestPark.Core.CosmosDb.Interfaces;
using ContestPark.Core.CosmosDb.Models;
using ContestPark.EventBus.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ContestPark.Chat.API.Controllers
{
    public class ChatController : Core.Controllers.ControllerBase
    {
        #region Private Variables

        private readonly IBlockRepository _blockRepository;
        private readonly IConversationRepository _conversationRepository;
        private readonly IEventBus _eventBus;
        private readonly IDocumentDbRepository<User> _userRepository;

        #endregion Private Variables

        #region Constructor

        public ChatController(ILogger<ChatController> logger,
                              IBlockRepository blockRepository,
                              IConversationRepository conversationRepository,
                              IDocumentDbRepository<User> userRepository,
                              IEventBus eventBus) : base(logger)
        {
            _blockRepository = blockRepository;
            _conversationRepository = conversationRepository;
            _userRepository = userRepository;
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
        public IActionResult Post([FromBody]Model.Message message)
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
        /// Kullanıcının mesajlarını siler
        /// </summary>
        /// <param name="conversationId">Konuşma id</param>
        [HttpDelete("{conversationId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationResult), (int)HttpStatusCode.BadRequest)]
        public IActionResult RemoveMessages([FromRoute]string conversationId)
        {
            if (string.IsNullOrEmpty(conversationId))
                return BadRequest();

            if (!_conversationRepository.IsConversationBelongUser(UserId, conversationId))// Conversation id o login olan kullanıcının bulunduğu bir konuşma mı kontrol ettik
                return BadRequest(ChatResource.ThisConversationIsNotYours);

            var @event = new RemoveMessagesIntegrationEvent(UserId, conversationId);
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
        [HttpGet("Block/Status/{userId}")]
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

        /// <summary>
        /// Engellediği kullanıcların listesi
        /// </summary>
        /// <param name="paging">Sayfalama</param>
        /// <returns>Engellenen kullanıcılar</returns>
        [HttpGet("Block")]
        [ProducesResponseType(typeof(ServiceModel<BlockModel>), (int)HttpStatusCode.OK)]
        public IActionResult UserBlockedList([FromQuery]PagingModel paging)
        {
            ServiceModel<BlockModel> result = _blockRepository.UserBlockedList(UserId, paging);
            IEnumerable<User> users = _userRepository.FindByIds(result.Items.Select(x => x.UserId).AsEnumerable());

            result.Items.ToList().ForEach(block =>
            {
                block.FullName = users.Where(u => u.Id == block.UserId).FirstOrDefault().FullName;
            });

            var notFoundUserIds = result
                                    .Items
                                    .Where(u => !users.Any(x => x.Id == u.UserId))
                                    .Select(x => x.UserId)
                                    .AsEnumerable();
            if (notFoundUserIds.Count() > 0)// Bulunamayan kullanıcıları identity apiden alması için event yolladık
            {
                var @event = new UserNotFoundIntegrationEvent(notFoundUserIds);
                _eventBus.Publish(@event);
            }

            return Ok(result);
        }

        #endregion Methods
    }
}