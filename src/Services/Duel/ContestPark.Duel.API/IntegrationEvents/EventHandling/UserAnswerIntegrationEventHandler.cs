using ContestPark.Duel.API.Enums;
using ContestPark.Duel.API.Infrastructure.Repositories.DuelDetail;
using ContestPark.Duel.API.Infrastructure.Repositories.Redis.UserAnswer;
using ContestPark.Duel.API.IntegrationEvents.Events;
using ContestPark.Duel.API.Models;
using ContestPark.Duel.API.Services.ScoreCalculator;
using ContestPark.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Duel.API.IntegrationEvents.EventHandling
{
    public class UserAnswerIntegrationEventHandler : IIntegrationEventHandler<UserAnswerIntegrationEvent>
    {
        private readonly IUserAnswerRepository _userAnswerRepository;
        private readonly IScoreCalculator _scoreCalculator;
        private readonly IDuelDetailRepository _duelDetailRepository;
        private readonly IEventBus _eventBus;
        private readonly ILogger<UserAnswerIntegrationEventHandler> _logger;
        private const byte MAX_ANSWER_COUNT = 6;

        public UserAnswerIntegrationEventHandler(IUserAnswerRepository userAnswerRepository,
                                                 IScoreCalculator scoreCalculator,
                                                 IDuelDetailRepository duelDetailRepository,
                                                 IEventBus eventBus,
                                                 ILogger<UserAnswerIntegrationEventHandler> logger)
        {
            _userAnswerRepository = userAnswerRepository;
            _scoreCalculator = scoreCalculator;
            _duelDetailRepository = duelDetailRepository;
            _eventBus = eventBus;
            _logger = logger;
        }

        public Task Handle(UserAnswerIntegrationEvent @event)
        {
            _logger.LogInformation($@"Soru cevaplandı. Duel id: {@event.DuelId}
                                                       Question id: {@event.QuestionId}
                                                       User id: {@event.UserId}
                                                       Stylish: {@event.Stylish}
                                                       Time: {@event.Time}");

            Stylish correctStylish = _duelDetailRepository.GetCorrectAnswer(@event.DuelId, @event.QuestionId);

            byte score = correctStylish == @event.Stylish ? _scoreCalculator.Calculator(@event.Round, @event.Time) : (byte)0;

            UserAnswerModel userAnswer = new UserAnswerModel
            {
                DuelId = @event.DuelId,
                QuestionId = @event.QuestionId,
                Stylish = @event.Stylish,
                Time = @event.Time,
                UserId = @event.UserId,
                IsFounder = @event.IsFounder,
                Score = score
            };

            _userAnswerRepository.Insert(userAnswer);

            List<UserAnswerModel> userAnswers = _userAnswerRepository.GetAnswers(userAnswer);

            List<UserAnswerModel> answers = userAnswers
                                                    .Where(x => x.DuelId == @event.DuelId && x.QuestionId == @event.QuestionId)
                                                    .ToList();

            if (answers.Count != 2)
                return Task.CompletedTask;

            Task.Factory.StartNew(() =>
            {
                bool isGameEnd = @event.Round == MAX_ANSWER_COUNT;

                UserAnswerModel founderAnswer = answers.FirstOrDefault(x => x.IsFounder);
                UserAnswerModel opponentAnswer = answers.FirstOrDefault(x => !x.IsFounder);

                if (founderAnswer == null || opponentAnswer == null)
                {
                    _logger.LogInformation($@"Cevaplamalardan biri boş geldi  Duel id: {@event.DuelId}
                                                                                  Question id: {@event.QuestionId}
                                                                                  User id: {@event.UserId}
                                                                                  Stylish: {@event.Stylish}
                                                                                  Time: {@event.Time}");

                    // TODO: #issue 228
                }

                var @nextQuestionEvent = new NextQuestionIntegrationEvent(@event.DuelId,
                                                                          founderAnswer.Stylish,
                                                                          opponentAnswer.Stylish,
                                                                          correctStylish,
                                                                          founderAnswer.Score,
                                                                          opponentAnswer.Score,
                                                                          isGameEnd);

                _eventBus.Publish(@nextQuestionEvent);

                if (isGameEnd)
                {
                    _duelDetailRepository.UpdateDuelDetailAsync(userAnswers);
                }
            });

            return Task.CompletedTask;
        }
    }
}
