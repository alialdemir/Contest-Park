using ContestPark.Chat.API.Infrastructure.Repositories.Block;
using ContestPark.Chat.API.Infrastructure.Repositories.Conversation;
using ContestPark.Chat.API.Infrastructure.Repositories.Message;
using ContestPark.Chat.API.IntegrationEvents.Events;
using ContestPark.Chat.API.Model;
using ContestPark.Chat.API.Resources;
using ContestPark.Core.Database.Models;
using ContestPark.Core.Models;
using ContestPark.Core.Services.Identity;
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
        private readonly IIdentityService _identityService;
        private readonly IEventBus _eventBus;
        private readonly IMessageRepository _messageRepository;

        #endregion Private Variables

        #region Constructor

        public ChatController(ILogger<ChatController> logger,
                              IBlockRepository blockRepository,
                              IConversationRepository conversationRepository,
                              IIdentityService identityService,
                              IMessageRepository messageRepository,
                              IEventBus eventBus) : base(logger)
        {
            _blockRepository = blockRepository;
            _conversationRepository = conversationRepository;
            _identityService = identityService;
            _messageRepository = messageRepository;
            _eventBus = eventBus;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Kullanıcı mesaj listesi
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ServiceModel<MessageModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get([FromQuery] PagingModel paging)
        {
            // TODO: VisibilityStatus ile ilgili birşey yapılmadı o kısım yazılacak

            ServiceModel<MessageModel> result = _conversationRepository.UserMessages(UserId, paging);

            if (result == null || result.Items == null || result.Items.Count() == 0)
                return Ok(result);

            IEnumerable<UserModel> users = await _identityService.GetUserInfosAsync(result.Items.Select(x => x.SenderUserId).AsEnumerable());

            result.Items.ToList().ForEach(message =>
            {
                UserModel user = users.FirstOrDefault(x => x.UserId == message.SenderUserId);

                message.UserFullName = user.FullName;
                message.UserProfilePicturePath = user.ProfilePicturePath;
                message.UserName = user.UserName;

                if (message.LastWriterUserId == UserId)// en son mesajı gönderen kendiisi ise
                {
                    message.Message = ChatResource.You + ": " + message.Message;
                }

                message.LastWriterUserId = null;// cliente gitmemesi için null atadım
            });

            return Ok(result);
        }

        /// <summary>
        /// Konuşma detayını döndürür
        /// </summary>
        /// <param name="conversationId">Konuşma id</param>
        /// <param name="paging">Sayfalama</param>
        /// <returns>Konuşma detayı</returns>
        [HttpGet("{conversationId}")]
        [ProducesResponseType(typeof(ServiceModel<ConversationDetailModel>), (int)HttpStatusCode.OK)]
        public IActionResult Get([FromRoute]long conversationId, [FromQuery] PagingModel paging)
        {
            if (!_conversationRepository.IsConversationBelongUser(UserId, conversationId))// Conversation id o login olan kullanıcının bulunduğu bir konuşma mı kontrol ettik
                return BadRequest(ChatResource.ThisConversationIsNotYours);

            ServiceModel<ConversationDetailModel> result = _messageRepository.ConversationDetail(UserId, conversationId, paging);

            return Ok(result);
        }

        /// <summary>
        /// Okunmamış mesaj sayısı
        /// </summary>
        [HttpGet("UnReadMessageCount")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IActionResult Get()// TODO: dönen type ProducesResponseType typeof olarak gösterilmeli
        {
            return Ok(new
            {
                UnReadMessageCount = _conversationRepository.UnReadMessageCount(UserId)
            });
        }

        /// <summary>
        /// Konuşmadaki mesajları okundu yapar
        /// </summary>
        [HttpPost("{conversationId}/ReadMessages")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Post([FromRoute]long conversationId)// TODO: dönen type ProducesResponseType typeof olarak gösterilmeli
        {
            bool isSuccess = await _conversationRepository.AllMessagesRead(UserId, conversationId);

            if (!isSuccess)
            {
                Logger.LogCritical("CRITICAL: konuşmadak mesajları okundu yap kısmı başarısız oldu", conversationId);
            }
            else
            {
                isSuccess = await _messageRepository.RemoveMessagesAsync(UserId, conversationId);
                if (!isSuccess)
                {
                    Logger.LogCritical("CRITICAL: Mesajlar okundu yap kısmı başarısız oldu", conversationId);
                }
            }

            return Ok();
        }

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
        public IActionResult RemoveMessages([FromRoute]long conversationId)
        {
            if (conversationId <= 0)
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
        public async Task<IActionResult> UserBlockedList([FromQuery]PagingModel paging)
        {
            ServiceModel<BlockModel> result = _blockRepository.UserBlockedList(UserId, paging);
            IEnumerable<UserModel> users = await _identityService.GetUserInfosAsync(result.Items.Select(x => x.UserId).AsEnumerable());

            result.Items.ToList().ForEach(block => block.FullName = users.FirstOrDefault(u => u.UserId == block.UserId).FullName);

            return Ok(result);
        }

        #endregion Methods
    }
}
