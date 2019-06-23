using ContestPark.Chat.API.IntegrationEvents.Events;
using ContestPark.Chat.API.Model;
using ContestPark.EventBus.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;

namespace ContestPark.Chat.API.Controllers
{
    public class ChatController : Core.Controllers.ControllerBase
    {
        #region Private Variables

        private readonly IEventBus _eventBus;

        #endregion Private Variables

        #region Constructor

        public ChatController(ILogger<ChatController> logger,
            IEventBus eventBus) : base(logger)
        {
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
            // TODO: 2 kullanıcı arasında engelleme var mı kontrol edilmeli

            var @event = new SendMessageIntegrationEvent(UserId, message.ReceiverUserId, message.Text);
            _eventBus.Publish(@event);

            return Ok();
        }

        #endregion Methods
    }
}