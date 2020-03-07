using ContestPark.Mobile.Models.Duel;
using ContestPark.Mobile.Models.Duel.Quiz;
using ContestPark.Mobile.Services.Signalr.Base;
using System;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Signalr.Duel
{
    public class DuelSignalRService : IDuelSignalRService
    {
        #region Private

        private readonly ISignalRServiceBase _signalRService;

        #endregion Private

        #region Constructor

        public DuelSignalRService(ISignalRServiceBase signalRService)
        {
            _signalRService = signalRService ?? throw new ArgumentNullException(nameof(signalRService));
        }

        #endregion Constructor

        #region Events

        public EventHandler<DuelStartingModel> DuelStartingEventHandler { get; set; }

        public EventHandler<DuelCreated> DuelCreatedEventHandler { get; set; }
        public EventHandler<InviteModel> InviteDuelEventHandler { get; set; }

        public EventHandler<NextQuestion> NextQuestionEventHandler { get; set; }

        #endregion Events

        #region Method

        /// <summary>
        /// Duello bekleme ekranındaki kullanıcı bilgilerini getirir
        /// </summary>
        public void DuelStarting()
        {
            _signalRService?.On<DuelStartingModel>("DuelStarting", (data) => DuelStartingEventHandler?.Invoke(data, null));
        }

        /// <summary>
        /// Sıradaki soruyu almak için
        /// </summary>
        public void DuelCreated()
        {
            _signalRService?.On<DuelCreated>("DuelCreated", (data) =>
            {
                DuelCreatedEventHandler?.Invoke(data, null);
            });
        }

        /// <summary>
        /// Düello daveti kabul etme eventi
        /// </summary>
        public void InviteDuel()
        {
            _signalRService?.On<InviteModel>("InviteDuel", (data) =>
            {
                InviteDuelEventHandler?.Invoke(data, null);
            });
        }

        /// <summary>
        /// On DuelStarting kısmını kapatır
        /// </summary>
        public void OffDuelStarting()
        {
            _signalRService?.Off("DuelStarting");
        }

        /// <summary>
        /// On DuelCreated kısmını kapatır
        /// </summary>
        public void OffDuelCreated()
        {
            _signalRService?.Off("DuelCreated");
        }

        public void NextQuestion()
        {
            _signalRService?.On<NextQuestion>("NextQuestion", (data) => NextQuestionEventHandler?.Invoke(data, null));
        }

        public void OffNextQuestion()
        {
            _signalRService?.Off("NextQuestion");
        }

        /// <summary>
        /// Soruya verdiği cevabı gönderir
        /// </summary>
        /// <param name="userAnswer">Cevap</param>
        public Task SaveAnswer(UserAnswer userAnswer)
        {
            return _signalRService.SendMessage("saveAnswer", userAnswer);
        }

        /// <summary>
        /// Duello için oluşturduğumuz signalr grubundan ayrıldık
        /// </summary>
        /// <param name="duelId">Duello id</param>
        public Task LeaveGroup(int duelId)
        {
            return _signalRService.SendMessage("leaveGroup", duelId);
        }

        #endregion Method
    }
}
