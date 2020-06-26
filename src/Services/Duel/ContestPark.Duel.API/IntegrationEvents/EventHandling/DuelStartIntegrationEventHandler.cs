using ContestPark.Core.Models;
using ContestPark.Core.Services.Identity;
using ContestPark.Duel.API.Enums;
using ContestPark.Duel.API.Infrastructure.Repositories.ContestDate;
using ContestPark.Duel.API.Infrastructure.Repositories.Duel;
using ContestPark.Duel.API.Infrastructure.Repositories.Question;
using ContestPark.Duel.API.Infrastructure.Repositories.Redis.UserAnswer;
using ContestPark.Duel.API.IntegrationEvents.Events;
using ContestPark.Duel.API.Models;
using ContestPark.Duel.API.Resources;
using ContestPark.Duel.API.Services.SubCategory;
using ContestPark.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Duel.API.IntegrationEvents.EventHandling
{
    public class DuelStartIntegrationEventHandler : IIntegrationEventHandler<DuelStartIntegrationEvent>
    {
        #region Private variables

        private readonly IEventBus _eventBus;
        private readonly IDuelRepository _duelRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IUserAnswerRepository _userAnswerRepository;
        private readonly IIdentityService _identityService;
        private readonly IContestDateRepository _contestDateRepository;
        private readonly ISubCategoryService _subCategoryService;
        private readonly DuelSettings _duelSettings;
        private readonly ILogger<DuelStartIntegrationEventHandler> _logger;

        #endregion Private variables

        #region Constructor

        public DuelStartIntegrationEventHandler(IEventBus eventBus,
                                                IDuelRepository duelRepository,
                                                IQuestionRepository questionRepository,
                                                IUserAnswerRepository userAnswerRepository,
                                                IIdentityService identityService,
                                                IContestDateRepository contestDateRepository,
                                                IOptions<DuelSettings> settings,
                                                ISubCategoryService subCategoryService,
                                                ILogger<DuelStartIntegrationEventHandler> logger)
        {
            _logger = logger;
            _eventBus = eventBus;
            _duelRepository = duelRepository;
            _questionRepository = questionRepository;
            _userAnswerRepository = userAnswerRepository;
            _identityService = identityService;
            _contestDateRepository = contestDateRepository;
            _subCategoryService = subCategoryService;
            _duelSettings = settings.Value;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Kullanıcılar arasında düello başlat 7 soru çek kullanıcılara gönder
        /// </summary>
        /// <param name="event">Düello bilgileri</param>
        public async Task Handle(DuelStartIntegrationEvent @event)
        {
            if (@event.BalanceType == BalanceTypes.Money && !(@event.FounderUserId.EndsWith("-bot") || @event.OpponentUserId.EndsWith("-bot")))
            {
                _logger.LogInformation("İki oyuncu para ile düello yapıyor. {FounderUserId} {OpponentUserId}", @event.FounderUserId, @event.OpponentUserId);

                return;
            }

            ContestDateModel contestDate = _contestDateRepository.ActiveContestDate();
            if (contestDate == null)
            {
                _logger.LogCritical("CRITICAL: Yarışma tarihi alınırken null kayıt geldi. Lütfen acil aktif yarışma tarihi(ContestDates tablosu) var mı kontrol edelim!");

                SendErrorMessage(@event.FounderUserId, DuelResource.ErrorStartingDuelPleaseTryAgain);
                SendErrorMessage(@event.OpponentUserId, DuelResource.ErrorStartingDuelPleaseTryAgain);

                return;
            }

            if (@event.Bet <= 0 && @event.BalanceType == BalanceTypes.Gold)
                @event.Bet = 40.00m;

            int duelId = await _duelRepository.Insert(new Infrastructure.Tables.Duel
            {
                BalanceType = @event.BalanceType,
                Bet = @event.Bet,
                DuelType = DuelTypes.Created,
                SubCategoryId = @event.SubCategoryId,
                OpponentUserId = @event.OpponentUserId,
                FounderUserId = @event.FounderUserId,
                ContestDateId = contestDate.ContestDateId,
                BetCommission = _duelSettings.BetCommission
            }) ?? 0;
            if (duelId <= 0)
            {
                _logger.LogCritical("CRITICAL: Düello başlatılamadı. {FounderUserId} {OpponentUserId} {BalanceType} {SubCategoryId} {Bet}",
                                    @event.FounderUserId,
                                    @event.OpponentUserId,
                                    @event.BalanceType,
                                    @event.SubCategoryId,
                                    @event.Bet);

                SendErrorMessage(@event.FounderUserId, DuelResource.ErrorStartingDuelPleaseTryAgain);
                SendErrorMessage(@event.OpponentUserId, DuelResource.ErrorStartingDuelPleaseTryAgain);

                return;
            }

            var questions = await _questionRepository.DuelQuestions(@event.SubCategoryId,
                                                                    @event.FounderUserId,
                                                                    @event.OpponentUserId,
                                                                    @event.FounderLanguage,
                                                                    @event.OpponentLanguage);
            DuelWinStatusModel winStatus = null;

            if (@event.FounderUserId.EndsWith("-bot") || @event.OpponentUserId.EndsWith("-bot"))
            {
                try
                {
                    string userId = @event.FounderUserId.EndsWith("-bot") ? @event.OpponentUserId : @event.FounderUserId;

                    winStatus = _duelRepository.WinStatus(userId, @event.BalanceType);
                }
                catch (System.Exception ex)
                {
                    _logger.LogError("WinStatus {message}", ex.Message);
                }
            }

            // Sorulara cevap verildiğinde kontrol edebilmek için redise eklendi
            _userAnswerRepository.AddRangeAsync(questions.Select(x => new UserAnswerModel
            {
                DuelId = duelId,
                QuestionId = x.QuestionId,
                FounderUserId = @event.FounderUserId,
                OpponentUserId = @event.OpponentUserId,
                CorrectAnswer = (Stylish)(x.Answers.FindIndex(a => a.IsCorrectAnswer) + 1),
                FounderAnswer = Stylish.NotSeeQuestion,
                OpponentAnswer = Stylish.NotSeeQuestion,
                WinStatus = winStatus,
            }).ToList());

            // Bakiyeler düşüldü
            ChangeBalance(@event.FounderUserId, @event.Bet, @event.BalanceType);
            ChangeBalance(@event.OpponentUserId, @event.Bet, @event.BalanceType);

            IEnumerable<UserLevelModel> userLevels = await _subCategoryService.UserLevel(new List<string>
            {
                @event.FounderUserId,
                @event.OpponentUserId
            }, @event.SubCategoryId);

            try
            {
                if (userLevels != null && (userLevels.Any(x => x.UserId == @event.FounderUserId) || userLevels.Any(x => x.UserId == @event.OpponentUserId)))
                {
                    short founderLevel = userLevels.FirstOrDefault(x => x.UserId == @event.FounderUserId).Level;
                    short opponentLevel = userLevels.FirstOrDefault(x => x.UserId == @event.OpponentUserId).Level;

                    await PublishDuelCreatedEvent(duelId,
                                                  @event.FounderUserId,
                                                  @event.FounderConnectionId,
                                                  founderLevel,
                                                  @event.OpponentUserId,
                                                  @event.OpponentConnectionId,
                                                  opponentLevel,
                                                  questions);
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex);
            }

            _logger.LogInformation("Düello başlatıldı. {duelId} {FounderUserId} {OpponentUserId} {BalanceType} {SubCategoryId} {Bet}",
                                   duelId,
                                   @event.FounderUserId,
                                   @event.OpponentUserId,
                                   @event.BalanceType,
                                   @event.SubCategoryId,
                                   @event.Bet);
        }

        /// <summary>
        /// DuelCreatedIntegrationEvent publish eder
        /// </summary>
        /// <param name="duelId">Düello id</param>
        /// <param name="founderUserId">Kurucu kullanıcı id</param>
        /// <param name="founderConnectionId">Kurucu connection id</param>
        /// <param name="opponentUserId">Rakip kullanıcı id</param>
        /// <param name="opponentConnectionId">Rakip connection id</param>
        /// <param name="questions">Düelloda sorulacak sorular</param>
        private async Task PublishDuelCreatedEvent(int duelId,
                                                   string founderUserId,
                                                   string founderConnectionId,
                                                   short founderLevel,
                                                   string opponentUserId,
                                                   string opponentConnectionId,
                                                   short opponentLevel,
                                                   IEnumerable<QuestionModel> questions)
        {
            // TODO: #issue 213

            // Eşleşen rakipler rakip bulubdu ekranı için event gönderildi
            List<UserModel> userInfos = (await _identityService.GetUserInfosAsync(new List<string>// identity servisden kullanıcı bilgileri alındı
                {
                    founderUserId,
                    opponentUserId
           }, includeCoverPicturePath: true)).ToList();

            UserModel founderUserModel = userInfos.FirstOrDefault(x => x.UserId == founderUserId);
            UserModel opponentUserModel = userInfos.FirstOrDefault(x => x.UserId == opponentUserId);

            if (founderUserModel == null || opponentUserModel == null)
            {
                _logger.LogCritical("CRITICAL: Düello oluştu fakat kullanıcı bilgilerine erişemedim ACİL bakın!");

                return;
            }

            var @duelEvent = new DuelCreatedIntegrationEvent(duelId,
                                                             questions,
                                                             founderUserModel.CoverPicturePath,
                                                             founderUserModel.ProfilePicturePath,
                                                             founderUserModel.UserId,
                                                             founderConnectionId,
                                                             founderUserModel.FullName,
                                                             founderLevel,
                                                             opponentUserModel.CoverPicturePath,
                                                             opponentUserModel.FullName,
                                                             opponentUserModel.ProfilePicturePath,
                                                             opponentUserModel.UserId,
                                                             opponentLevel,
                                                             opponentConnectionId);

            _eventBus.Publish(duelEvent);
        }

        /// <summary>
        /// Signalr ile clients hata mesajı gönderir
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <param name="message">Gönderilecek mesaj</param>
        private void SendErrorMessage(string userId, string message)
        {
            var @event = new SendErrorMessageWithSignalrIntegrationEvent(userId, message);

            _eventBus.Publish(@event);
        }

        private void ChangeBalance(string userId, decimal bet, BalanceTypes balanceType)
        {
            if (bet <= 0)
                return;

            bet = -bet;

            var @event = new ChangeBalanceIntegrationEvent(bet, userId, balanceType, BalanceHistoryTypes.Duel);

            _eventBus.Publish(@event);
        }

        #endregion Methods
    }
}
