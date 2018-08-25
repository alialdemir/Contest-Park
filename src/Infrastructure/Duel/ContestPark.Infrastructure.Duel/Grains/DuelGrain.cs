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
        public Task DuelStart(DuelStart duelStart)
        {
            if (string.IsNullOrEmpty(duelStart.FounderUserId) || string.IsNullOrEmpty(duelStart.OpponentUserId) || duelStart.SubCategoryId <= 0)
            {
                _logger.LogWarning($@"Duello oluşturulken değerler boş geldi.
                                    {nameof(duelStart.FounderConnectionId)}: {duelStart.FounderConnectionId}
                                    {nameof(duelStart.OpponentUserId)}: {duelStart.OpponentUserId}
                                    {nameof(duelStart.SubCategoryId)}: {duelStart.SubCategoryId}");

                return Task.CompletedTask;
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
                return Task.CompletedTask;
            }

            DuelCreated(duelStart, duelId);

            _logger.LogInformation($"Düello id {duelId} oluşturuldu. Düello kullanıcı bilgileri signalr tarafına gönderilmek için hazırlanıyor.");

            DuelStarting(duelId, duelStart.FounderConnectionId, duelStart.OpponentConnectionId, duelStart.SubCategoryId, duelStart.FounderLanguage, duelStart.OpponentLanguage);

            _logger.LogInformation($"Düello kullanıcı bilgileri signalr tarafına gönderildi.");

            return Task.CompletedTask;
        }

        /// <summary>
        /// Düello oluşturuldu eventini publish eder
        /// </summary>
        private void DuelCreated(DuelStart duelStart, int duelId)
        {
            if (duelStart.Bet > 0)
            {
                ICpGrain cpGrain = GrainFactory.GetGrain<ICpGrain>(1);

                Task.WhenAll(
                    cpGrain.RemoveGold(duelStart.FounderUserId, duelStart.Bet, GoldProcessNames.Game),
                    cpGrain.RemoveGold(duelStart.OpponentUserId, duelStart.Bet, GoldProcessNames.Game));
            }

            _logger.LogInformation($@"{duelId} düello için bu kullanıcılardan bahis miktarları düşüldü.
                                   FounderUserUd: {duelStart.FounderUserId}  -  OpponentUserId: {duelStart.OpponentUserId}");

            GrainFactory
                .GetGrain<IQuestionGrain>(1)
                .QuestionCreate(new QuestionInfo(duelStart.SubCategoryId,
                                                          duelStart.FounderUserId,
                                                          duelStart.OpponentUserId,
                                                          duelStart.FounderConnectionId,
                                                          duelStart.OpponentConnectionId,
                                                          duelStart.FounderLanguage,
                                                          duelStart.OpponentLanguage));

            _logger.LogInformation($"Duello soruları oluşturma isteği gönderildi Duel Id: {duelId}");
        }

        /// <summary>
        /// Düello başlıyor eventini publish eder
        /// </summary>
        private void DuelStarting(int duelId, string founderConnectionId, string opponentConnectionId, Int16 subCategoryId, Languages founderLanguage, Languages opponentLanguage)
        {
            DuelStarting duelStartingModel = _duelRepository.GetDuelStarting(duelId);

            var duelScreen = new DuelStartingScreen(
                duelId: duelStartingModel.DuelId,
                subCategoryId: subCategoryId,

                founderFullName: duelStartingModel.FounderFullName,
                founderProfilePicturePath: duelStartingModel.FounderProfilePicturePath,
                founderCoverPicturePath: duelStartingModel.FounderCoverPicturePath,
                founderConnectionId: founderConnectionId,
                founderUserId: duelStartingModel.FounderUserId,
                founderLanguage: founderLanguage,

                opponentFullName: duelStartingModel.OpponentFullName,
                opponentProfilePicturePath: duelStartingModel.OpponentProfilePicturePath,
                opponentCoverPicturePath: duelStartingModel.OpponentCoverPicturePath,
                opponentConnectionId: opponentConnectionId,
                opponentUserId: duelStartingModel.OpponentUserId,
                opponentLanguage: opponentLanguage);

            GrainFactory
                .GetGrain<IQuestionSignalrGrain>(1)
                .DuelStartingScreenAsync(duelScreen);
        }

        #endregion DuelCreate

        #region User answer save

        /// <summary>
        /// Oyunucunun verdiği cevabı kayıt eder
        /// </summary>
        /// <param name="userAnswer">Duello bilgisi</param>
        public Task SaveUserAnswer(UserAnswer userAnswer)
        {
            bool isGameEnd = IsGameEnd(userAnswer.DuelId).Result;

            if (isGameEnd)
                return Task.CompletedTask;

            #region Cevap kayıt edildi

            DuelInfoEntity duelInfoEntity = _duelInfoRepository.GetDuelInfoByDuelId(userAnswer.DuelId, userAnswer.QuestionInfoId) ?? new DuelInfoEntity();

            duelInfoEntity.DuelId = userAnswer.DuelId;
            duelInfoEntity.CorrectAnswer = userAnswer.CorrectAnswer;
            duelInfoEntity.QuestionInfoId = userAnswer.QuestionInfoId;

            if (userAnswer.IsFounder)
            {
                duelInfoEntity.FounderAnswer = userAnswer.Stylish;
                duelInfoEntity.FounderScore = userAnswer.IsCorrect ? (byte)10 : (byte)0;
                duelInfoEntity.FounderTime = userAnswer.Time;
            }
            else
            {
                duelInfoEntity.OpponentAnswer = userAnswer.Stylish;
                duelInfoEntity.OpponentScore = userAnswer.IsCorrect ? (byte)10 : (byte)0;
                duelInfoEntity.OpponentTime = userAnswer.Time;
            }

            if (duelInfoEntity.DuelInfoId > 0) _duelInfoRepository.Update(duelInfoEntity);
            else _duelInfoRepository.Insert(duelInfoEntity);

            #endregion Cevap kayıt edildi

            return Task.CompletedTask;
        }

        /// <summary>
        /// Düelloda soruyu cevaplayınca yapılacak işlemleri  çalıştırır
        /// </summary>
        /// <param name="userAnswer">Düello bilgisi</param>
        public async Task SaveUserAnswerProcess(UserAnswer userAnswer)
        {
            IDuelGrain duelGrain = GrainFactory.GetGrain<IDuelGrain>(1);

            await duelGrain.SaveUserAnswer(userAnswer);

            bool isGameEnd = await duelGrain.IsGameEnd(userAnswer.DuelId);

            if (!isGameEnd)
            {
                // Sonraki soruya geçildi
                await GrainFactory
                    .GetGrain<IQuestionGrain>(1)
                    .QuestionCreate(new QuestionInfo(
                        userAnswer.SubcategoryId,
                        userAnswer.FounderUserId,
                        userAnswer.OpponentUserId,
                        userAnswer.FounderConnectionId,
                        userAnswer.OpponentConnectionId,
                        userAnswer.FounderLanguage,
                        userAnswer.OpponentLanguage));
            }

            // TODO: Add to score
            // TODO: Add to post
            // TODO: Mission control
            // TODO: Level control
        }

        /// <summary>
        /// Düelloda 7 sorudan fazla soru sorulduysa false sorulmadıysa true döner
        /// </summary>
        /// <param name="duelId">Düello id</param>
        /// <returns>Oyun bitti ise true bitmedi ise false</returns>
        public Task<bool> IsGameEnd(int duelId)
        {
            return Task.FromResult(_duelInfoRepository.IsGameEnd(duelId));
        }

        #endregion User answer save

        #endregion Methods
    }
}