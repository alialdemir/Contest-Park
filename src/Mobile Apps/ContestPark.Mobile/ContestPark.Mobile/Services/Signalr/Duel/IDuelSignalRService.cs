using ContestPark.Mobile.Models.Duel;
using ContestPark.Mobile.Models.Duel.Quiz;
using System;

namespace ContestPark.Mobile.Services.Signalr.Duel
{
    public interface IDuelSignalRService
    {
        void DuelStarting();

        void DuelCreated();

        void NextQuestion();

        void OffDuelStarting();

        void OffDuelCreated();

        void OffNextQuestion();
        System.Threading.Tasks.Task SaveAnswer(UserAnswer userAnswer);
        System.Threading.Tasks.Task LeaveGroup(int duelId);

        EventHandler<DuelStartingModel> DuelStartingEventHandler { get; set; }

        EventHandler<DuelCreated> DuelCreatedEventHandler { get; set; }

        EventHandler<NextQuestion> NextQuestionEventHandler { get; set; }
    }
}
