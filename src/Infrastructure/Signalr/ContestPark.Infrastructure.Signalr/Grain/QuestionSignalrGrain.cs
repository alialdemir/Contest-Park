using ContestPark.Domain.Duel.Model.Response;
using ContestPark.Domain.Signalr.Interfaces;
using ContestPark.Domain.Signalr.Model.Request;
using Microsoft.Extensions.Logging;
using Orleans;
using SignalR.Orleans.Core;
using System;
using System.Threading.Tasks;

namespace ContestPark.Infrastructure.Signalr.Grain
{
    public class QuestionSignalrGrain : Orleans.Grain, IQuestionSignalrGrain
    {
        #region Private variables

        private HubContext<IContestParkHub> _hubContext;

        private readonly ILogger<QuestionSignalrGrain> _logger;

        #endregion Private variables

        #region Constructor

        public QuestionSignalrGrain(ILogger<QuestionSignalrGrain> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion Constructor

        #region Methods

        public override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();

            _hubContext = GrainFactory.GetHub<IContestParkHub>();
        }

        /// <summary>
        /// Sıradaki soruyu kullanıcılara gönderir
        /// </summary>
        /// <param name="questionCreated">Soru bilgisi</param>
        public async Task NextQuestionAsync(NextQuestion nextQuestion)
        {
            string groupName = GetGroupNameByDuelId(nextQuestion.DuelId);

            await _hubContext
                        .Group(groupName)
                        .SendSignalRMessage("NextQuestion", nextQuestion);

            _logger.LogInformation($"Duello sorusu oyunculara gönderildi Duel Id: {nextQuestion.DuelId} Question Info Id: {nextQuestion.Question.QuestionId}");
        }

        /// <summary>
        /// Düello başlangıç ekranını gösterir ve duel id göre signalr group oluşturur
        /// </summary>
        /// <param name="duelStartingScreen">Düello başlangıç ekran bilgileri</param>
        public async Task DuelStartingScreenAsync(DuelStartingScreen duelStartingScreen)
        {
            string groupName = GetGroupNameByDuelId(duelStartingScreen.DuelId);

            await _hubContext
                        .Group(groupName)
                        .SendSignalRMessage("DuelScreen", duelStartingScreen);

            _logger.LogInformation($"Düello ekranı oyunculara gönderildi Duel Id: {duelStartingScreen.DuelId}");
        }

        /// <summary>
        /// Düello id göre signalr group oluşturur
        /// </summary>
        /// <param name="duelId"></param>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public async Task AddToGroup(int duelId, string connectionId)
        {
            if (string.IsNullOrEmpty(connectionId) || duelId <= 0)
                return;

            string groupName = GetGroupNameByDuelId(duelId);

            await _hubContext
                 .Group(groupName)
                 .Add("ContestParkHub", connectionId);

            _logger.LogInformation($"{groupName} isimli gruba connection id: {connectionId} eklendi.");
        }

        private string GetGroupNameByDuelId(int duelId)
        {
            return $"duel{duelId.ToString()}";
        }

        /// <summary>
        /// Gruptan connection id çıkarır
        /// </summary>
        /// <param name="duelId">Düello id</param>
        /// <param name="connectionIds">Signalr bağlantı id</param>
        public async Task RemoveGroup(int duelId, params string[] connectionIds)
        {
            string groupName = GetGroupNameByDuelId(duelId);

            foreach (string connectionId in connectionIds)
            {
                if (!string.IsNullOrEmpty(connectionId))
                {
                    await _hubContext
                        .Group(groupName).Remove(connectionId);

                    _logger.LogInformation($"{groupName} isimli gruptan connection id: {connectionId} çıkarıldı.");
                }
            }
        }

        #endregion Methods
    }
}