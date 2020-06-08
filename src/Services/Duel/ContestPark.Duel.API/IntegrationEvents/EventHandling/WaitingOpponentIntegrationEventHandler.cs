using ContestPark.Duel.API.Infrastructure.Repositories.Duel;
using ContestPark.Duel.API.Infrastructure.Repositories.Redis.DuelUser;
using ContestPark.Duel.API.IntegrationEvents.Events;
using ContestPark.Duel.API.Models;
using ContestPark.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ContestPark.Duel.API.IntegrationEvents.EventHandling
{
    public class WaitingOpponentIntegrationEventHandler : IIntegrationEventHandler<WaitingOpponentIntegrationEvent>
    {
        #region Private variables

        private readonly IDuelUserRepository _duelUserRepository;
        private readonly IEventBus _eventBus;
        private readonly IDuelRepository _duelRepository;
        private readonly ILogger<WaitingOpponentIntegrationEventHandler> _logger;

        #endregion Private variables

        #region Constructor

        public WaitingOpponentIntegrationEventHandler(IDuelUserRepository duelUserRepository,
                                                      IEventBus eventBus,
                                                      IDuelRepository duelRepository,
                                                      ILogger<WaitingOpponentIntegrationEventHandler> logger)
        {
            _duelUserRepository = duelUserRepository;
            _eventBus = eventBus;
            _duelRepository = duelRepository;
            _logger = logger;
        }

        #endregion Constructor

        #region Handle method

        /// <summary>
        /// Kullanıcı rakip arama işlemlerini yapar eşleşen rakip varsa ikisi için düello başlatır yoksa sıraya alır
        /// </summary>
        public async Task Handle(WaitingOpponentIntegrationEvent @event)
        {
            if (string.IsNullOrEmpty(@event.UserId) || @event.SubCategoryId < 0 || @event.Bet < 0)
            {
                _logger.LogWarning("Duello oluşturulken değerler boş geldi. {UserId} {SubCategoryId} {Bet} {BalanceType}",
                                   @event.UserId,
                                   @event.SubCategoryId,
                                   @event.Bet,
                                   @event.BalanceType);

                return;
            }

            _logger.LogInformation("Rakip bekleniyor... {UserId} {SubCategoryId} {Bet} {BalanceType}",
                                   @event.UserId,
                                   @event.SubCategoryId,
                                   @event.Bet,
                                   @event.BalanceType);

            int duelId = _duelRepository.LastPlayingDuel(@event.UserId);
            if (duelId > 0)
            {
                _logger.LogInformation("Oyuncunun daha önceden tamamlanmamış düellosuna rastlandı. userId: {userId}", @event.UserId);

                var @eventDuelEscape = new DuelEscapeIntegrationEvent(duelId,
                                                                      @event.UserId,
                                                                      isDuelCancel: true);

                _eventBus.Publish(@eventDuelEscape);
            }

            DuelUserModel waitingDuelUser = _duelUserRepository.GetDuelUser(new DuelUserModel
            {
                BalanceType = @event.BalanceType,
                Bet = @event.Bet,
                ConnectionId = @event.ConnectionId,
                SubCategoryId = @event.SubCategoryId,
                UserId = @event.UserId
            });
            if (waitingDuelUser != null)// eğer bekleyen rakip varsa onu alır
            {
                await StartDuelAsync(@event, waitingDuelUser);
            }
            else if (!@event.UserId.EndsWith("-bot"))// rakip yoksa ve bot değilse sıraya alır
            {
                AddAwatingMode(@event);
            }
        }

        #endregion Handle method

        #region Private methods

        /// <summary>
        /// Kullanıcı redise bekleyenler arasına eklendi.
        /// </summary>
        private void AddAwatingMode(WaitingOpponentIntegrationEvent @event)
        {
            // TODO: reklam izleyerek altın aldığını history olarak tutulmalı
            if (@event.Bet == 0 && @event.BalanceType == Enums.BalanceTypes.Gold)// Eğer reklam izleyerek oynuyorsa 0 altın geliyor o zaman mininmum bahis mikatarı kadar
            {
                _logger.LogInformation("Reklam izleyerek düello yapıldı {userId}", @event.UserId);

                @event.Bet = 40;//Minimum bahis miktarı 40 altınla olduğu için 40 verdik
            }

            bool isSuccess = _duelUserRepository.Insert(new DuelUserModel
            {
                Bet = @event.Bet,
                ConnectionId = @event.ConnectionId,
                SubCategoryId = @event.SubCategoryId,
                UserId = @event.UserId,
                BalanceType = @event.BalanceType,
                Language = @event.Language
            });

            if (isSuccess)
            {
                _logger.LogInformation("Rakip bekleyen {UserId} sıraya alındı. ", @event.UserId);
            }
            else
            {
                _logger.LogCritical("CRITICAL: Kullanıcı düello sırasına eklenirken hata oluştu  {UserId}", @event.UserId);
            }
        }

        /// <summary>
        /// Singalr ile oyunu başlat
        /// </summary>
        private Task StartDuelAsync(WaitingOpponentIntegrationEvent waitingOpponentIntegration, DuelUserModel duelUserModel)
        {
            try
            {
                _duelUserRepository.Delete(duelUserModel);

                bool random = new Random().Next(1, 6) % 2 == 0;// Random bir sayı alıp tek mi çift mi diye baktık ona göre kurucu veya rakip konumuna getiriyoruz :D

                var eventDuelStart = new DuelStartIntegrationEvent(subCategoryId: waitingOpponentIntegration.SubCategoryId,
                                                            bet: waitingOpponentIntegration.Bet,

                                                            founderUserId: random ? waitingOpponentIntegration.UserId : duelUserModel.UserId,
                                                            founderConnectionId: random ? waitingOpponentIntegration.ConnectionId : duelUserModel.ConnectionId,
                                                            founderLanguage: random ? waitingOpponentIntegration.Language : duelUserModel.Language,

                                                            opponentUserId: !random ? waitingOpponentIntegration.UserId : duelUserModel.UserId,
                                                            opponentConnectionId: !random ? waitingOpponentIntegration.ConnectionId : duelUserModel.ConnectionId,
                                                            opponentLanguage: !random ? waitingOpponentIntegration.Language : duelUserModel.Language,

                                                            balanceType: duelUserModel.BalanceType);

                _eventBus.Publish(eventDuelStart);

                _logger.LogInformation($"{waitingOpponentIntegration.UserId} ile {duelUserModel.UserId} arasında düello için eşleşti.");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"CRITICAL: DuelStartIntegrationEvent publish edilirken hata oluştu. {waitingOpponentIntegration.UserId} {duelUserModel.UserId}");

                // TODO: düello başlatılırken hata oluştu
            }

            return Task.CompletedTask;
        }

        #endregion Private methods
    }
}
