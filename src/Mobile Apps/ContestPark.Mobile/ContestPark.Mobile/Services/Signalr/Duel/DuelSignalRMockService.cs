using ContestPark.Mobile.Models.Duel;
using ContestPark.Mobile.Models.Duel.Quiz;
using System;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Signalr.Duel
{
    public class DuelSignalRMockService : IDuelSignalRService
    {
        public EventHandler<DuelStartingModel> DuelStartingEventHandler { get; set; }

        public EventHandler<DuelCreated> DuelCreatedEventHandler { get; set; }
        public EventHandler<NextQuestion> NextQuestionEventHandler { get; set; }

        public void DuelStarting()
        {
        }

        public void DuelCreated()
        {
        }

        public void OffDuelStarting()
        {
        }

        public void OffDuelCreated()
        {
        }

        public void NextQuestion()
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
