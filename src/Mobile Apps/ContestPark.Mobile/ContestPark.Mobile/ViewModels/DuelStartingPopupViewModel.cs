using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Models.Duel;
using ContestPark.Mobile.Models.Duel.Quiz;
using ContestPark.Mobile.Services.Audio;
using ContestPark.Mobile.Services.Duel;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.Services.Signalr.Duel;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
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
            Off
        }

        #endregion Enums

        #region Private Variables

        private readonly IAudioService _audioService;

        private readonly IDuelService _duelService;

        private readonly IDuelSignalRService _duelSignalRService;

        private readonly ISettingsService _settingsService;

        #endregion Private Variables

        #region Constructor

        public DuelStartingPopupViewModel(IAudioService audioService,
                                          IDuelService duelService,
                                          IDuelSignalRService duelSignalRService,
                                          INavigationService navigationService,
                                          IPageDialogService pageDialogService,
                                          ISettingsService settingsService,
                                          IPopupNavigation popupNavigation) : base(navigationService, pageDialogService, popupNavigation)
        {
            _audioService = audioService ?? throw new ArgumentNullException(nameof(audioService));

            _duelService = duelService ?? throw new ArgumentNullException(nameof(duelService));

            _duelSignalRService = duelSignalRService ?? throw new ArgumentNullException(nameof(duelSignalRService));

            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));

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

        /// <summary>
        /// Eğer sorular gelmişse quiz view geçerken true ise alert çıkmasın
        /// </summary>
        public bool IsNextQuestionExit { get; set; } = false;

        public string OpponentUserId { get; set; }
        public SelectedSubCategoryModel SelectedSubCategory { get; set; } = new SelectedSubCategoryModel();
        public StandbyModes StandbyMode { get; set; }

        public StandbyModeModel StandbyModeModel { get; private set; } = new StandbyModeModel();
        private bool RandomPicturStatus { get; set; } = true;

        #endregion Properties

        #region SignalR

        /// <summary>
        /// Eventleri dinler
        /// </summary>
        private void DuelSignalrListener()
        {
            _duelSignalRService.DuelStartingEventHandler += OnDuelStarting;
            _duelSignalRService.DuelStarting();

            _duelSignalRService.DuelCreatedEventHandler += OnDuelCreated;
            _duelSignalRService.DuelCreated();
        }

        /// <summary>
        /// Eventleri dinlemeyi bırakır
        /// </summary>
        private void OffSignalr()
        {
            _duelSignalRService.DuelStartingEventHandler -= OnDuelStarting;
            _duelSignalRService.OffDuelStarting();

            _duelSignalRService.DuelCreatedEventHandler -= OnDuelCreated;
            _duelSignalRService.OffDuelCreated();
        }

        #endregion SignalR

        #region Methods

        protected override async Task InitializeAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            if (string.IsNullOrEmpty(_settingsService.SignalRConnectionId))
            {
                await DisplayAlertAsync(
                    ContestParkResources.Error,
                    ContestParkResources.ConnectionToTheServerForTheDuelWasNotEstablished,
                    ContestParkResources.Okay);

                DuelCloseCommand.Execute(null);
            }
            else if (StandbyModes.Off == StandbyMode)
            {
                //////if (!string.IsNullOrEmpty(OpponentUserId))
                //////{
                //////    bool isSuccess = await _duelService.DuelStartWithUserId(OpponentUserId);
                //////    if (!isSuccess)
                //////        await NotStartingDuel();
                //////}
                //else if (!string.IsNullOrEmpty(DuelId))
                //{
                //    bool isSuccess = await _duelService.DuelStartWithDuelId(DuelId);
                //    // TODO: direk Duel id(notification) ile gelirse gibi durumlar..
                //}
            }
            else
            {
                await StandbyModeOff();
            }

            await base.InitializeAsync();

            IsBusy = false;
        }

        /// <summary>
        /// Kullanıcının beklediği düelloya rastgele bot ekler
        /// </summary>
        private void AddToBot()
        {
            int randomSecond = new Random().Next(5, 10);

            Device.StartTimer(new TimeSpan(0, 0, 0, randomSecond, 0), () =>
            {
                if (IsNextQuestionExit || DuelStarting.DuelId > 0)
                    return false;

                _duelService.AddOpponent(new StandbyModeModel
                {
                    Bet = StandbyModeModel.Bet,
                    SubCategoryId = StandbyModeModel.SubCategoryId,
                    BalanceType = StandbyModeModel.BalanceType,
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
        /// Bekleme ekranı kapatır
        /// </summary>
        private async Task ExecuteDuelCloseCommandAsync()
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

            RandomPicturStatus = false;

            // Signalr events remove
            if (DuelStarting.DuelId == 0)
                OffSignalr();

            _duelSignalRService.DuelStartingEventHandler -= OnDuelStarting;
            _duelSignalRService.OffDuelStarting();

            await RemoveFirstPopupAsync();

            if (DuelStarting.DuelId == 0 && !string.IsNullOrEmpty(_settingsService.SignalRConnectionId))
            {
                await _duelService.ExitStandMode(StandbyModeModel);
            }
        }

        /// <summary>
        /// Rakip bekliyor moduna aldık
        /// </summary>
        private async Task ExecuteDuelOpenRandomAsync()
        {
            StandbyModeModel.ConnectionId = _settingsService.SignalRConnectionId;

            bool isSuccess = await _duelService.StandbyMode(StandbyModeModel);// TODO: success kontrol et hata oluşursa mesaj çıksın
            if (!isSuccess)
                await NotStartingDuel();

            AddToBot();
        }

        /// <summary>
        /// Ekrandaki bilgileri yeniler
        /// </summary>
        private void OnDuelStarting(object sender, DuelStartingModel e)
        {
            DuelStartingModel duelEnterScreenModel = (DuelStartingModel)sender;
            if (duelEnterScreenModel != null)
            {
                RandomPicturStatus = false;

                AnimationCommand?.Execute(null);

                DuelStarting = duelEnterScreenModel;
            }
            else
            {
                NotStartingDuel().Wait();
            }
        }

        /// <summary>
        /// Düellodaki soruları alır
        /// </summary>
        private async void OnDuelCreated(object sender, DuelCreated e)
        {
            DuelCreated duelCreated = (DuelCreated)sender;
            if (duelCreated == null)
            {
                // TODO: Server tarafına düello iptali için istek gönder bahis miktarı geri kullanıcıya eklensin.
                return;
            }

            if (DuelStarting.OpponentFullName != ContestParkResources.AwaitingOpponent && !IsNextQuestionExit)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    AudioStop();

                    await Task.Delay(3000); // Rakibi görebilmesi için 3sn beklettim

                    IsNextQuestionExit = true;

                    await PushPopupPageAsync(new QuestionPopupView
                    {
                        DuelCreated = duelCreated,
                        DuelStarting = new DuelStartingModel
                        {
                            FounderProfilePicturePath = DuelStarting.FounderProfilePicturePath,
                            OpponentProfilePicturePath = DuelStarting.OpponentProfilePicturePath,
                            DuelId = DuelStarting.DuelId,
                            FounderCoverPicturePath = DuelStarting.FounderCoverPicturePath,
                            FounderFullName = DuelStarting.FounderFullName,
                            FounderUserId = DuelStarting.FounderUserId,
                            OpponentCoverPicturePath = DuelStarting.OpponentCoverPicturePath,
                            OpponentFullName = DuelStarting.OpponentFullName,
                            OpponentUserId = DuelStarting.OpponentUserId
                        },
                        SubcategoryName = SelectedSubCategory.SubcategoryName,
                        SubCategoryPicturePath = SelectedSubCategory.SubCategoryPicturePath
                    });

                    DuelCloseCommand.Execute(null);
                });

                OffSignalr();
            }
            else
            {
                // buraya gelmiş ise rakip bilgileriden önce sorular gelmiştir.... o zamaan rakip bilgileri gelince burayı tekrar çağırmalı
                // TODO: rakip fotoğrafınn gelmesini beklet
            }
        }

        /// <summary>
        /// Düello başlatılırken hata oluşurse mesaj gösterip duello başlama ekranını kapatır
        /// </summary>
        /// <returns></returns>
        private async Task NotStartingDuel()
        {
            await DisplayAlertAsync(
          ContestParkResources.Error,
          ContestParkResources.ErrorStartingDuelPleaseTryAgain,
          ContestParkResources.Okay);

            DuelCloseCommand.Execute(null);
        }

        /// <summary>
        /// Bekleme modundayken rakip profil resmini değiştir
        /// </summary>
        private async Task RandomPicturesAsync()
        {
            string[] pictures = await _duelService.RandomUserProfilePictures();

            if (pictures == null || pictures.Length == 0)
                return;

            int pictureIndex = pictures.Length;

            Device.StartTimer(new TimeSpan(0, 0, 0, 0, 700), () =>
            {
                if (pictureIndex >= 0)
                {
                    pictureIndex--;

                    if (pictureIndex < 0)
                        pictureIndex = pictures.Length - 1;

                    DuelStarting.OpponentProfilePicturePath = pictures[pictureIndex];
                }

                return RandomPicturStatus;
            });
        }

        /// <summary>
        /// Rakip araken bekleme ekranı için gerekli işlemler
        /// </summary>
        private async Task RandomUserProfilePicturesAsync()
        {
            DuelStarting.OpponentFullName = ContestParkResources.AwaitingOpponent;

            await RandomPicturesAsync();
        }

        /// <summary>
        /// Bekleme modundayken yapılan işlemler
        /// </summary>
        private async Task StandbyModeOff()
        {
            DuelStarting = new DuelStartingModel()
            {
                FounderFullName = _settingsService.CurrentUser.FullName,
                FounderCoverPicturePath = _settingsService.CurrentUser.CoverPicturePath,
                FounderProfilePicturePath = _settingsService.CurrentUser.ProfilePicturePath
            };

            if (_settingsService.IsSoundEffectActive)
                _audioService.Play(Audio.AwaitingOpponent, true);

            DuelSignalrListener();// SignalR listener load

            await RandomUserProfilePicturesAsync();

            DuelOpenCommand.Execute(null);
        }

        #endregion Methods

        #region Commands

        public ICommand AnimationCommand;

        public ICommand DuelCloseCommand => new Command(async () => await ExecuteDuelCloseCommandAsync());
        private ICommand DuelOpenCommand => new Command(async () => await ExecuteDuelOpenRandomAsync());

        #endregion Commands
    }
}
