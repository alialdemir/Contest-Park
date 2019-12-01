using ContestPark.Admin.API.Infrastructure.Repositories.AnswerLocalized;
using ContestPark.Admin.API.Infrastructure.Repositories.Question;
using ContestPark.Admin.API.Infrastructure.Repositories.QuestionLocalized;
using ContestPark.Admin.API.Infrastructure.Repositories.QuestionOfQuestionLocalized;
using ContestPark.Admin.API.Infrastructure.Tables;
using ContestPark.Admin.API.IntegrationEvents.Events;
using ContestPark.Admin.API.Services.Picture;
using ContestPark.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Admin.API.IntegrationEvents.EventHandling
{
    public class CreateQuestionIntegrationEventHandler : IIntegrationEventHandler<CreateQuestionIntegrationEvent>
    {
        #region Private variables

        private readonly IEventBus _eventBus;
        private readonly IQuestionLocalizedRepository _questionLocalizedRepository;
        private readonly IAnswerLocalizedRepository _answerLocalizedRepository;
        private readonly IQuestionOfQuestionLocalizedRepository _questionOfQuestionLocalizedRepository;
        private readonly IFileUploadService _fileUploadService;
        private readonly IQuestionRepository _questionRepository;
        private readonly ILogger<CreateQuestionIntegrationEventHandler> _logger;

        #endregion Private variables

        #region Constructor

        public CreateQuestionIntegrationEventHandler(IEventBus eventBus,
                                                     IQuestionLocalizedRepository questionLocalizedRepository,
                                                     IAnswerLocalizedRepository answerLocalizedRepository,
                                                     IQuestionOfQuestionLocalizedRepository questionOfQuestionLocalizedRepository,
                                                     IFileUploadService fileUploadService,
                                                     IQuestionRepository questionRepository,
                                                     ILogger<CreateQuestionIntegrationEventHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _questionLocalizedRepository = questionLocalizedRepository;
            _answerLocalizedRepository = answerLocalizedRepository;
            _questionOfQuestionLocalizedRepository = questionOfQuestionLocalizedRepository;
            _fileUploadService = fileUploadService;
            _questionRepository = questionRepository;
        }

        /// <summary>
        /// Oyuna soru ekleme eventi
        /// </summary>
        /// <param name="event">Sorular</param>
        public async Task Handle(CreateQuestionIntegrationEvent @event)
        {
            _logger.LogInformation("Yeni sorular ekleniyor...");

            if (@event == null || !@event.Questions.Any())
            {
                _logger.LogWarning("Soru listesi boş geldi.");

                return;
            }

            foreach (var item in @event.Questions)
            {
                foreach (var question in item.Questions)
                {
                    string fileUrl = string.Empty;

                    if (!string.IsNullOrEmpty(question.Link))
                    {
                        fileUrl = await _fileUploadService.UploadFileToStorageAsync(question.Link, question.SubCategoryId, question.QuestionType);
                        if (string.IsNullOrEmpty(fileUrl))
                        {
                            _logger.LogError("Soru yükleme işlemi başarısız oldu");
                            continue;
                        }
                    }

                    int questionId = await _questionRepository.Insert(new Question
                    {
                        Link = fileUrl,
                        SubCategoryId = question.SubCategoryId,
                        AnswerType = question.AnswerTypes,
                        QuestionType = question.QuestionType,
                    });

                    foreach (var ql in item.QuestionLocalized)
                    {
                        int questionLocalizedId = _questionLocalizedRepository.IsQuestionRegistry(ql.Question);
                        if (questionLocalizedId == 0)
                        {
                            questionLocalizedId = await _questionLocalizedRepository.Insert(new QuestionLocalized
                            {
                                Language = ql.Language,
                                Question = ql.Question,
                            });
                        }

                        await _questionOfQuestionLocalizedRepository.Insert(new QuestionOfQuestionLocalized
                        {
                            QuestionId = questionId,
                            QuestionLocalizedId = questionLocalizedId
                        });
                    }

                    await _answerLocalizedRepository.Insert(item.Answers.Select(answer => new AnswerLocalized
                    {
                        QuestionId = questionId,
                        Language = answer.Language,
                        CorrectStylish = answer.CorrectStylish,
                        Stylish1 = answer.Stylish1,
                        Stylish2 = answer.Stylish2,
                        Stylish3 = answer.Stylish3,
                    }).ToList());
                }
            }

            _logger.LogInformation("Yeni sorular ekrandi. Eklenen soru sayısı {Count}", @event.Questions.Count);
        }

        #endregion Constructor
    }
}
