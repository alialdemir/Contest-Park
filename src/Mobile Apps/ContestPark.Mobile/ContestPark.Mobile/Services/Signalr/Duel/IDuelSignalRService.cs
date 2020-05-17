using ContestPark.Mobile.Models.Duel.InviteDuel;
using ContestPark.Mobile.Models.Duel.Quiz;
using ContestPark.Mobile.Models.Error;
using System;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Signalr.Duel
{
    public interface IDuelSignalRService
    {
        bool IsConnect { get; }

        void DuelCreated();

        void NextQuestion();

        void OffDuelCreated();

        void OffNextQuestion();

        void OffSendErrorMessage();

        Task SaveAnswer(UserAnswer userAnswer);

        Task LeaveGroup(int duelId);

        void InviteDuel();

        void SendErrorMessage();

        EventHandler<DuelCreated> DuelCreatedEventHandler { get; set; }

        EventHandler<NextQuestion> NextQuestionEventHandler { get; set; }
        EventHandler<InviteModel> InviteDuelEventHandler { get; set; }
        EventHandler<ErrorMessageModel> SendErrorMessagetHandler { get; set; }
    }
}
