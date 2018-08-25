using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Models.Duel;
using ContestPark.Mobile.Models.Duel.Quiz;
using ContestPark.Mobile.Services.Audio;
using ContestPark.Mobile.Services.Duel;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.Services.Signalr.Duel;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Prism.Navigation;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Pages;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace ContestPark.Mobile.ViewModels
{
    public class QuestionPopupViewModel : ViewModelBase
    {
        #region Private variables

        private readonly IDuelSignalRService _duelSignalRService;

        private readonly IDuelService _duelService;

        private readonly ISettingsService _settingsService;

        private readonly IAudioService _audioService;

        #endregion Private variables

        #region Constructor

        public QuestionPopupViewModel(IPopupNavigation popupNavigation,
                                      IDuelSignalRService duelSignalRService,
                                      IDuelService duelService,
                                      ISettingsService settingsService,
                                      IAudioService audioService) : base(popupNavigation: popupNavigation)
        {
            _duelSignalRService = duelSignalRService;
            _duelService = duelService;
            _settingsService = settingsService;
            _audioService = audioService;
        }

        #endregion Constructor

        #region Properties

        public bool IsFounder => _settingsService.UserInfo.UserId == DuelScreen.FounderUserId;

        public bool IsTimerEnable { get; set; }

        public bool IsStylishClick { get; set; }

        public bool IsExit { get; set; }

        public byte Round { get; set; } = 1;

        private byte _founderScore;

        public byte FounderScore
        {
            get => _founderScore;
            set
            {
                _founderScore = value;
                RaisePropertyChanged(() => FounderScore);
            }
        }

        private byte _opponentScore;

        public byte OpponentScore
        {
            get => _opponentScore;
            set
            {
                _opponentScore = value;
                RaisePropertyChanged(() => OpponentScore);
            }
        }

        private byte _time = 10;

        public byte Time
        {
            get => _time;
            set
            {
                _time = value;
                RaisePropertyChanged(() => Time);
            }
        }

        private QuestionModel _question;

        public QuestionModel Question
        {
            get => _question;
            set
            {
                _question = value;
                RaisePropertyChanged(() => Question);

                CreateAnswerPair();
            }
        }

        private AnswerPair _answers;

        public AnswerPair Answers
        {
            get => _answers;
            set
            {
                _answers = value;
                RaisePropertyChanged(() => Answers);
            }
        }

        private DuelStartingModel _duelStarting;

        public DuelStartingModel DuelScreen
        {
            get => _duelStarting;
            set
            {
                _duelStarting = value;
                RaisePropertyChanged(() => DuelScreen);
            }
        }

        public string SubcategoryName { get; set; }

        public string SubCategoryPicturePath { get; set; }

        #endregion Properties

        #region SignalR

        private void DuelSignalrListener()
        {
            _duelSignalRService.NextQuestionEventHandler += NextQuestion;
            _duelSignalRService.NextQuestion();
        }

        #endregion SignalR

        #region Methods

        protected override async Task InitializeAsync()
        {
            if (IsInitialized)
                return;

            IsInitialized = true;

            DisplayQuestionExpectedPopup();

            DuelSignalrListener();

            StartTimer();

            await base.InitializeAsync();
        }

        /// <summary>
        /// Düellodaki sıradaki soruyu alır
        /// </summary>
        private void NextQuestion(object sender, QuestionModel e)
        {
            QuestionModel questionModel = (QuestionModel)sender;
            if (questionModel == null)
            {
                // TODO: Server tarafına düello iptali için istek gönder bahis miktarı geri kullanıcıya eklensin.

                IsExit = true;
                DuelCloseCommand.Execute(null);
                return;
            }

            Device.StartTimer(new TimeSpan(0, 0, 0, 2, 0), () =>// Hemen sonraki soruya geçmemesi için 2 sn beklettim
            {
                if (IsExit)
                    return false;

                DisplayQuestionExpectedPopup();

                Device.StartTimer(new TimeSpan(0, 0, 0, 2, 0), () =>// Bekleme ekranı çıkmadan soru ekranda gözükmesin
                {
                    if (IsExit)
                        return false;

                    Question = questionModel;

                    return false;
                });

                return false;
            });
        }

        /// <summary>
        /// Bekleme ekranı gösterir belirli süre sonra gizler
        /// </summary>
        private void DisplayQuestionExpectedPopup()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            if (Round > 7) // Raund sayısı buradan değişiyor
            {
                DuelCloseCommand.Execute(null);
                return;
            }

            Time = 10;

            ResetStylishColor();

            PopupPage popupPage = QuestionExpectedPopupAsync();

            StartGame(popupPage);

            Round += 1;

            IsBusy = false;
        }

        /// <summary>
        /// Bekleme ekranı göster
        /// </summary>
        /// <returns></returns>
        private PopupPage QuestionExpectedPopupAsync()
        {
            PopupPage popupPage = new QuestionExpectedPopupView()
            {
                SubcategoryName = SubcategoryName,
                SubCategoryPicturePath = SubCategoryPicturePath,
                RoundCount = Round
            };

            Device.BeginInvokeOnMainThread(async () => await PushPopupPageAsync(popupPage));

            return popupPage;
        }

        /// <summary>
        /// Oyunu başlatır
        /// </summary>
        /// <param name="popupPage"></param>
        private void StartGame(PopupPage popupPage)
        {
            Device.StartTimer(new TimeSpan(0, 0, 0, 3, 0), () =>
            {
                if (IsExit)
                    return false;

                RemovePopupPageAsync(popupPage);

                StylishAnimation();

                IsStylishClick = true;

                Device.StartTimer(new TimeSpan(0, 0, 0, 0, 2500), () => // Soru gözükünce hemen süre başlamasın
                {
                    if (!IsExit)
                        IsTimerEnable = true;

                    return false;
                });

                return false;
            });
        }

        /// <summary>
        /// Şıkları animasyonlu şekilde gösterir
        /// </summary>
        private void StylishAnimation()
        {
            AnimateStylishCommand?.Execute(null);
        }

        /// <summary>
        /// Süreyi başlat
        /// </summary>
        private void StartTimer()
        {
            Device.StartTimer(new TimeSpan(0, 0, 0, 1, 0), () =>
            {
                if (IsStylishClick && IsTimerEnable)
                    Time -= 1;

                if (Time == 0)
                    UnableToReply();

                return !IsExit;
            });
        }

        /// <summary>
        /// Süre doldu. Soru cevaplanmadı
        /// </summary>
        private void UnableToReply()
        {
            //  IsTimeEnable = false;

            IsStylishClick = false;

            Color greenColor = GetColor("Green");

            string correctAnswer = Question.Answers.FirstOrDefault(x => x.IsCorrect).Answers;

            ChangeStylishColor(greenColor, correctAnswer);

            SaveAnswer(Stylish.UnableToReply, false);
        }

        /// <summary>
        /// Sorunun cevabını doğru bildimi kontrol eder
        /// </summary>
        /// <param name="answerModel">Şıkkın bilgisi</param>
        private async Task ExecuteAnswerCommandCommand(AnswerModel answerModel)
        {
            if (answerModel == null || !IsStylishClick)
                return;

            IsStylishClick = false;

            IsTimerEnable = false;

            Color stylishColor;

            if (answerModel.IsCorrect)
            {
                stylishColor = GetColor("Green");

                IncreaseScore();

                //   _audioService?.Play(Audio.Success);
            }
            else
            {
                stylishColor = GetColor("Red");

                //       _audioService?.Play(Audio.Fail);
            }

            ChangeStylishColor(stylishColor, answerModel.Answers);

            Stylish answerStylish = GetAnswerByUserAnswer(answerModel.Answers);

            await SaveAnswer(answerStylish, answerModel.IsCorrect);
        }

        /// <summary>
        /// Verilen cevabı sunucuya gönderir
        /// </summary>
        private async Task SaveAnswer(Stylish answer, bool isCorrect)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            string connectionId = IsFounder ? DuelScreen.OpponentConnectionId : DuelScreen.FounderConnectionId;

            await _duelService.SaveAnswer(new UserAnswer
            {
                SubcategoryId = DuelScreen.SubCategoryId,
                DuelId = DuelScreen.DuelId,
                IsCorrect = isCorrect,
                ConnectionId = connectionId,
                Time = Time,
                IsFounder = IsFounder,
                QuestionInfoId = Question.QuestionInfoId,
                CorrectAnswer = GetCorrectAnswer(),
                Stylish = answer,
                FounderConnectionId = DuelScreen.FounderConnectionId,
                FounderUserId = DuelScreen.FounderUserId,
                FounderLanguage = DuelScreen.FounderLanguage,
                OpponentConnectionId = DuelScreen.OpponentConnectionId,
                OpponentUserId = DuelScreen.OpponentUserId,
                OpponentLanguage = DuelScreen.OpponentLanguage
            });

            IsBusy = false;
        }

        /// <summary>
        /// Soru ekranını kapatır düel result ekranı açar
        /// </summary>
        private void ExecuteDuelCloseCommand()
        {
            IsExit = true;

            IsStylishClick = false;

            _duelSignalRService.NextQuestionEventHandler -= NextQuestion;
            _duelSignalRService.OffNextQuestion();

            Device.BeginInvokeOnMainThread(async () =>
            {
                await PushPopupPageAsync(new DuelResultPopupView());

                await RemoveFirstPopupAsync();
            });
        }

        /// <summary>
        /// Cevapları yan yana sıralamak için ayrı bir model oluşturduk
        /// </summary>
        private void CreateAnswerPair()
        {
            var answers = Question?.Answers;
            if (answers?.Count == 4)
            {
                Answers = new AnswerPair(answers[0], answers[1], answers[2], answers[3]);
            }
        }

        /// <summary>
        /// Puanı 10 arttıır
        /// </summary>
        private void IncreaseScore()
        {
            const byte maxScore = 10;

            if (IsFounder) FounderScore += maxScore;
            else OpponentScore += maxScore;
        }

        /// <summary>
        /// Doğru cevabın Stylish enum verir
        /// </summary>
        /// <returns>Doğru cevap</returns>
        private Stylish GetCorrectAnswer()
        {
            return (Stylish)Question
                                         .Answers
                                         .IndexOf(x => x.IsCorrect);
        }

        /// <summary>
        /// Kullanıcının verdiği cevabı Stylish enuma çevirir
        /// </summary>
        /// <param name="answer">Kullanıcının verdiği cevap</param>
        /// <returns></returns>
        private Stylish GetAnswerByUserAnswer(string answer)
        {
            return (Stylish)Question
                                          .Answers
                                          .IndexOf(x => x.Answers == answer);
        }

        /// <summary>
        /// Renk adına göre rengi döndürür
        /// </summary>
        private Color GetColor(string colorName)
        {
            return (Color)ContestParkApp.Current.Resources[colorName];
        }

        /// <summary>
        /// Tüm şıkların arka plan rengini beyaz yapar
        /// </summary>
        private void ResetStylishColor()
        {
            Color whiteColor = GetColor("White");
            Question
                .Answers
                .ForEach((answer) => answer.Color = whiteColor);
        }

        /// <summary>
        /// Cevaba göre şıkların rengini değiştiri
        /// </summary>
        /// <param name="color">Arka plan renği</param>
        /// <param name="answer">Seçilen cevap</param>
        private void ChangeStylishColor(Color color, string answer)
        {
            Question
                .Answers
                .FirstOrDefault(p => p.Answers == answer)
                .Color = color;
        }

        #endregion Methods

        #region Commands

        /// <summary>
        /// Soru ekranı kapatır
        /// </summary>
        public ICommand DuelCloseCommand => new Command(() => ExecuteDuelCloseCommand());

        private ICommand _answerCommand;

        public ICommand AnswerCommand => _answerCommand ?? (_answerCommand = new Command<AnswerModel>(async (answerModel) => await ExecuteAnswerCommandCommand(answerModel)));

        public ICommand AnimateStylishCommand;

        #endregion Commands

        #region Navigation

        public override void OnNavigatingTo(INavigationParameters parameters)
        {
        }

        #endregion Navigation
    }
}