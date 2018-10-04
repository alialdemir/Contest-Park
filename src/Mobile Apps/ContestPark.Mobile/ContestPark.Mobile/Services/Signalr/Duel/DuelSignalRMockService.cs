using ContestPark.Mobile.Models.Duel;
using ContestPark.Mobile.Models.Duel.Quiz;
using System;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Signalr.Duel
{
    public class DuelSignalRMockService : IDuelSignalRService
    {
        public EventHandler<DuelStartingModel> DuelScreenInfoEventHandler { get; set; }

        public EventHandler<NextQuestion> NextQuestionEventHandler { get; set; }

        public void DuelScreenInfo()
        {
        }

        public void NextQuestion()
        {
        }

        public void OffDuelScreenInfo()
        {
        }

        public void OffNextQuestion()
        {
        }

        public Task SaveAnswer(UserAnswer userAnswer)
        {
            return Task.CompletedTask;
        }
    }
}