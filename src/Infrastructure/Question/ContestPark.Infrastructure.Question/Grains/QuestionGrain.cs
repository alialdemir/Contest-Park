using ContestPark.Domain.Question.Interfaces;
using ContestPark.Domain.Question.Model.Request;
using ContestPark.Infrastructure.Question.Repositories.Question;
using Orleans;
using System.Threading.Tasks;

namespace ContestPark.Infrastructure.Question.Grains
{
    public class QuestionGrain : Grain, IQuestionGrain
    {
        #region Private variables

        private readonly IQuestionRepository _questionRepository;

        private IAskedQuestionGrain _askedQuestionGrain;

        #endregion Private variables

        #region Constructor

        public QuestionGrain(IQuestionRepository questionRepository)
        {
            _questionRepository = questionRepository;
        }

        #endregion Constructor

        #region Methods

        public override Task OnActivateAsync()
        {
            long primaryId = this.GetPrimaryKeyLong();
            _askedQuestionGrain = GrainFactory.GetGrain<IAskedQuestionGrain>(primaryId);

            return base.OnActivateAsync();
        }

        public override async Task OnDeactivateAsync()
        {
            await _askedQuestionGrain.OnDeactivateAsync();

            await base.OnDeactivateAsync();
        }

        public async Task<Domain.Question.Model.Response.Question> QuestionCreate(QuestionInfo questionInfo)
        {
            var question = await _questionRepository.GetQuestionAsync(questionInfo);

            await _askedQuestionGrain
                    .Insert(questionInfo.SubCategoryId,
                    question.QuestionInfoId,
                    questionInfo.FounderUserId,
                    questionInfo.OpponentUserId);

            return question;
        }

        #endregion Methods
    }
}