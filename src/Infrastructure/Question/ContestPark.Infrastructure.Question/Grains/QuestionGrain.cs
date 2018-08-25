using ContestPark.Domain.Question.Interfaces;
using ContestPark.Domain.Question.Model.Request;
using ContestPark.Domain.Question.Model.Response;
using ContestPark.Domain.Signalr.Interfaces;
using ContestPark.Infrastructure.Question.Repositories.Question;
using Orleans;
using System;
using System.Threading.Tasks;

namespace ContestPark.Infrastructure.Question.Grains
{
    public class QuestionGrain : Grain, IQuestionGrain
    {
        #region Private variables

        private readonly IQuestionRepository _questionRepository;

        #endregion Private variables

        #region Constructor

        public QuestionGrain(IQuestionRepository questionRepository)
        {
            _questionRepository = questionRepository;
        }

        #endregion Constructor

        #region Methods

        public async Task QuestionCreate(QuestionInfo questionInfo)
        {
            var question = await _questionRepository.GetQuestionAsync(questionInfo);

            if (question == null)
            {
                // TODO: question gelmese fail eventi publish edilmeli
                throw new ArgumentNullException(nameof(question));
            }

            await GrainFactory
                .GetGrain<IQuestionSignalrGrain>(1)
                .NextQuestionAsync(new QuestionCreated(questionInfo.FounderConnectionId,
                    questionInfo.OpponentConnectionId,
                    question,
                    questionInfo.FounderLanguage,
                    questionInfo.OpponentLanguage));

            await GrainFactory
                .GetGrain<IAskedQuestionGrain>(1)
                .Insert(questionInfo.SubCategoryId,
                    question.QuestionInfoId,
                    questionInfo.FounderUserId,
                    questionInfo.OpponentUserId);
        }

        #endregion Methods
    }
}