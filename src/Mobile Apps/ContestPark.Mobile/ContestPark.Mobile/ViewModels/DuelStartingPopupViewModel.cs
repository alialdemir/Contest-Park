using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Models.Duel;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.ServiceModel;
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
using System.Linq;
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

        public StandbyModes StandbyMode { get; set; }

        public string SubCategoryPicturePath { get; set; }

        public string SubcategoryName { get; set; }

        public StandbyModeModel StandbyModeModel { get; private set; } = new StandbyModeModel();
        private DuelStartingModel _duelScreen = new DuelStartingModel();

        public DuelStartingModel DuelScreen
        {
            get
            {
                return _duelScreen;
            }
            set
            {
                _duelScreen = value;
                RaisePropertyChanged(() => DuelScreen);
            }
        }

        private bool RandomPicturStatus { get; set; } = true;

        /// <summary>
        /// Eğer sorular gelmişse quiz view geçerken true ise alert çıkmasın
        /// </summary>
        public bool IsNextQuestionExit { get; set; } = false;

        #endregion Properties

        #region SignalR

        private void DuelSignalrListener()
        {
            _duelSignalRService.DuelScreenInfoEventHandler += SetDuelScreen;
            _duelSignalRService.DuelScreenInfo();

            _duelSignalRService.NextQuestionEventHandler += NextQuestion;
            _duelSignalRService.NextQuestion();
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
#if !(DEBUG)
                await DisplayAlertAsync(
                    ContestParkResources.Error,
                    ContestParkResources.ConnectionToTheServerForTheDuelWasNotEstablished,
                    ContestParkResources.Okay);

#else
                await DisplayAlertAsync(
                    ContestParkResources.Error,
                    "Signalr connection id boş geldi",
                    ContestParkResources.Okay);
#endif

                DuelCloseCommand.Execute(null);
            }
            else if (!StandbyModes.Off.HasFlag(StandbyMode))
            {
                // TODO: direk user id ile gelirse veya direk Duel id(notification) ile gelirse gibi durumlar..
            }
            else
            {
                await StandbyModeOff();
            }

            await base.InitializeAsync();

            IsBusy = false;
        }

        /// <summary>
        /// Bekleme modundayken yapılan işlemler
        /// </summary>
        private async Task StandbyModeOff()
        {
            DuelScreen = new DuelStartingModel()
            {
                FounderFullName = _settingsService.UserInfo.FullName,
                FounderCoverPicturePath = _settingsService.UserInfo.CoverPicturePath,
                FounderProfilePicturePath = _settingsService.UserInfo.ProfilePicturePath
            };

            _audioService.Play(Audio.AwaitingOpponent, true);

            DuelSignalrListener();// SignalR listener load

            await RandomUserProfilePicturesAsync();

            DuelOpenCommand.Execute(null);
        }

        /// <summary>
        /// Ekrandaki bilgileri yeniler
        /// </summary>
        private void SetDuelScreen(object sender, DuelStartingModel e)
        {
            DuelStartingModel duelEnterScreenModel = (DuelStartingModel)sender;
            if (duelEnterScreenModel != null)
            {
                RandomPicturStatus = false;

                AnimationCommand?.Execute(null);

                DuelScreen = duelEnterScreenModel;
            }
            else
            {
                // TODO: boş gelirse problem vardır bekleme modunu kapat
            }
        }

        /// <summary>
        /// Düellodaki sıradaki soruyu alır
        /// </summary>
        private async void NextQuestion(object sender, QuestionModel e)
        {
            QuestionModel questionModel = (QuestionModel)sender;
            if (questionModel == null)
            {
                // TODO: Server tarafına düello iptali için istek gönder bahis miktarı geri kullanıcıya eklensin.
                return;
            }

            if (DuelScreen.OpponentFullName != ContestParkResources.AwaitingOpponent)
            {
                AudioStop();

                await Task.Delay(3000); // Rakibi görebilmesi için 3sn beklettim

                IsNextQuestionExit = true;
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await PushPopupPageAsync(new QuestionPopupView
                    {
                        Question = questionModel,
                        DuelScreen = DuelScreen,
                        SubcategoryName = SubcategoryName,
                        SubCategoryPicturePath = SubCategoryPicturePath
                    });

                    DuelCloseCommand.Execute(null);
                });
            }
            else
            {
                // TODO: rakip fotoğrafınn gelmesini beklet
            }
        }

        /// <summary>
        /// Rakip bekliyor moduna aldık
        /// </summary>
        private async Task ExecuteDuelOpenRandomAsync()
        {
            StandbyModeModel.ConnectionId = _settingsService.SignalRConnectionId;

            await _duelService.StandbyMode(StandbyModeModel);// TODO: success kontrol et hata oluşursa mesaj çıksın
        }

        /// <summary>
        /// Rakip araken bekleme ekranı için gerekli işlemler
        /// </summary>
        private async Task RandomUserProfilePicturesAsync()
        {
            DuelScreen.OpponentFullName = ContestParkResources.AwaitingOpponent;

            await RandomPicturesAsync();
        }

        /// <summary>
        /// Bekleme modundayken rakip profil resmini değiştir
        /// </summary>
        private async Task RandomPicturesAsync()
        {
            ServiceModel<string> serviceModel = await _duelService.RandomUserProfilePictures(new PagingModel() { PageSize = 30 });

            if (serviceModel?.Items == null)
                return;

            string[] pictures = serviceModel.Items.ToArray();
            int pictureIndex = pictures.Length;

            Device.StartTimer(new TimeSpan(0, 0, 0, 0, 700), () =>
            {
                if (pictureIndex >= 0)
                {
                    pictureIndex--;

                    if (pictureIndex < 0)
                        pictureIndex = pictures.Length - 1;

                    DuelScreen.OpponentProfilePicturePath = pictures[pictureIndex];
                }

                return RandomPicturStatus;
            });
        }

        /// <summary>
        /// Bekleme ekranı kapatır
        /// </summary>
        private async Task ExecuteDuelCloseCommandAsync()
        {
            if (DuelScreen.DuelId != 0 && !IsNextQuestionExit)// Duel id 0 değilse düello başlamıştır
            {
                bool isOk = await DisplayAlertAsync(ContestParkResources.Exit,
                                                    ContestParkResources.AreYouSureYouWantToLeave,
                                                    ContestParkResources.Okay,
                                                    ContestParkResources.Cancel);
                if (!isOk)
                {
                    return;
                }
            }

            AudioStop();

            RandomPicturStatus = false;

            // Signalr events remove
            _duelSignalRService.DuelScreenInfoEventHandler -= SetDuelScreen;
            _duelSignalRService.NextQuestionEventHandler -= NextQuestion;

            _duelSignalRService.OffDuelScreenInfo();
            _duelSignalRService.OffNextQuestion();

            await RemoveFirstPopupAsync();

            if (DuelScreen.DuelId == 0)
            {
                await _duelService.ExitStandMode(StandbyModeModel);
            }
        }

        /// <summary>
        /// Sesi kapatır
        /// </summary>
        private void AudioStop()
        {
            _audioService?.Stop();
        }

        #endregion Methods

        #region Commands

        public ICommand AnimationCommand;

        private ICommand DuelOpenCommand => new Command(async () => await ExecuteDuelOpenRandomAsync());

        public ICommand DuelCloseCommand => new Command(async () => await ExecuteDuelCloseCommandAsync());

        #endregion Commands
    }
}