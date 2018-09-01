using ContestPark.Core.Enums;
using ContestPark.Domain.Cp.Enums;
using ContestPark.Domain.Cp.Interfaces;
using ContestPark.Domain.Duel.Interfaces;
using ContestPark.Domain.Duel.Model.Request;
using ContestPark.Domain.Duel.Model.Response;
using ContestPark.Domain.Question.Interfaces;
using ContestPark.Domain.Question.Model.Request;
using ContestPark.Domain.Signalr.Interfaces;
using ContestPark.Domain.Signalr.Model.Request;
using ContestPark.Infrastructure.Duel.Entities;
using ContestPark.Infrastructure.Duel.Repositories.Duel;
using ContestPark.Infrastructure.Duel.Repositories.DuelInfo;
using Microsoft.Extensions.Logging;
using Orleans;
using System;
using System.Threading.Tasks;
using DuelStart = ContestPark.Domain.Duel.Model.Request.DuelStart;

namespace ContestPark.Infrastructure.Duel.Grains
{
    public class DuelGrain : Grain, IDuelGrain
    {
        #region Private variables

        private readonly IDuelRepository _duelRepository;

        private readonly IDuelInfoRepository _duelInfoRepository;

        private readonly ILogger<DuelGrain> _logger;

        #endregion Private variables

        #region Constructor

