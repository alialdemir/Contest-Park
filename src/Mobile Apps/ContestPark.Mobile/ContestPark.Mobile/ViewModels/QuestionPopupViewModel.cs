using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Models.Duel;
using ContestPark.Mobile.Models.Duel.Quiz;
using ContestPark.Mobile.Models.Error;
using ContestPark.Mobile.Services.AdMob;
using ContestPark.Mobile.Services.Analytics;
using ContestPark.Mobile.Services.Audio;
using ContestPark.Mobile.Services.Duel;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.Services.Signalr.Duel;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Prism.Events;
using Prism.Navigation;
using Prism.Services;
using Rg.Plugins.Popup.Contracts;
using System;
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
        private readonly IDuelService _duelService;
        private readonly IAdMobService _adMobService;
        private readonly IDuelSignalRService _duelSignalRService;
        private readonly IAnalyticsService _analyticsService;
        private readonly OnSleepEvent _onSleepEvent;
        private readonly ISettingsService _settingsService;
        private SubscriptionToken _subscriptionToken;

        #endregion Private variables

        #region Constructor

        public QuestionPopupViewModel(IPopupNavigation popupNavigation,
                                      INavigationService navigationService,
                                      IDuelSignalRService duelSignalRService,
                                      IPageDialogService pageDialogService,
                                      IEventAggregator eventAggregator,
                                      IAnalyticsService analyticsService,
                                      IDuelService duelService,
                                      IAdMobService adMobService,
                                      ISettingsService settingsService,
                                      IAudioService audioService) : base(navigationService: navigationService, dialogService: pageDialogService, popupNavigation: popupNavigation)
        {
            _duelSignalRService = duelSignalRService;
            _analyticsService = analyticsService;
            _onSleepEvent = eventAggregator.GetEvent<OnSleepEvent>();
            _duelService = duelService;
            _adMobService = adMobService;
            _settingsService = settingsService;
            _audioService = audioService;

            FounderImageBorderColor = PrimaryColor;
            OpponentImageBorderColor = PrimaryColor;
        }

        #endregion Constructor

        #region Properties

        private AnswerPair _answers;

        private string _founderImageBorderColor;

        private byte _founderScore;

        private string _opponentImageBorderColor;

        private byte _opponentScore;

        private byte _time = 10;

        private Models.Duel.QuestionModel _question;

        public Models.Duel.QuestionModel Question
        {
            get { return _question; }
            set
            {
                _question = value;
                RaisePropertyChanged(() => Question);
            }
        }

        public AnswerPair Answers
        {
            get => _answers;
            set
            {
                _answers = value;
                RaisePropertyChanged(() => Answers);
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

        public string RedColor
        {
            get { return "#993232"; }
        }

        public byte Round { get; set; } = 1;

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
        private bool IsFounder => _settingsService.CurrentUser.UserId == Question.DuelStarting.FounderUserId;

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

        public bool IsGameEnd { get; set; }

        #endregion Properties

        #region SignalR

        private void DuelSignalrListener()
        {
            _duelSignalRService.NextQuestionEventHandler += NextQuestion;

            _duelSignalRService.NextQuestion();

            _duelSignalRService.SendErrorMessagetHandler += OnSendErrorMessage;
            _duelSignalRService.SendErrorMessage();
        }

        /// <summary>
        /// Eventleri dinlemeyi bırakır
        /// </summary>
        private void OffSignalr()
        {
            _duelSignalRService.NextQuestionEventHandler -= NextQuestion;
            _duelSignalRService.OffNextQuestion();

            _duelSignalRService.SendErrorMessagetHandler -= OnSendErrorMessage;
            _duelSignalRService.OffSendErrorMessage();
        }

        #endregion SignalR

        #region Methods

        public override Task InitializeAsync(INavigationParameters parameters = null)
        {
            if (parameters.ContainsKey("Question"))
                Question = parameters.GetValue<Models.Duel.QuestionModel>("Question");

            SetCurrentQuestion();

            ResetImageBorderColor();

            DisplayQuestionExpectedPopup();

            DuelSignalrListener();

            StartTimer();

            base.InitializeAsync(parameters);

            OnSleepEventListener();

            LoadInterstitialVideoCommand.Execute(null);

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
                _duelService.DuelCancel();

                IsExit = true;
                GotoBackCommand.Execute(false);

                return;
            }

            IsTimerEnable = false;

            FounderScore += questionModel.FounderScore;
            OpponentScore += questionModel.OpponentScore;

            _audioService.Stop();

            ChangeStylishColor(questionModel.FounderStylish, questionModel.CorrectStylish);
            ChangeStylishColor(questionModel.OpponentStylish, questionModel.CorrectStylish);
            ChangeStylishColor(questionModel.CorrectStylish, questionModel.CorrectStylish);

            ShowStylishArrow(questionModel.FounderStylish, true, true);
            ShowStylishArrow(questionModel.OpponentStylish, false, true);

            PlaySound(questionModel.FounderStylish, questionModel.OpponentStylish, questionModel.CorrectStylish);

            ChangeImageBorderColor(questionModel.FounderStylish, questionModel.OpponentStylish, questionModel.CorrectStylish);

            _analyticsService.SendEvent("Düello", $"{questionModel.Round}. Raund Oynandı", Question.SubCategoryName);

            if (questionModel.IsGameEnd || Round >= 7)// Düello sona erdi
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
            if (Question == null || Question.DuelCreated == null || Question.DuelCreated.Questions == null || Round > Question.DuelCreated.Questions.Count())
            {
                IsExit = true;

                GotoBackCommand.Execute(false);
            }

            Languages currentLanguage = _settingsService.CurrentUser.Language;

            var currentQuestion = Question.DuelCreated.Questions.ToList()[Round - 1];
            if (currentQuestion == null
                || currentQuestion.Answers == null
                || !currentQuestion.Answers.Any(x => x.Language == currentLanguage)
                || !currentQuestion.Questions.Any(x => x.Language == currentLanguage))
            {
                _duelService.DuelCancel();

                IsExit = true;
                GotoBackCommand.Execute(false);

                return;
            }

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

            if (CurrentQuestion.QuestionType == QuestionTypes.Music)// Eğer soru müzikli ise play edildi
            {
                _audioService.ToggleAudio(CurrentQuestion.Link);
            }

            CreateAnswerPair();
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

            QuestionExpectedPopup();

            StartGame();

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
        public override async Task GoBackAsync(INavigationParameters parameters = null, bool? isShowAlert = false)
        {
            if (isShowAlert.HasValue && isShowAlert.Value)// useModalNavigation parametresini alert gösterilsinmi gösterilmesin mi diye kullandım
            {
                bool isOkay = await DisplayAlertAsync(ContestParkResources.Exit,
                                                  ContestParkResources.AreYouSureYouWantToLeave,
                                                  ContestParkResources.Okay,
                                                  ContestParkResources.Cancel);

                if (!isOkay)
                    return;
            }

            IsExit = true;

            IsStylishClick = false;

            if (Question.DuelCreated.DuelId > 0)
            {
                await _duelSignalRService.LeaveGroup(Question.DuelCreated.DuelId);

                if (!IsGameEnd)// Oyun sonlanmadan çıkmış ise düellodan kaçtı olarak bildirdik
                {
                    await _duelService.DuelEscape(Question.DuelCreated.DuelId);
                }
            }

            OffSignalr();

            _audioService.Stop();

            _onSleepEvent.Unsubscribe(_subscriptionToken);

            await NavigateToPopupAsync<DuelResultPopupView>(new NavigationParameters
            {
                { "DuelId", Question.DuelCreated.DuelId }
            });

            await RemoveFirstPopupAsync<QuestionExpectedPopupView>();

            await RemoveFirstPopupAsync<QuestionPopupView>();
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

                IsGameEnd = true;

                GotoBackCommand.Execute(false);

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
            if (!_settingsService.IsSoundEffectActive)
                return;

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
        private void QuestionExpectedPopup()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                NavigateToPopupAsync<QuestionExpectedPopupView>(new NavigationParameters
                    {
                        { "SubcategoryName", Question.SubCategoryName },
                        { "SubCategoryPicturePath", Question.SubCategoryPicturePath },
                        { "RoundCount",  Round },
                    });
            });
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
        private void SaveAnswer(SaveAnswerModel saveAnswer)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            ShowStylishArrow(saveAnswer.Stylish, IsFounder, true);

            _duelSignalRService.SaveAnswer(new UserAnswer
            {
                Time = Time,
                QuestionId = CurrentQuestion.QuestionId,
                DuelId = Question.DuelCreated.DuelId,
                Stylish = saveAnswer.Stylish,
                UserId = saveAnswer.UserId,
                BalanceType = Question.BalanceType,
            });

            _analyticsService.SendEvent("Düello", saveAnswer.Stylish.ToString(), Question.SubCategoryName);

            IsBusy = false;
        }

        /// <summary>
        /// Oyunu başlatır
        /// </summary>
        private void StartGame()
        {
            Device.StartTimer(new TimeSpan(0, 0, 0, 3, 0), () =>
            {
                if (IsExit)
                    return false;

                RemoveFirstPopupAsync<QuestionExpectedPopupView>();

                // Şıkları animasyonlu şekilde gösterir
                AnimateStylishCommand?.Execute(null);

                IsStylishClick = true;

                Device.StartTimer(new TimeSpan(0, 0, 0, 0, 500), () => // Soru gözükünce hemen süre başlamasın
                {
                    if (!IsExit)
                    {
                        IsTimerEnable = true;
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

        /// <summary>
        /// Eğer soru ekranındayken oyundan çıkarsa yenilmiş sayılsın
        /// </summary>
        private void OnSleepEventListener()
        {
            _subscriptionToken = _onSleepEvent.Subscribe(() =>
            {
                GotoBackCommand.Execute(false);

                DisplayAlertAsync("",
                                  ContestParkResources.YouLeftTheGameDuringTheDuelYouAreDefeated,
                                  ContestParkResources.Okay);
            });
        }

        /// <summary>
        /// Düello sırasında hata oluşursa mesaj göstersin ve bekleme modundan çıksın
        /// </summary>
        /// <param name="sender">Hata mesajı</param>
        private void OnSendErrorMessage(object sender, ErrorMessageModel e)
        {
            ErrorMessageModel error = (ErrorMessageModel)sender;
            if (error == null || string.IsNullOrEmpty(error.Message))
                return;

            Device.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlertAsync(string.Empty,
                                        error.Message,
                                        ContestParkResources.Okay);

                await _duelService.DuelCancel();

                IsGameEnd = true;

                GotoBackCommand.Execute(false);
            });
        }

        #endregion Methods

        #region Commands

        public ICommand AnimateStylishCommand;

        private ICommand _answerCommand;
        private ICommand _saveAnswerCommand;

        /// <summary>
        /// Tam ekran reklma yükle
        /// </summary>
        private ICommand LoadInterstitialVideoCommand
        {
            get
            {
                return new Command(() => _adMobService.LoadInterstitialVideo());
            }
        }

        public ICommand AnswerCommand => _answerCommand ?? (_answerCommand = new Command<AnswerModel>(ExecuteAnswerCommandCommand));
        private ICommand SaveAnswerCommand => _saveAnswerCommand ?? (_saveAnswerCommand = new Command<SaveAnswerModel>(SaveAnswer));

        #endregion Commands
    }
}
