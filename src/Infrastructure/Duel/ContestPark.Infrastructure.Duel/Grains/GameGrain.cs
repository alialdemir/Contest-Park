using ContestPark.Domain.Cp.Interfaces;
using ContestPark.Domain.Duel.Enums;
using ContestPark.Domain.Duel.Interfaces;
using ContestPark.Domain.Duel.Model.Request;
using ContestPark.Domain.Duel.Model.Response;
using ContestPark.Domain.Question.Interfaces;
using ContestPark.Domain.Question.Model.Request;
using ContestPark.Domain.Question.Model.Response;
using ContestPark.Domain.Signalr.Interfaces;
using ContestPark.Infrastructure.Duel.Entities;
using ContestPark.Infrastructure.Duel.Repositories.DuelInfo;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Infrastructure.Duel.Grains
{
    [StorageProvider(ProviderName = "GrainStorage")]
    public class GameGrain : Grain<GameGrainState>, IGameGrain
    {
        #region Private variables

        private readonly byte _gameCount = 7;//Kaç soru soralabileceği

        private readonly IDuelInfoRepository _duelInfoRepository;

        private readonly ILogger<GameGrain> _logger;

        private IQuestionGrain _questionGrain;

        private IQuestionSignalrGrain _questionSignalrGrain;

        private IDuelGrain _duelGrain;

        private ICpGrain _cpGrain;

        #endregion Private variables

        #region Constructor

        public GameGrain(IDuelInfoRepository duelInfoRepository,
                         ILogger<GameGrain> logger)
        {
            _duelInfoRepository = duelInfoRepository;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Düelloda 7 sorudan fazla soru sorulduysa false sorulmadıysa true döner
        /// </summary>
        /// <returns>Oyun bitti ise true bitmedi ise false</returns>
        private bool IsGameEnd
        {
            get
            {
                return this.State.DuelInfos.Count >= _gameCount;
            }
        }

        #endregion Properties

        #region Methods

        #region override

        public override Task OnActivateAsync()
        {
            long primaryId = this.GetPrimaryKeyLong();

            _questionGrain = GrainFactory.GetGrain<IQuestionGrain>(primaryId);

            _questionSignalrGrain = GrainFactory.GetGrain<IQuestionSignalrGrain>(primaryId);

            _cpGrain = GrainFactory.GetGrain<ICpGrain>(primaryId);

            _duelGrain = GrainFactory.GetGrain<IDuelGrain>(primaryId);

            return base.OnActivateAsync();
        }

        public override async Task OnDeactivateAsync()
        {
            await _duelInfoRepository.InsertAsync(this.State.DuelInfos.Values);

            await _questionSignalrGrain.RemoveGroup(this.State.Game.DuelId, this.State.Game.FounderConnectionId, this.State.Game.OpponentConnectionId);

            await _questionGrain.OnDeactivateAsync();

            await _questionSignalrGrain.OnDeactivateAsync();

            await _cpGrain.OnDeactivateAsync();

            await _duelGrain.OnDeactivateAsync();

            _logger.LogInformation($"Game grain deactivate. Primary Key: {this.GetPrimaryKeyLong()}");

            await base.OnDeactivateAsync();
        }

        #endregion override

        #region User answer save

        /// <summary>
        /// Düelloda soruyu cevaplayınca yapılacak işlemleri  çalıştırır
        /// </summary>
        /// <param name="userAnswer">Düello bilgisi</param>
        public async Task SaveUserAnswerProcess(UserAnswer userAnswer)
        {
            if (this.State?.Game == null)
                return;

            DuelInfoEntity duelInfoEntity = SaveUserAnswer(userAnswer);

            #region Sonraki soruya geçildi

            if (IsQuestionAnswered(userAnswer.Id))
            {
                NextQuestion nextQuestion = new NextQuestion()
                {
                    DuelId = this.State.Game.DuelId,
                    FounderStylish = duelInfoEntity.FounderAnswer,
                    OpponentStylish = duelInfoEntity.OpponentAnswer,
                    FounderScore = duelInfoEntity.FounderScore,
                    OpponentScore = duelInfoEntity.OpponentScore,
                    Id = Guid.NewGuid(),
                    IsGameEnd = IsGameEnd
                };

                if (!IsGameEnd)
                {
                    nextQuestion.Question = await _questionGrain
                                                       .QuestionCreate(new QuestionInfo(
                                                           this.State.Game.SubcategoryId,
                                                           this.State.Game.FounderUserId,
                                                           this.State.Game.OpponentUserId,
                                                           this.State.Game.FounderLanguage,
                                                           this.State.Game.OpponentLanguage));
                }
                else nextQuestion.Question = new Question();

                await _questionSignalrGrain.NextQuestionAsync(nextQuestion);

                if (IsGameEnd)// Oyun bittiyse signalr group silindi
                {
                    _logger.LogInformation($"Düello sona erdi. Duel Id: {this.State.Game.DuelId}");

                    AddWinnerGold();

                    UpdateTotalScores();

                    await this.OnDeactivateAsync();// Grain deactive
                }
            }

            #endregion Sonraki soruya geçildi

            // TODO: Add to score
            // TODO: Add to post
            // TODO: Mission control
            // TODO: Level control
        }

        /// <summary>
        /// Düello tablosuna toplan skorları günceller
        /// </summary>
        private void UpdateTotalScores()
        {
            byte founderScore = (byte)this.State.DuelInfos.Sum(p => p.Value.FounderScore);
            byte opponentScore = (byte)this.State.DuelInfos.Sum(p => p.Value.OpponentScore);

            _duelGrain.UpdateTotalScores(this.State.Game.DuelId, founderScore, opponentScore);
        }

        /// <summary>
        /// Düello sonunda kazanana altını altarır
        /// </summary>
        private void AddWinnerGold()
        {
            byte founderScore = (byte)this.State.DuelInfos.Sum(p => p.Value.FounderScore);
            byte opponentScore = (byte)this.State.DuelInfos.Sum(p => p.Value.OpponentScore);

            if (founderScore == opponentScore)
            {
                int gold = this.State.Game.Bet / 2;// iki oyunculu ve berabere bittiği için ikiye böldük

                Task.WhenAll(
                    _cpGrain.AddGold(this.State.Game.FounderUserId, gold, Domain.Cp.Enums.GoldProcessNames.Draw),
                    _cpGrain.AddGold(this.State.Game.OpponentUserId, gold, Domain.Cp.Enums.GoldProcessNames.Draw));
            }
            else if (founderScore > opponentScore)
            {
                _cpGrain.AddGold(this.State.Game.FounderUserId, this.State.Game.Bet, Domain.Cp.Enums.GoldProcessNames.Win);
            }
            else if (opponentScore > founderScore)
            {
                _cpGrain.AddGold(this.State.Game.OpponentUserId, this.State.Game.Bet, Domain.Cp.Enums.GoldProcessNames.Win);
            }
        }

        /// <summary>
        /// Soruyu iki kullanıcıda cevapladı mı onu kontrol eder
        /// </summary>
        /// <param name="questionInfoId">Soru id</param>
        /// <returns>Eğer iki tarafta soruyu cevapladıysa true cevaplamadıysa false</returns>
        private bool IsQuestionAnswered(Guid key)
        {
            DuelInfoEntity duelInfoEntity = this.State.DuelInfos[key];

            if (duelInfoEntity == null)
                return false;

            return (duelInfoEntity.FounderAnswer == Stylish.A
                || duelInfoEntity.FounderAnswer == Stylish.B
                || duelInfoEntity.FounderAnswer == Stylish.C
                || duelInfoEntity.FounderAnswer == Stylish.D
                || duelInfoEntity.FounderAnswer == Stylish.UnableToReply)
                && (duelInfoEntity.OpponentAnswer == Stylish.A
                || duelInfoEntity.OpponentAnswer == Stylish.B
                || duelInfoEntity.OpponentAnswer == Stylish.C
                || duelInfoEntity.OpponentAnswer == Stylish.D
                || duelInfoEntity.OpponentAnswer == Stylish.UnableToReply);
        }

        /// <summary>
        /// Oyunucunun verdiği cevabı kayıt eder
        /// </summary>
        /// <param name="userAnswer">Duello bilgisi</param>
        private DuelInfoEntity SaveUserAnswer(UserAnswer userAnswer)
        {
            DuelInfoEntity duelInfoEntity = this.State.DuelInfos.ContainsKey(userAnswer.Id)
                                            ? this.State.DuelInfos[userAnswer.Id]
                                            : new DuelInfoEntity();

            duelInfoEntity.DuelId = this.State.Game.DuelId;
            duelInfoEntity.CorrectAnswer = userAnswer.CorrectAnswer;
            duelInfoEntity.QuestionInfoId = userAnswer.QuestionInfoId;

            bool isCorrect = userAnswer.CorrectAnswer == userAnswer.Stylish;
            byte score = isCorrect ? ScoreCalculator(userAnswer.Time) : (byte)0;

            if (userAnswer.IsFounder)
            {
                duelInfoEntity.FounderAnswer = userAnswer.Stylish;
                duelInfoEntity.FounderScore = score;
                duelInfoEntity.FounderTime = userAnswer.Time;
            }
            else
            {
                duelInfoEntity.OpponentAnswer = userAnswer.Stylish;
                duelInfoEntity.OpponentScore = score;
                duelInfoEntity.OpponentTime = userAnswer.Time;
            }

            this.State.DuelInfos[userAnswer.Id] = duelInfoEntity;

            return duelInfoEntity;
        }

        /// <summary>
        /// Süreye göre puan hesaplar soru başına maksimum 20 puan alabilir
        /// </summary>
        /// <param name="time">Soruya kaçıncı saniyede cevapladı</param>
        /// <returns>Aldığı puan</returns>
        private byte ScoreCalculator(byte time)
        {
            const byte maxTime = 10;// Maksimum oyun süresi

            return Convert.ToByte((time * 2) + (maxTime - time));
        }

        public Task SetState(GameState gameState)
        {
            this.State.Game = gameState;

            return Task.CompletedTask;
        }

        #endregion User answer save

        #endregion Methods
    }

    public class GameGrainState
    {
        public Dictionary<Guid, DuelInfoEntity> DuelInfos { get; set; } = new Dictionary<Guid, DuelInfoEntity>();

        public GameState Game { get; set; }
    }
}