        public DuelGrain(IDuelRepository duelRepository,
            IDuelInfoRepository duelInfoRepository,
                         ILogger<DuelGrain> logger)
        {
            _duelRepository = duelRepository ?? throw new ArgumentNullException(nameof(duelRepository));
            _duelInfoRepository = duelInfoRepository;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion Constructor

        #region Methods

        #region DuelCreate

        /// <summary>
        /// Eşleşen kullanıcılara duello oluşturup DuelCreatedIntegrationEvent yayınlar
        /// </summary>
        /// <param name="duelStart">Eşleşen kullanıcı bilgileri</param>
        public async Task DuelStart(DuelStart duelStart)
        {
            // test
            duelStart.OpponentLanguage = Languages.English;

            if (string.IsNullOrEmpty(duelStart.FounderUserId) || string.IsNullOrEmpty(duelStart.OpponentUserId) || duelStart.SubCategoryId <= 0)
            {
                _logger.LogWarning($@"Duello oluşturulken değerler boş geldi.
                                    {nameof(duelStart.FounderConnectionId)}: {duelStart.FounderConnectionId}
                                    {nameof(duelStart.OpponentUserId)}: {duelStart.OpponentUserId}
                                    {nameof(duelStart.SubCategoryId)}: {duelStart.SubCategoryId}");

                return;
            }

            int duelId = _duelRepository.Insert(new DuelEntity
            {
                Bet = duelStart.Bet * 2, //İki kullanıcılı olduğu için chip miktarının 2 katını alıyoruz
                SubCategoryId = duelStart.SubCategoryId,
                FounderUserId = duelStart.FounderUserId,
                OpponentUserId = duelStart.OpponentUserId,
            });

            if (duelId <= 0)
            {
                _logger.LogWarning($"Düello oluşturulamadı. Founder user id: {duelStart.FounderUserId} Opponent user id: {duelStart.OpponentUserId} Subcateogry id: {duelStart.SubCategoryId}");
                return;
            }

            await AddToSignalrGroup(duelId,
                                    duelStart.FounderConnectionId,
                                    duelStart.OpponentConnectionId,
                                    duelStart.FounderUserId,
                                    duelStart.OpponentUserId);

            await DuelStarting(duelId, duelStart.FounderConnectionId, duelStart.OpponentConnectionId, duelStart.SubCategoryId, duelStart.FounderLanguage, duelStart.OpponentLanguage);

            _logger.LogInformation($"Düello id {duelId} oluşturuldu. Düello kullanıcı bilgileri signalr tarafına gönderilmek için hazırlanıyor.");

            await DuelCreated(duelStart, duelId);

            _logger.LogInformation($"Düello başladı. Duel Id: {duelId}");
        }

        /// <summary>
        /// Düello oluşturuldu eventini publish eder
        /// </summary>
        private async Task DuelCreated(DuelStart duelStart, int duelId)
        {
            // Duelle için statee eklendi
            await GrainFactory
                 .GetGrain<IGameGrain>(duelId)
                 .SetState(new GameState
                 {
                     DuelId = duelId,
                     SubcategoryId = duelStart.SubCategoryId,
                     Bet = duelStart.Bet * 2, //İki kullanıcılı olduğu için chip miktarının 2 katını alıyoruz

                     FounderConnectionId = duelStart.FounderConnectionId,
                     FounderLanguage = duelStart.FounderLanguage,
                     FounderUserId = duelStart.FounderUserId,

                     OpponentConnectionId = duelStart.OpponentConnectionId,
                     OpponentLanguage = duelStart.OpponentLanguage,
                     OpponentUserId = duelStart.OpponentUserId,
                 });

            if (duelStart.Bet > 0)
            {
                ICpGrain cpGrain = GrainFactory.GetGrain<ICpGrain>(1);

                await Task.WhenAll(
                   cpGrain.RemoveGold(duelStart.FounderUserId, duelStart.Bet, GoldProcessNames.Game),
                   cpGrain.RemoveGold(duelStart.OpponentUserId, duelStart.Bet, GoldProcessNames.Game));
            }

            _logger.LogInformation($@"{duelId} düello için bu kullanıcılardan bahis miktarları düşüldü.
                                   FounderUserUd: {duelStart.FounderUserId}  -  OpponentUserId: {duelStart.OpponentUserId}");

            var questionCreated = await GrainFactory
                .GetGrain<IQuestionGrain>(duelId)
                .QuestionCreate(new QuestionInfo(duelStart.SubCategoryId,
                                                 duelStart.FounderUserId,
                                                 duelStart.OpponentUserId,
                                                 duelStart.FounderLanguage,
                                                 duelStart.OpponentLanguage));

            _logger.LogInformation($"Duello soruları oluşturma isteği gönderildi Duel Id: {duelId}");

            await GrainFactory
                .GetGrain<IQuestionSignalrGrain>(duelId)
                  .NextQuestionAsync(new NextQuestion
                  {
                      DuelId = duelId,
                      Id = Guid.NewGuid(),
                      Question = questionCreated
                  });
        }

        /// <summary>
        /// Eğer kullanıcılar bot değilse duello id göre siganlr grubuna ekler
        /// </summary>
        private async Task AddToSignalrGroup(int duelId, string founderConnectionId, string opponentConnectionId, string founderUserId, string opponentUserId)
        {
            IQuestionSignalrGrain questionSignalrGrain = GrainFactory.GetGrain<IQuestionSignalrGrain>(duelId);

            if (!founderUserId.Contains("-bot") && !string.IsNullOrEmpty(founderConnectionId))
                await questionSignalrGrain.AddToGroup(duelId, founderConnectionId);

            if (!opponentUserId.Contains("-bot") && !string.IsNullOrEmpty(opponentConnectionId))
                await questionSignalrGrain.AddToGroup(duelId, opponentConnectionId);
        }

        /// <summary>
        /// Düello başlıyor eventini publish eder
        /// </summary>
        private async Task DuelStarting(int duelId, string founderConnectionId, string opponentConnectionId, Int16 subCategoryId, Languages founderLanguage, Languages opponentLanguage)
        {
            DuelStarting duelStartingModel = _duelRepository.GetDuelStarting(duelId);

            var duelScreen = new DuelStartingScreen(
                duelId: duelStartingModel.DuelId,
                // subCategoryId: subCategoryId,

                founderFullName: duelStartingModel.FounderFullName,
                founderProfilePicturePath: duelStartingModel.FounderProfilePicturePath,
                founderCoverPicturePath: duelStartingModel.FounderCoverPicturePath,
                   //  founderConnectionId: founderConnectionId,
                   founderUserId: duelStartingModel.FounderUserId,
                //  founderLanguage: founderLanguage,

                opponentFullName: duelStartingModel.OpponentFullName,
                opponentProfilePicturePath: duelStartingModel.OpponentProfilePicturePath,
                opponentCoverPicturePath: duelStartingModel.OpponentCoverPicturePath,
                //  opponentConnectionId: opponentConnectionId,
                opponentUserId: duelStartingModel.OpponentUserId
               // opponentLanguage: opponentLanguage
               );

            await GrainFactory
                    .GetGrain<IQuestionSignalrGrain>(duelId)
                    .DuelStartingScreenAsync(duelScreen);
        }

        /// <summary>
        /// Düello total scores günceller
        /// </summary>
        /// <param name="duelId">Düello id</param>
        /// <param name="founderScore">Kurucu puan</param>
        /// <param name="opponentScore">Rakip puan</param>
        public Task UpdateTotalScores(int duelId, byte founderScore, byte opponentScore)
        {
            _duelRepository.UpdateTotalScores(duelId, founderScore, opponentScore);

            return Task.CompletedTask;
        }

        #endregion DuelCreate

        #endregion Methods
    }
}