using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Models.Duel;
using ContestPark.Mobile.Models.Duel.Quiz;
using ContestPark.Mobile.Services.Audio;
using ContestPark.Mobile.Services.Bot;
using ContestPark.Mobile.Services.Duel;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.Services.Signalr.Duel;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Pages;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace ContestPark.Mobile.ViewModels
{
    public class QuestionPopupViewModel : ViewModelBase
    {
        #region Private variables

        private readonly IAudioService _audioService;
        private readonly IBotService _botService;
        private readonly IDuelService _duelService;
        private readonly IDuelSignalRService _duelSignalRService;
        private readonly ISettingsService _settingsService;

        #endregion Private variables

        #region Constructor

        public QuestionPopupViewModel(IPopupNavigation popupNavigation,
                                      IDuelSignalRService duelSignalRService,
                                      IDuelService duelService,
                                      ISettingsService settingsService,
                                      IAudioService audioService,
                                      IBotService botService) : base(popupNavigation: popupNavigation)
        {
            _duelSignalRService = duelSignalRService;
            _duelService = duelService;
            _settingsService = settingsService;
            _audioService = audioService;
            _botService = botService;

            FounderImageBorderColor = PrimaryColor;
            OpponentImageBorderColor = PrimaryColor;
        }

        #endregion Constructor

        #region Properties

        private AnswerPair _answers;

        private DuelStartingModel _duelStarting;

        private string _founderImageBorderColor;

        private byte _founderScore;

        private string _opponentImageBorderColor;

        private byte _opponentScore;

        private NextQuestion _question;

        private byte _time = 10;

        public AnswerPair Answers
        {
            get => _answers;
            set
            {
                _answers = value;
                RaisePropertyChanged(() => Answers);
            }
        }

        public DuelStartingModel DuelScreen
        {
            get => _duelStarting;
            set
            {
                _duelStarting = value;
                RaisePropertyChanged(() => DuelScreen);
            }
        }

        public string FounderImageBorderColor
        {
            get => _founderImageBorderColor;
            set
            {
                _founderImageBorderColor = value;
                RaisePropertyChanged(() => FounderImageBorderColor);
            }
        }

        public byte FounderScore
        {
            get => _founderScore;
            set
            {
                _founderScore = value;
                RaisePropertyChanged(() => FounderScore);
            }
        }

        public string GreenColor
        {
            get { return "#017d46"; }
        }

        public string OpponentImageBorderColor
        {
            get => _opponentImageBorderColor;
            set
            {
                _opponentImageBorderColor = value;
                RaisePropertyChanged(() => OpponentImageBorderColor);
            }
        }

        public byte OpponentScore
        {
            get => _opponentScore;
            set
            {
                _opponentScore = value;
                RaisePropertyChanged(() => OpponentScore);
            }
        }

        public string PrimaryColor
        {
            get { return "#ffc107"; }
        }

        public NextQuestion Question
        {
            get => _question;
            set
            {
                _question = value;
                RaisePropertyChanged(() => Question);

                CreateAnswerPair();
            }
        }

        public string RedColor
        {
            get { return "#993232"; }
        }

        public byte Round { get; set; } = 1;
        public string SubcategoryName { get; set; }
        public string SubCategoryPicturePath { get; set; }

        public byte Time
        {
            get => _time;
            set
            {
                _time = value;
                RaisePropertyChanged(() => Time);
            }
        }

        private bool IsExit { get; set; }
        private bool IsFounder => _settingsService.CurrentUser.UserId == DuelScreen.FounderUserId;

        private bool IsStylishClick { get; set; }
        private bool IsTimerEnable { get; set; }

        #endregion Properties

        #region SignalR

        private void DuelSignalrListener()
        {
            _duelSignalRService.NextQuestionEventHandler += NextQuestion;

            _duelSignalRService.NextQuestion();
        }

        #endregion SignalR

        #region Methods

        protected override Task InitializeAsync()
        {
            if (IsInitialized)
                return Task.CompletedTask;

            ResetImageBorderColor();

            IsInitialized = true;

            DisplayQuestionExpectedPopup();

            DuelSignalrListener();

            StartTimer();

            base.InitializeAsync();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Botu aktif eder
        /// </summary>
        private void BotActive()
        {
            if (DuelScreen.FounderUserId.Contains("-bot") || DuelScreen.OpponentUserId.Contains("-bot"))// Eğer bot ile oynuyorsa oyuna bot eklendi
                _botService.Init(SaveAnswer, IsFounder);
        }

        /// <summary>
        /// Verilen cevaplara göre resimlerin border color değiştirir
        /// </summary>
        /// <param name="founderStylish"></param>
        /// <param name="opponentStylish"></param>
        private void ChangeImageBorderColor(Stylish founderStylish, Stylish opponentStylish)
        {
            Stylish correctAnswer = GetCorrectAnswer();
            if (IsFounder)
            {
                bool isCorrectFounder = correctAnswer == founderStylish;
                FounderImageBorderColor = isCorrectFounder ? GreenColor : RedColor;
            }
            else
            {
                bool isCorrectOpponent = correctAnswer == opponentStylish;
                OpponentImageBorderColor = isCorrectOpponent ? GreenColor : RedColor;
            }
        }

        /// <summary>
        /// Verilen cevaba göre şıkların renklerini değiştirir
        /// </summary>
        private void ChangeStylishColor(Stylish stylish)
        {
            byte index = (byte)stylish;

            if (index < 0 || index >= Question?.Question?.Answers?.Count)
                return;

            Stylish correctAnswer = GetCorrectAnswer();

            bool isCorrect = correctAnswer == stylish;

            Color color = GetColor(isCorrect ? "Green" : "Red");

            Question
                .Question
                .Answers[index]
                .Color = color;
        }

        /// <summary>
        /// Cevapları yan yana sıralamak için ayrı bir model oluşturduk
        /// </summary>
        private void CreateAnswerPair()
        {
            var answers = Question?.Question?.Answers;
            if (answers?.Count == 4)
            {
                Answers = new AnswerPair(answers[0], answers[1], answers[2], answers[3]);
            }
        }

        /// <summary>
        /// Bekleme ekranı gösterir belirli süre sonra gizler
        /// </summary>
        private void DisplayQuestionExpectedPopup()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            Time = 10;

            ResetImageBorderColor();

            ResetStylishColor();

            PopupPage popupPage = QuestionExpectedPopup();

            StartGame(popupPage);

            Round += 1;

            IsBusy = false;
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

            Stylish answerStylish = GetAnswerByUserAnswer(answerModel.Answers);

            await SaveAnswer(answerStylish, IsFounder);
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
        /// Oyun bittiği zaman 2 sn sonra duel result ekranına gider
        /// </summary>
        private void GameEnd()
        {
            Device.StartTimer(new TimeSpan(0, 0, 0, 2, 0), () => // Son sorunun cevabınıda görsün
            {
                if (IsExit)
                    return false;

                DuelCloseCommand.Execute(null);

                return false;
            });
        }

        /// <summary>
        /// Kullanıcının verdiği cevabı Stylish enuma çevirir
        /// </summary>
        /// <param name="answer">Kullanıcının verdiği cevap</param>
        /// <returns></returns>
        private Stylish GetAnswerByUserAnswer(string answer)
        {
            return (Stylish)Question
                .Question
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
        /// Doğru cevabın Stylish enum verir
        /// </summary>
        /// <returns>Doğru cevap</returns>
        private Stylish GetCorrectAnswer()
        {
            return (Stylish)Question
                .Question
                .Answers
                .IndexOf(x => x.IsCorrect);
        }

        /// <summary>
        /// Düellodaki sıradaki soruyu alır
        /// </summary>
        private void NextQuestion(object sender, NextQuestion e)
        {
            NextQuestion questionModel = (NextQuestion)sender;
            if (questionModel == null)
            {
                // TODO: Server tarafına düello iptali için istek gönder bahis miktarı geri kullanıcıya eklensin.

                IsExit = true;
                DuelCloseCommand.Execute(null);

                return;
            }

            IsTimerEnable = false;

            FounderScore += questionModel.FounderScore;
            OpponentScore += questionModel.OpponentScore;

            ChangeStylishColor(questionModel.FounderStylish);
            ChangeStylishColor(questionModel.OpponentStylish);

            PlaySound(questionModel.FounderStylish, questionModel.OpponentStylish);

            ChangeImageBorderColor(questionModel.FounderStylish, questionModel.OpponentStylish);

            if (questionModel.IsGameEnd || questionModel?.Question == null || questionModel.Question.Questions?.Count == 0)
            {
                GameEnd();
            }
            else if (questionModel?.Question != null)
            {
                Languages currentLanguage = _settingsService.CurrentUser.Language;
                questionModel.Question = questionModel.Question.GetQuestionByLanguage(currentLanguage);

                Device.StartTimer(new TimeSpan(0, 0, 0, 2, 0), () => // Hemen sonraki soruya geçmemesi için 2 sn beklettim
                {
                    if (IsExit)
                        return false;

                    DisplayQuestionExpectedPopup();

                    Device.StartTimer(new TimeSpan(0, 0, 0, 2, 0), () => // Bekleme ekranı çıkmadan soru ekranda gözükmesin
                    {
                        if (IsExit)
                            return false;

                        Question = questionModel;

                        return false;
                    });

                    return false;
                });
            }
        }

        /// <summary>
        /// Verilen cevaplara göre sesleri çalar
        /// </summary>
        /// <param name="founderStylish">Kurucunun verdiği cevap</param>
        /// <param name="opponentStylish">Rakip oyuncunun verdiği cevap</param>
        private void PlaySound(Stylish founderStylish, Stylish opponentStylish)
        {
            Stylish correctAnswer = GetCorrectAnswer();
            if (IsFounder)
            {
                bool isCorrectFounder = correctAnswer == founderStylish;

                _audioService?.Play(isCorrectFounder ? Audio.Success : Audio.Fail);
            }
            else
            {
                bool isCorrectOpponent = correctAnswer == opponentStylish;
                _audioService?.Play(isCorrectOpponent ? Audio.Success : Audio.Fail);
            }
        }

        /// <summary>
        /// Bekleme ekranı göster
        /// </summary>
        private PopupPage QuestionExpectedPopup()
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
        /// Kullanıcı resimlerinin borderleri default renği alır
        /// </summary>
        private void ResetImageBorderColor()
        {
            FounderImageBorderColor = PrimaryColor;

            OpponentImageBorderColor = PrimaryColor;
        }

        /// <summary>
        /// Tüm şıkların arka plan rengini beyaz yapar
        /// </summary>
        private void ResetStylishColor()
        {
            Color whiteColor = GetColor("White");
            Question
                .Question
                .Answers
                .ForEach((answer) => answer.Color = whiteColor);
        }

        /// <summary>
        /// Verilen cevabı sunucuya gönderir
        /// </summary>
        private async Task SaveAnswer(Stylish answer, bool isFounder)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            await _duelSignalRService.SaveAnswer(new UserAnswer
            {
                DuelId = DuelScreen.DuelId,
                Time = Time,
                IsFounder = isFounder,
                QuestionInfoId = Question.Question.QuestionInfoId,
                CorrectAnswer = GetCorrectAnswer(),
                Stylish = answer,
                Id = Question.Id
            });

            IsBusy = false;
        }

        /// <summary>
        /// Oyunu başlatır
        /// </summary>
        private void StartGame(PopupPage popupPage)
        {
            Device.StartTimer(new TimeSpan(0, 0, 0, 3, 0), () =>
            {
                if (IsExit)
                    return false;

                RemovePopupPageAsync(popupPage);

                // Şıkları animasyonlu şekilde gösterir
                AnimateStylishCommand?.Execute(null);

                IsStylishClick = true;

                Device.StartTimer(new TimeSpan(0, 0, 0, 0, 2500), () => // Soru gözükünce hemen süre başlamasın
                {
                    if (!IsExit)
                    {
                        IsTimerEnable = true;
                        BotActive();
                    }

                    return false;
                });

                return false;
            });
        }

        /// <summary>
        /// Süreyi başlat
        /// </summary>
        private void StartTimer()
        {
            Device.StartTimer(new TimeSpan(0, 0, 0, 1, 0), () =>
            {
                if (Time > 10 || Time < 0)
                    Time = 10;

                if (IsTimerEnable)
                    Time -= 1;

                if (Time == 0 && IsStylishClick)
                    UnableToReply();

                return !IsExit;
            });
        }

        /// <summary>
        /// Süre doldu. Soru cevaplanmadı
        /// </summary>
        private void UnableToReply()
        {
            IsStylishClick = false;

            SaveAnswer(Stylish.UnableToReply, IsFounder).Wait();
        }

        #endregion Methods

        #region Commands

        public ICommand AnimateStylishCommand;

        private ICommand _answerCommand;

        public ICommand AnswerCommand => _answerCommand ?? (_answerCommand = new Command<AnswerModel>(async (answerModel) => await ExecuteAnswerCommandCommand(answerModel)));

        /// <summary>
        /// Soru ekranı kapatır
        /// </summary>
        public ICommand DuelCloseCommand => new Command(() => ExecuteDuelCloseCommand());

        #endregion Commands
    }
}