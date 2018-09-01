using ContestPark.Mobile.Models.Duel;
using ContestPark.Mobile.Models.Duel.Quiz;
using System;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Signalr.Duel
{
    public interface IDuelSignalRService
    {
        void DuelScreenInfo();

        void NextQuestion();

        void OffDuelScreenInfo();

        void OffNextQuestion();

        EventHandler<DuelStartingModel> DuelScreenInfoEventHandler { get; set; }

        EventHandler<NextQuestion> NextQuestionEventHandler { get; set; }

        Task SaveAnswer(UserAnswer userAnswer);
    }
}