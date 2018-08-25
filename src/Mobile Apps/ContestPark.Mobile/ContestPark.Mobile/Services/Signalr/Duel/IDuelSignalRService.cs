using ContestPark.Mobile.Models.Duel;
using System;

namespace ContestPark.Mobile.Services.Signalr.Duel
{
    public interface IDuelSignalRService
    {
        void DuelScreenInfo();

        void NextQuestion();

        void OffDuelScreenInfo();

        void OffNextQuestion();

        EventHandler<DuelStartingModel> DuelScreenInfoEventHandler { get; set; }

        EventHandler<QuestionModel> NextQuestionEventHandler { get; set; }
    }
}