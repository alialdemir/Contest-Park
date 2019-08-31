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
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

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

        private DuelCreated _duelCreadted;

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
            get { return "#1BD5AC"; }
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
            get { return "#DD3448"; }
        }

        public DuelCreated DuelCreated
        {
            get => _duelCreadted;
            set
            {
                _duelCreadted = value;
                RaisePropertyChanged(() => DuelCreated);
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

        private Question _currentQuestion = new Question();

        public Question CurrentQuestion
        {
            get { return _currentQuestion; }
            set
            {
                _currentQuestion = value;

                RaisePropertyChanged(() => CurrentQuestion);
            }
        }

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

            SetCurrentQuestion();

            ResetImageBorderColor();

            IsInitialized = true;

            DisplayQuestionExpectedPopup();

            DuelSignalrListener();

            StartTimer();

            base.InitializeAsync();

            return Task.CompletedTask;
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

            ChangeStylishColor(questionModel.FounderStylish, questionModel.CorrectStylish);
            ChangeStylishColor(questionModel.OpponentStylish, questionModel.CorrectStylish);
            ChangeStylishColor(questionModel.CorrectStylish, questionModel.CorrectStylish);

            ShowStylishArrow(questionModel.FounderStylish, true, true);
            ShowStylishArrow(questionModel.OpponentStylish, false, true);

            PlaySound(questionModel.FounderStylish, questionModel.OpponentStylish, questionModel.CorrectStylish);

            ChangeImageBorderColor(questionModel.FounderStylish, questionModel.OpponentStylish, questionModel.CorrectStylish);

            if (questionModel.IsGameEnd || Round >= 7)
            {
                GameEnd();
                return;
            }

            Device.StartTimer(new TimeSpan(0, 0, 0, 4, 0), () => // Hemen sonraki soruya geçmemesi için biraz beklettim
            {
                if (IsExit)
                    return false;

                Round = questionModel.Round;

                DisplayQuestionExpectedPopup();

                ShowStylishArrow(questionModel.FounderStylish, true, false);
                ShowStylishArrow(questionModel.OpponentStylish, false, false);

                Device.StartTimer(new TimeSpan(0, 0, 0, 2, 0), () => // Bekleme ekranı çıkmadan soru ekranda gözükmesin
                {
                    if (IsExit)
                        return false;

                    SetCurrentQuestion();// sıradaki soru set edildi

                    return false;
                });

                return false;
            });
        }

        private void SetCurrentQuestion()
        {
            if (Round > DuelCreated.Questions.Count())
                return;

            var currentQuestion = DuelCreated.Questions.ToList()[Round - 1];
            Languages currentLanguage = _settingsService.CurrentUser.Language;

            CurrentQuestion = new Question
            {
                Answers = currentQuestion.Answers.Where(x => x.Language == currentLanguage).ToList(),
                AnswerType = currentQuestion.AnswerType,
                Link = currentQuestion.Link,
                NextQuestion = currentQuestion.Questions.FirstOrDefault(x => x.Language == currentLanguage).Question,
                QuestionId = currentQuestion.QuestionId,
                QuestionType = currentQuestion.QuestionType,
                Questions = currentQuestion.Questions.Where(x => x.Language == currentLanguage).ToList()
            };

            CreateAnswerPair();
        }

        /// <summary>
        /// Botu aktif eder
        /// </summary>
        private void BotActive()
        {
            // Eğer bot ile oynuyorsa oyuna bot eklendi

            if (DuelScreen.FounderUserId.Contains("-bot"))
            {
                _botService.Init(BotSaveAnswerCommand, DuelScreen.FounderUserId);
            }

            if (DuelScreen.OpponentUserId.Contains("-bot"))
            {
                _botService.Init(BotSaveAnswerCommand, DuelScreen.OpponentUserId);
            }
        }

        /// <summary>
        /// Verilen cevaplara göre resimlerin border color değiştirir
        /// </summary>
        private void ChangeImageBorderColor(Stylish founderStylish, Stylish opponentStylish, Stylish correctAnswer)
        {
            FounderImageBorderColor = correctAnswer == founderStylish ? GreenColor : RedColor;

            OpponentImageBorderColor = correctAnswer == opponentStylish ? GreenColor : RedColor;
        }

        /// <summary>
        /// Verilen cevaba göre şıkların renklerini değiştirir
        /// </summary>
        private void ChangeStylishColor(Stylish stylish, Stylish correctAnswer)
        {
            byte index = (byte)stylish;

            if (index < 0 || index > 4)
                return;

            bool isCorrect = correctAnswer == stylish;

            Color color = Color.FromHex(isCorrect ? GreenColor : RedColor);

            CurrentQuestion
                .Answers[index - 1]
                .Color = color;
        }

        /// <summary>
        /// Sağ ve soldaki şık oklarını gösterip gizler
        /// </summary>
        /// <param name="stylish">Hangi şıkkın ok yönü gösterilecek</param>
        /// <param name="isLeft">True ise sağ ok false ise sol oku gösterir/gizler</param>
        /// <param name="isVisible">True ise gözükür false ise gizler</param>
        private void ShowStylishArrow(Stylish stylish, bool isLeft, bool isVisible)
        {
            byte index = (byte)stylish;

            if (index < 0 || index > 4)
                return;

            if (isLeft)
            {
                CurrentQuestion
                    .Answers[index - 1]
                    .IsLeftArrowVisible = isVisible;
            }
            else
            {
                CurrentQuestion
                    .Answers[index - 1]
                    .IsRightArrowVisible = isVisible;
            }
        }

        /// <summary>
        /// Cevapları yan yana sıralamak için ayrı bir model oluşturduk
        /// </summary>
        private void CreateAnswerPair()
        {
            var answers = CurrentQuestion?.Answers;
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

            IsBusy = false;
        }

        /// <summary>
        /// Sorunun cevabını doğru bildimi kontrol eder
        /// </summary>
        /// <param name="answerModel">Şıkkın bilgisi</param>
        private void ExecuteAnswerCommandCommand(AnswerModel answerModel)
        {
            if (answerModel == null || !IsStylishClick)
                return;

            IsStylishClick = false;

            Stylish answerStylish = GetAnswerByUserAnswer(answerModel.Answers);

            SaveAnswerCommand.Execute(new SaveAnswerModel
            {
                Stylish = answerStylish,
                UserId = _settingsService.CurrentUser.UserId,
            });
        }

        /// <summary>
        /// Soru ekranını kapatır düel result ekranı açar
        /// </summary>
        private void ExecuteDuelCloseCommand()
        {
            IsExit = true;

            IsStylishClick = false;

            if (DuelScreen.DuelId > 0)
            {
                _duelSignalRService.LeaveGroup(DuelScreen.DuelId);
            }

            _duelSignalRService.NextQuestionEventHandler -= NextQuestion;
            _duelSignalRService.OffNextQuestion();

            Device.BeginInvokeOnMainThread(async () =>
            {
                await PushPopupPageAsync(new DuelResultPopupView()
                {
                    DuelId = DuelScreen.DuelId
                });

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
            return (Stylish)CurrentQuestion
                .Answers
                .FindIndex(x => x.Answers == answer) + 1;
        }

        /// <summary>
        /// Renk adına göre rengi döndürür
        /// </summary>
        private Color GetColor(string colorName)
        {
            return (Color)ContestParkApp.Current.Resources[colorName];
        }

        /// <summary>
        /// Verilen cevaplara göre sesleri çalar
        /// </summary>
        /// <param name="founderStylish">Kurucunun verdiği cevap</param>
        /// <param name="opponentStylish">Rakip oyuncunun verdiği cevap</param>
        private void PlaySound(Stylish founderStylish, Stylish opponentStylish, Stylish correctAnswer)
        {
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
            CurrentQuestion
                .Answers
                .ForEach((answer) => answer.Color = whiteColor);
        }

        /// <summary>
        /// Verilen cevabı sunucuya gönderir
        /// </summary>
        private async Task SaveAnswer(SaveAnswerModel saveAnswer)
        {
            Debug.WriteLine("if öncesi  " + saveAnswer.Stylish);
            if (IsBusy)
                return;

            IsBusy = true;

            Debug.WriteLine("if sonrası " + saveAnswer.Stylish);

            await _duelSignalRService.SaveAnswer(new UserAnswer
            {
                Time = Time,
                QuestionId = CurrentQuestion.QuestionId,
                DuelId = DuelScreen.DuelId,
                Stylish = saveAnswer.Stylish,
                UserId = saveAnswer.UserId,
            });

            IsBusy = false;
        }

        /// <summary>
        /// Botun verdiği cevapları sunucuya gönderir
        /// </summary>
        private async Task BotSaveAnswer(SaveAnswerModel saveAnswer)
        {
            Debug.WriteLine("bot cevap" + saveAnswer.Stylish);

            await _duelSignalRService.SaveAnswer(new UserAnswer
            {
                Time = Time,
                QuestionId = CurrentQuestion.QuestionId,
                DuelId = DuelScreen.DuelId,
                Stylish = saveAnswer.Stylish,
                UserId = saveAnswer.UserId,
            });
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
                if (Time > 10 || Time <= 0)
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

            SaveAnswerCommand.Execute(new SaveAnswerModel
            {
                Stylish = Stylish.UnableToReply,
                UserId = _settingsService.CurrentUser.UserId
            });
        }

        #endregion Methods

        #region Commands

        public ICommand AnimateStylishCommand;

        private ICommand _answerCommand;
        private ICommand _botAnswerCommand;
        private ICommand _saveAnswerCommand;

        public ICommand AnswerCommand => _answerCommand ?? (_answerCommand = new Command<AnswerModel>((answerModel) => ExecuteAnswerCommandCommand(answerModel)));
        private ICommand SaveAnswerCommand => _saveAnswerCommand ?? (_saveAnswerCommand = new Command<SaveAnswerModel>(async (answerModel) => await SaveAnswer(answerModel)));
        private ICommand BotSaveAnswerCommand => _botAnswerCommand ?? (_botAnswerCommand = new Command<SaveAnswerModel>(async (answerModel) => await BotSaveAnswer(answerModel)));

        /// <summary>
        /// Soru ekranı kapatır
        /// </summary>
        public ICommand DuelCloseCommand => new Command(() => ExecuteDuelCloseCommand());

        #endregion Commands
    }
}
