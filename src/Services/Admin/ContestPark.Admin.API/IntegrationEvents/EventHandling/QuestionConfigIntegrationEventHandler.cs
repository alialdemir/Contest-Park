using ContestPark.Admin.API.IntegrationEvents.Events;
using ContestPark.Admin.API.Services.QuestionService;
using ContestPark.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Admin.API.IntegrationEvents.EventHandling
{
    public class QuestionConfigIntegrationEventHandler : IIntegrationEventHandler<QuestionConfigIntegrationEvent>
    {
        #region Private variables

        private readonly IEventBus _eventBus;
        private readonly IQuestionService _questionService;
        private readonly ILogger<QuestionConfigIntegrationEventHandler> _logger;

        #endregion Private variables

        #region Constructor

        public QuestionConfigIntegrationEventHandler(IEventBus eventBus,
                                                     IQuestionService questionService,
                                                     ILogger<QuestionConfigIntegrationEventHandler> logger)
        {
            _eventBus = eventBus;
            _questionService = questionService;
            _logger = logger;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Json ile soru oluştur
        /// </summary>
        /// <param name="event">Soru bilgileri</param>
        public Task Handle(QuestionConfigIntegrationEvent @event)
        {
            if (@event.QuestionConfig == null
                || @event.QuestionConfig.SubCategoryId <= 0
                || string.IsNullOrEmpty(@event.QuestionConfig.Json)
                || string.IsNullOrEmpty(@event.QuestionConfig.AnswerKey)
                || string.IsNullOrEmpty(@event.QuestionConfig.Question))
            {
                _logger.LogError("Json ile soru oluştur config bilgisi boş geldi.");

                return Task.CompletedTask;
            }

            _logger.LogInformation("Json ile soru oluşturuluyor");

            var questions = _questionService.GenerateQuestion(@event.QuestionConfig);
            if (questions == null || !questions.Any())
            {
                _logger.LogError("Json ile soru oluşturulurken sorular boş geldi");

                return Task.CompletedTask;
            }

            var @eventQuestion = new CreateQuestionIntegrationEvent(questions);

            _eventBus.Publish(@eventQuestion);

            _logger.LogInformation("Json ile {{count}} adet soru oluşturuldu", questions.Count);

            return Task.CompletedTask;
        }

        #endregion Methods
    }
}
