using ContestPark.Domain.Question.Model.Response;
using ContestPark.Domain.Signalr.Interfaces;
using ContestPark.Domain.Signalr.Model.Request;
using Orleans;
using SignalR.Orleans.Core;
using System.Threading.Tasks;

namespace ContestPark.Infrastructure.Signalr.Grain
{
    public class QuestionSignalrGrain : Orleans.Grain, IQuestionSignalrGrain
    {
        #region Private variables

        private HubContext<IContestParkHub> _hubContext;

        #endregion Private variables

        #region Methods

        public override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();

            _hubContext = GrainFactory.GetHub<IContestParkHub>();
        }

        public override Task OnDeactivateAsync()
        {
            return base.OnDeactivateAsync();
        }

        public async Task NextQuestionAsync(QuestionCreated questionCreated)
        {
            if (!string.IsNullOrEmpty(questionCreated.FounderConnectionId))
            {
                // Dile göre soru ve cevapları filtreleme yaptık
                Question founderQuestion = questionCreated.Question.GetQuestionByLanguage(questionCreated.FounderLanguage);

                await _hubContext
                    .Group(questionCreated.FounderConnectionId)
                    .SendSignalRMessage("NextQuestion", founderQuestion);
            }

            if (!string.IsNullOrEmpty(questionCreated.OpponentConnectionId))
            {
                // Dile göre soru ve cevapları filtreleme yaptık
                Question opponenQuestion = questionCreated.Question.GetQuestionByLanguage(questionCreated.OpponentLanguage);

                await _hubContext
                    .Group(questionCreated.OpponentConnectionId)
                    .SendSignalRMessage("NextQuestion", opponenQuestion);
            }
            DeactivateOnIdle();
        }

        public async Task DuelStartingScreenAsync(DuelStartingScreen duelStartingScreen)
        {
            if (!string.IsNullOrEmpty(duelStartingScreen.FounderConnectionId))
            {
                await _hubContext
                    .Group(duelStartingScreen.FounderConnectionId)
                    .SendSignalRMessage("DuelScreen", duelStartingScreen);
            }

            if (!string.IsNullOrEmpty(duelStartingScreen.OpponentConnectionId))
            {
                await _hubContext
                    .Group(duelStartingScreen.OpponentConnectionId)
                    .SendSignalRMessage("DuelScreen", duelStartingScreen);
            }
        }

        #endregion Methods
    }
}