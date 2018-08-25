using ContestPark.Core.Enums;
using ContestPark.Domain.Duel.Interfaces;
using ContestPark.Domain.Signalr.Interfaces;
using ContestPark.Domain.Signalr.Model.Request;
using ContestPark.Infrastructure.Signalr.Entities;
using ContestPark.Infrastructure.Signalr.Repositories.DuelUser;
using Microsoft.Extensions.Logging;
using ServiceStack;
using System;
using System.Threading.Tasks;

namespace ContestPark.Infrastructure.Signalr.Grain
{
    public class DuelSignalrGrain : Orleans.Grain, IDuelSignalrGrain
    {
        #region Private variables

        private readonly IDuelUserRepository _duelUserRepository;

        private readonly ILogger<DuelSignalrGrain> _logger;

        #endregion Private variables

        #region Constructor

        public DuelSignalrGrain(IDuelUserRepository duelUserRepository,
                                ILogger<DuelSignalrGrain> logger)
        {
            _duelUserRepository = duelUserRepository;
            _logger = logger;
        }

        #endregion Constructor

        #region Methods

        #region WaitingOpponent

        public async Task WaitingOpponentAsync(WaitingOpponent waitingOpponent)
        {
            if (string.IsNullOrEmpty(waitingOpponent.UserId) || waitingOpponent.SubCategoryId < 0 || waitingOpponent.Bet < 0)
            {
                _logger.LogWarning($@"Duello oluşturulken değerler boş geldi.
                                    {nameof(waitingOpponent.UserId)}: {waitingOpponent.UserId}
                                    {nameof(waitingOpponent.SubCategoryId)}: {waitingOpponent.SubCategoryId}
                                    {nameof(waitingOpponent.Bet)}: {waitingOpponent.Bet}");
                return;
            }

            _logger.LogInformation($"Rakip bekleyen kullanıcı id: {waitingOpponent.UserId}");

            DuelUser waitingDuelUser = new DuelUser
            {
                Bet = waitingOpponent.Bet,
                SubCategoryId = waitingOpponent.SubCategoryId,
                ConnectionId = "",
                UserId = "3333-3333-3333-bot",
                Language = Languages.English
            };// await _duelUserRepository.GetDuelUserAsync(@event.UserId, @event.SubCategoryId, @event.Bet);

            if (waitingDuelUser != null)
            {
                await StartDuelAsync(waitingOpponent, waitingDuelUser);
            }
            else
            {
                await AddAwatingModeAsync(waitingOpponent);
            }

            _duelUserRepository.ClearExpiredDuelUserList();
        }

        /// <summary>
        /// Kullanıcı redise bekleyenler arasına eklendi.
        /// </summary>
        private async Task AddAwatingModeAsync(WaitingOpponent waitingOpponent)
        {
            bool isSuccess = await _duelUserRepository.InsertAsync(new DuelUser
            {
                Bet = waitingOpponent.Bet,
                ConnectionId = waitingOpponent.ConnectionId,
                SubCategoryId = waitingOpponent.SubCategoryId,
                UserId = waitingOpponent.UserId,
                Language = waitingOpponent.Language
            });

            _logger.LogInformation($"Rakip bekleyen kullanıcı sıraya alındı. kullanıcı id: {waitingOpponent.UserId}");
        }

        /// <summary>
        /// Singalr ile oyunu başlat
        /// </summary>
        private async Task StartDuelAsync(WaitingOpponent waitingOpponent, DuelUser duelUser)
        {
            await _duelUserRepository.DeleteAsync(duelUser);

            var duelStart = new ContestPark.Domain.Duel.Model.Request.DuelStart(subCategoryId: waitingOpponent.SubCategoryId,
                                                       bet: waitingOpponent.Bet,

                                                       founderUserId: waitingOpponent.UserId,
                                                       founderConnectionId: waitingOpponent.ConnectionId,
                                                       founderLanguage: waitingOpponent.Language,

                                                       opponentUserId: duelUser.UserId,
                                                       opponentConnectionId: duelUser.ConnectionId,
                                                       opponentLanguage: duelUser.Language);

            await GrainFactory.GetGrain<IDuelGrain>(GetId()).DuelStart(duelStart);

            _logger.LogInformation("Rakipler eşleşti.", new
            {
                FounderUserId = waitingOpponent.UserId,
                OpponentUserId = duelUser.UserId
            });
        }

        #endregion WaitingOpponent

        #region WaitingOpponentRemove

        public async Task WaitingOpponentRemoveAsync(WaitingOpponentRemove opponentRemove)
        {
            bool isSuccess = await _duelUserRepository.DeleteAsync(new DuelUser()
            {
                Bet = opponentRemove.Bet,
                ConnectionId = opponentRemove.ConnectionId,
                SubCategoryId = opponentRemove.SubCategoryId,
                UserId = opponentRemove.UserId
            });

            if (isSuccess)
            {
                _logger.LogInformation($"Bekleme modundan çıkan kullanıcı id {opponentRemove.UserId}");
            }
            else
            {
                _logger.LogWarning($"Bir hata oluştu. Bekleme modundan çıkartılamadı kullanıcı id {opponentRemove.UserId}");
            }
        }

        #endregion WaitingOpponentRemove

        #endregion Methods

        #region Private methdos

        private int GetId()
        {
            return Convert.ToInt32(GrainReference.GetId());
        }

        #endregion Private methdos
    }
}