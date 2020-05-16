using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Duel;
using ContestPark.Mobile.Models.Duel.InviteDuel;
using ContestPark.Mobile.Models.Duel.Quiz;
using ContestPark.Mobile.Models.Error;
using ContestPark.Mobile.Services.Audio;
using ContestPark.Mobile.Services.Duel;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.Services.Signalr.Duel;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Microsoft.AppCenter.Analytics;
using Prism.Events;
using Prism.Navigation;
using Prism.Services;
using Rg.Plugins.Popup.Contracts;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class DuelStartingPopupViewModel : ViewModelBase
    {
        #region Enums

        public enum StandbyModes
        {
            On,
            Off,
            Invited
        }

        #endregion Enums

        #region Private Variables

        private readonly OnSleepEvent _onSleepEvent;
        private SubscriptionToken _subscriptionToken;
        private readonly IAudioService _audioService;

        private readonly IDuelService _duelService;

        private readonly IDuelSignalRService _duelSignalRService;

        private readonly ISettingsService _settingsService;

        #endregion Private Variables

        #region Constructor

        public DuelStartingPopupViewModel(IAudioService audioService,
                                          IEventAggregator eventAggregator,
                                          IDuelService duelService,
                                          IDuelSignalRService duelSignalRService,
                                          IPopupNavigation popupNavigation,
                                          IPageDialogService dialogService,
                                          INavigationService navigationService,
                                          ISettingsService settingsService) : base(navigationService, dialogService, popupNavigation)
        {
            _audioService = audioService ?? throw new ArgumentNullException(nameof(audioService));

            _duelService = duelService ?? throw new ArgumentNullException(nameof(duelService));

            _duelSignalRService = duelSignalRService ?? throw new ArgumentNullException(nameof(duelSignalRService));

            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));

            _onSleepEvent = eventAggregator.GetEvent<OnSleepEvent>();
        }

        #endregion Constructor

        #region Properties

        private DuelStartingModel _duelStarting = new DuelStartingModel();

        public DuelStartingModel DuelStarting
        {
            get
            {
                return _duelStarting;
            }
            set
            {
                _duelStarting = value;

                RaisePropertyChanged(() => DuelStarting);
            }
        }

        public SelectedBetModel SelectedBet { get; set; }

        /// <summary>
        /// Eğer sorular gelmişse quiz view geçerken true ise alert çıkmasın
        /// </summary>
        private bool IsNextQuestionExit { get; set; } = false;

        private bool RandomPicturStatus { get; set; } = true;

        private bool IsExit { get; set; }

        #endregion Properties

        #region SignalR

        /// <summary>
        /// Eventleri dinler
        /// </summary>
        private void DuelSignalrListener()
        {
            _duelSignalRService.DuelCreatedEventHandler += OnDuelCreated;
            _duelSignalRService.DuelCreated();

            _duelSignalRService.SendErrorMessagetHandler += OnSendErrorMessage;
            _duelSignalRService.SendErrorMessage();
        }

        /// <summary>
        /// Eventleri dinlemeyi bırakır
        /// </summary>
        private void OffSignalr()
        {
            _duelSignalRService.DuelCreatedEventHandler -= OnDuelCreated;
            _duelSignalRService.OffDuelCreated();

            _duelSignalRService.SendErrorMessagetHandler -= OnSendErrorMessage;
            _duelSignalRService.OffSendErrorMessage();
        }

        #endregion SignalR

        #region Methods

        public override void Initialize(INavigationParameters parameters = null)
        {
            if (parameters.ContainsKey("SelectedDuelInfo"))
                SelectedBet = parameters.GetValue<SelectedBetModel>("SelectedDuelInfo");

            DuelStartingCommand.Execute(null);

            base.Initialize(parameters);
        }

        /// <summary>
        /// Düello başlatma işlemleri
        /// </summary>
        private void ExecuteDuelStartingCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            DuelStarting = new DuelStartingModel()
            {
                FounderFullName = _settingsService.CurrentUser.FullName,
                FounderCoverPicturePath = _settingsService.CurrentUser.CoverPicturePath,
                FounderProfilePicturePath = _settingsService.CurrentUser.ProfilePicturePath
            };

            if (_settingsService.IsSoundEffectActive)
                _audioService.Play(AudioTypes.AwaitingOpponent, true);

            if (string.IsNullOrEmpty(_settingsService.SignalRConnectionId))
            {
                NoConnection();
            }
            else if (StandbyModes.Invited == SelectedBet.StandbyMode)
            {
                DuelSignalrListener();

                OnSleepEventListener();
            }
            else if (StandbyModes.Off == SelectedBet.StandbyMode)
            {
                if (!string.IsNullOrEmpty(SelectedBet.OpponentUserId))
                {
                    DuelSignalrListener();

                    OnSleepEventListener();

                    DuelStartWithInvite();
                }
            }
            else
            {
                StandbyModeOff();
            }

            IsBusy = false;
        }

        private void NoConnection()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                AudioStop();

                DisplayAlertAsync(
                 ContestParkResources.Error,
                 ContestParkResources.ConnectionToTheServerForTheDuelWasNotEstablished,
                 ContestParkResources.Okay);

                // Burada daha signlar falan bağlanmamış oluyor o yüzden direk base sınıfdaki goback çağrıldı
                base.GoBackAsync(useModalNavigation: true);
            });
        }

        /// <summary>
        /// Düello daveti ile düello başlama adımlarını gerçekleştirir
        /// </summary>
        private async void DuelStartWithInvite()
        {
            var opponentUserInfo = await _duelService.InviteDuel(new InviteDuelModel
            {
                Bet = SelectedBet.Bet,
                SubCategoryId = SelectedBet.SubcategoryId,
                BalanceType = SelectedBet.BalanceType,
                OpponentUserId = SelectedBet.OpponentUserId,
                FounderConnectionId = _settingsService.SignalRConnectionId
            });
            if (opponentUserInfo == null)
            {
                await DisplayAlertAsync(ContestParkResources.Error,
                                        ContestParkResources.TheOpponentDidNotAcceptTheDuel,
                                        ContestParkResources.Okay);

                IsNextQuestionExit = true;

                await GoBackAsync(useModalNavigation: true);

                return;
            }

            DuelStarting.OpponentUserId = opponentUserInfo.UserId;
            DuelStarting.OpponentProfilePicturePath = opponentUserInfo.ProfilePicturePath;
            DuelStarting.OpponentCoverPicturePath = opponentUserInfo.CoverPicturePath;
            DuelStarting.OpponentFullName = opponentUserInfo.FullName;
            DuelStarting.OpponentCountry = ContestParkResources.AwaitingOpponent;
            //DuelStarting.OpponentLevel = opponentUserInfo.Level

            Device.StartTimer(new TimeSpan(0, 0, 15), () =>
            {
                // 15 Saniye içinde rakip düelloyu kabul etmezse oyuncuya rakibiniz düello davetinizi kabul etmedi mesajı veriyoruz
                if (DuelStarting.DuelId <= 0 && !IsNextQuestionExit)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                   {
                       await DisplayAlertAsync(string.Empty,
                                               ContestParkResources.YourOpponentDidNotAcceptYourDuelInvitation,
                                               ContestParkResources.Okay);

                       IsNextQuestionExit = true;

                       await GoBackAsync(useModalNavigation: true);
                   });
                }

                return false;
            });
        }

        /// <summary>
        /// Kullanıcının beklediği düelloya rastgele bot ekler
        /// </summary>
        private void AddToBot()
        {
            int randomSecond = new Random().Next(3, 7);

            Device.StartTimer(new TimeSpan(0, 0, 0, randomSecond, 0), () =>
            {
                if (IsNextQuestionExit || DuelStarting.DuelId > 0)
                    return false;

                _duelService.AddOpponent(new StandbyModeModel
                {
                    Bet = SelectedBet.Bet,
                    SubCategoryId = SelectedBet.SubcategoryId,
                    BalanceType = SelectedBet.BalanceType,
                });

                return false;
            });
        }

        /// <summary>
        /// Sesi kapatır
        /// </summary>
        private void AudioStop()
        {
            if (_settingsService.IsSoundEffectActive)
                _audioService?.Stop();
        }

        /// <summary>
        /// Düello başlıyor ekranını kapatır
        /// </summary>
        public override async Task GoBackAsync(INavigationParameters parameters = null, bool? useModalNavigation = false)
        {
            if (DuelStarting.DuelId != 0 && !IsNextQuestionExit)// Duel id 0 değilse düello başlamıştır
            {
                bool isOkay = await DisplayAlertAsync(ContestParkResources.Exit,
                                                      ContestParkResources.AreYouSureYouWantToLeave,
                                                      ContestParkResources.Okay,
                                                      ContestParkResources.Cancel);
                if (!isOkay)
                {
                    return;
                }
            }

            AudioStop();

            IsNextQuestionExit = true;

            RandomPicturStatus = false;

            OffSignalr();

            _onSleepEvent.Unsubscribe(_subscriptionToken);

            await RemoveFirstPopupAsync<DuelStartingPopupView>();

            if (DuelStarting.DuelId == 0 && !string.IsNullOrEmpty(_settingsService.SignalRConnectionId) && SelectedBet.StandbyMode == StandbyModes.On)
            {
                await _duelService.ExitStandMode(new StandbyModeModel
                {
                    BalanceType = SelectedBet.BalanceType,
                    Bet = SelectedBet.Bet,
                    ConnectionId = _settingsService.SignalRConnectionId,
                    SubCategoryId = SelectedBet.SubcategoryId,
                });
            }
        }

        /// <summary>
        /// Rakip bekliyor moduna aldık
        /// </summary>
        private async Task ExecuteDuelOpenRandomAsync()
        {
            bool isSuccess = await _duelService.StandbyMode(new StandbyModeModel
            {
                BalanceType = SelectedBet.BalanceType,
                Bet = SelectedBet.Bet,
                ConnectionId = _settingsService.SignalRConnectionId,
                SubCategoryId = SelectedBet.SubcategoryId,
            });// TODO: success kontrol et hata oluşursa mesaj çıksın
            if (!isSuccess)
                NotStartingDuel();

            AddToBot();
        }

        /// <summary>
        /// Düellodaki soruları alır
        /// </summary>
        private void OnDuelCreated(object sender, DuelCreated e)
        {
            DuelCreated duelCreated = (DuelCreated)sender;
            if (duelCreated == null)
            {
                Analytics.TrackEvent($"Duello başlama ekranında bilgiler boş geldi userId {_settingsService.CurrentUser.UserId}");

                NotStartingDuel();

                return;
            }

            Analytics.TrackEvent($"{duelCreated.DuelId} düello founder id {duelCreated.FounderUserId} opponent id {duelCreated.OpponentUserId} arasında başlıyor");

            _settingsService.AddPendingDuelId(duelCreated.DuelId);

            RandomPicturStatus = false;

            RandomPicturStatus = false;

            AnimationCommand?.Execute(null);

            DuelStarting = duelCreated;

            var question = new Models.Duel.QuestionModel
            {
                DuelStarting = DuelStarting,
                DuelCreated = duelCreated,
                BalanceType = SelectedBet.BalanceType,
                SubCategoryName = SelectedBet.SubCategoryName,
                SubCategoryPicturePath = SelectedBet.SubCategoryPicturePath
            };

            AudioStop();

            Device.StartTimer(new TimeSpan(0, 0, 0, 3, 0), () =>
            {
                IsNextQuestionExit = true;

                if (IsExit)
                    return false;

                NavigateToPopupAsync<QuestionPopupView>(new NavigationParameters
                                                        {
                                                                { "Question", question }
                                                        });

                RemoveFirstPopupAsync<DuelStartingPopupView>();

                OffSignalr();

                return false;
            });
        }

        /// <summary>
        /// Düello başlatılırken hata oluşurse mesaj gösterip duello başlama ekranını kapatır
        /// </summary>
        /// <returns></returns>
        private async void NotStartingDuel()
        {
            await DisplayAlertAsync(ContestParkResources.Error,
                                 ContestParkResources.ErrorStartingDuelPleaseTryAgain,
                                 ContestParkResources.Okay);

            await _duelService.DuelCancel();

            IsNextQuestionExit = true;

            await GoBackAsync(useModalNavigation: true);
        }

        /// <summary>
        /// Bekleme modundayken rakip profil resmini değiştir
        /// </summary>
        private async void RandomPicturesAsync()
        {
            string[] pictures = await _duelService.RandomUserProfilePictures();

            if (pictures == null || pictures.Length == 0)
                return;

            int pictureIndex = pictures.Length;

            Device.StartTimer(new TimeSpan(0, 0, 0, 0, 700), () =>
            {
                if (!RandomPicturStatus)
                    return RandomPicturStatus;

                if (pictureIndex >= 0)
                {
                    pictureIndex--;

                    if (pictureIndex < 0)
                        pictureIndex = pictures.Length - 1;

                    if (DuelStarting.DuelId == 0)
                        DuelStarting.OpponentProfilePicturePath = pictures[pictureIndex];
                }

                return RandomPicturStatus;
            });
        }

        /// <summary>
        /// Rakip araken bekleme ekranı için gerekli işlemler
        /// </summary>
        private void RandomUserProfilePicturesAsync()
        {
            DuelStarting.OpponentFullName = ContestParkResources.AwaitingOpponent;

            RandomPicturesAsync();
        }

        /// <summary>
        /// Bekleme modundayken yapılan işlemler
        /// </summary>
        private void StandbyModeOff()
        {
            DuelSignalrListener();// SignalR listener load

            OnSleepEventListener();

            RandomUserProfilePicturesAsync();

            DuelOpenCommand.Execute(null);
        }

        /// <summary>
        /// Eğer düello başlamışsa ve oyundan çıkarsa yenilmiş sayılsın
        /// </summary>
        private void OnSleepEventListener()
        {
            _subscriptionToken = _onSleepEvent.Subscribe(async () =>
            {
                IsExit = true;

                IsNextQuestionExit = true;

                GotoBackCommand.Execute(true);

                if (DuelStarting.DuelId > 0)
                {
                    await _duelService.DuelEscape(DuelStarting.DuelId);

                    await DisplayAlertAsync("",
                                        ContestParkResources.YouLeftTheGameDuringTheDuelYouAreDefeated,
                                        ContestParkResources.Okay);
                }
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

            Device.BeginInvokeOnMainThread(() =>
           {
               DisplayAlertAsync(string.Empty,
                                 error.Message,
                                 ContestParkResources.Okay);

               _duelService.DuelCancel();

               GoBackAsync(useModalNavigation: true);
           });
        }

        #endregion Methods

        #region Commands

        private ICommand DuelStartingCommand => new Command(ExecuteDuelStartingCommand);

        public ICommand AnimationCommand;

        private ICommand DuelOpenCommand => new CommandAsync(ExecuteDuelOpenRandomAsync);

        #endregion Commands
    }
}
