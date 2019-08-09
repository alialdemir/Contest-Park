using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Models.Chat;
using ContestPark.Mobile.Services.Blocking;
using ContestPark.Mobile.Services.Chat;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Prism.Events;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class ChatDetailViewModel : ViewModelBase<ChatDetailModel>
    {
        #region Private variable

        public readonly ISettingsService _settingsService;
        private readonly IBlockingService _blockingService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IChatService _chatService;
        private string _fullName, _userName;
        // private readonly IChatsSignalRService _chatsSignalRService;

        #endregion Private variable

        #region Constructors

        public ChatDetailViewModel(INavigationService navigationService,
                                   IPageDialogService pageDialogService,
                                   IChatService chatService,
                                   IBlockingService blockingService,
                                   IEventAggregator eventAggregator,
                                   ISettingsService settingsService) : base(navigationService, pageDialogService)
        {
            _chatService = chatService;
            _settingsService = settingsService;
            _blockingService = blockingService;
            _eventAggregator = eventAggregator;
            //    _chatsSignalRService = chatsSignalRService;
        }

        #endregion Constructors

        #region SignalR

        /// <summary>
        /// Gelen mesajları dinleme
        /// </summary>
        private void Client_OnDataReceived(object sender, ChatDetailModel chatHistory)
        {
            if (SenderUserId == chatHistory.SenderId)
            {
                Items.Add(chatHistory);

                Device.StartTimer(new TimeSpan(0, 0, 3), () =>
                {
                    ListViewScrollToBottomCommand?.Execute(Items.Count - 1);
                    return false;
                });
            }
            // tab dan badge attır ve chat list yenile
        }

        private void StartHub()
        {
            //   _chatsSignalRService.OnDataReceived += Client_OnDataReceived;
            //    _chatsSignalRService.ChatProxy();
        }

        #endregion SignalR

        #region Properties

        private string _message = string.Empty;

        private string _senderProfilePicturePath;
        public bool? IsBlocking { get; set; }

        /// <summary>
        /// Yazdığı mesaj
        /// </summary>
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                RaisePropertyChanged(() => Message);
            }
        }

        /// <summary>
        /// Karşı konuşmacının profil resmi
        /// </summary>
        public string SenderProfilePicturePath
        {
            get
            {
                return _senderProfilePicturePath;
            }
            set
            {
                _senderProfilePicturePath = value;
                RaisePropertyChanged(() => SenderProfilePicturePath);
            }
        }

        private long ConversationId { get; set; }

        /// <summary>
        /// Konuştuğu kullanıcı id
        /// </summary>
        public string SenderUserId { get; private set; }

        #endregion Properties

        #region Methods

        protected override async Task InitializeAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            ServiceModel = await _chatService.ChatDetailAsync(ConversationId, ServiceModel);

            await base.InitializeAsync();

            ListViewScrollToBottomCommand?.Execute(ServiceModel.PageNumber == 1 ? Items.Count - 1 : Items.Count / 3);

            IsBusy = false;
        }

        /// <summary>
        /// Kullanıcı engelle
        /// </summary>
        /// <param name="isBlocking"></param>
        private async Task ExecuteBlockingProgressCommandAsync(bool isBlocking)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            bool isSuccess = false;

            if (await IsBlockingAsync() == true)
                isSuccess = await _blockingService.UnBlock(SenderUserId);
            else
                isSuccess = await _blockingService.Block(SenderUserId);

            if (isSuccess)// eğer engel kaldırır veya engellerse localdeki engelleme durumu sıfırlamak için null atadık tekrar IsBlockingAsync çağrıldığında sunucudan çeker
                IsBlocking = null;

            IsBusy = false;
        }

        /// <summary>
        /// Mesaj ayarları
        /// </summary>
        /// <returns></returns>
        private async Task ExecuteChatSettingsCommandAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            string buttons = await IsBlockingAsync() == true ? ContestParkResources.RemoveBlock : ContestParkResources.Block;

            string selectedActionSheet = await DisplayActionSheetAsync(
                ContestParkResources.ChatSettings,
                ContestParkResources.Cancel, null,
                ContestParkResources.DeleteConversation, buttons);

            IsBusy = false;

            if (selectedActionSheet == ContestParkResources.DeleteConversation)
                DeleteMessageCommand.Execute(null);
            else if (selectedActionSheet == buttons)
                BlockingProgressCommand.Execute(await IsBlockingAsync());
        }

        /// <summary>
        /// İki kullanıcı arasındaki mesajları siler
        /// </summary>
        private async Task ExecuteDeleteMessageCommandAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            bool isDelete = await DisplayAlertAsync(
                ContestParkResources.DeleteThisEntireConversation,
                ContestParkResources.IfYouDeleteTheCopyOfThisSpeechThatCanNotBeUndone,
                ContestParkResources.DeleteConversation,
                ContestParkResources.Cancel);

            if (isDelete)
            {
                ChatDetailModel[] items = new ChatDetailModel[Items.Count];
                Items.CopyTo(items, 0);

                Items.Clear();

                bool isRemoveMessages = await _chatService.DeleteAsync(ConversationId);

                if (!isRemoveMessages)// eğer hata olursa mesajları geri yüklüyoruz
                {
                    Items.AddRange(items);

                    await DisplayAlertAsync("",
                           ContestParkResources.GlobalErrorMessage,
                           ContestParkResources.Okay);
                }
                else
                {
                    _eventAggregator
                                .GetEvent<MessageRefleshEvent>()
                                .Publish(ConversationId);
                }
            }

            IsBusy = false;
        }

        /// <summary>
        /// Profile sayfasına git
        /// </summary>
        /// <param name="userName">Profili açılacak kullanıcının kullanıcı adı</param>
        private async Task ExecuteGotoProfilePageCommand()
        {
            if (IsBusy || string.IsNullOrEmpty(_userName))
                return;

            IsBusy = true;

            await PushNavigationPageAsync(nameof(ProfileView), new NavigationParameters
                {
                    {"UserName", _userName }
                });

            IsBusy = false;
        }

        /// <summary>
        /// Mesaj gönder
        /// </summary>
        private Task ExecuteSendMessageCommand()
        {
            if (IsBusy || string.IsNullOrEmpty(_message))
                return Task.CompletedTask;

            IsBusy = true;

            Items.Add(new ChatDetailModel
            {
                Date = DateTime.Now,
                Message = _message,
                SenderId = _settingsService.CurrentUser.UserId
            });

            //////await _chatService.SendChat(new SendChatModel
            //////{
            //////    Message = _message.Trim(),
            //////    ReceiverId = SenderUserId,
            //////    PublicKey = "675b5dce-10cc-4bcd-b635-1e911f6c4eaa"// TODO: config gibi bir yerden çekilmeli
            //////});

            ListViewScrollToBottomCommand?.Execute(Items.Count - 1);

            EditorFocusCommand?.Execute(null);

            Message = String.Empty;

            IsBusy = false;

            return Task.CompletedTask;
        }

        /// <summary>
        /// İki kullanıcı arasında engel var mı kontrol eder
        /// </summary>
        /// <returns>Engellemiş ise true engellememiş ise false</returns>
        private async Task<bool?> IsBlockingAsync()
        {
            if (IsBlocking == null)
            {
                IsBlocking = await _blockingService.BlockingStatusAsync(SenderUserId);
            }

            return IsBlocking;
        }

        #endregion Methods

        #region Commands

        private ICommand blockingProgressCommand;

        /// <summary>
        /// Sohbet ayaralrı toolbaritem command
        /// </summary>
        private ICommand chatSettingsCommand;

        private ICommand deleteMessageCommand;

        private ICommand gotoProfileCommand;

        private ICommand sendMessageCommand;

        /// <summary>
        /// Engelleme işlemi command
        /// </summary>
        public ICommand BlockingProgressCommand
        {
            get { return blockingProgressCommand ?? (blockingProgressCommand = new Command<bool>(async (isBlocking) => await ExecuteBlockingProgressCommandAsync(isBlocking))); }
        }

        public ICommand ChatSettingsCommand
        {
            get { return chatSettingsCommand ?? (chatSettingsCommand = new Command(async () => await ExecuteChatSettingsCommandAsync())); }
        }

        public ICommand DeleteMessageCommand
        {
            get { return deleteMessageCommand ?? (deleteMessageCommand = new Command(async () => await ExecuteDeleteMessageCommandAsync())); }
        }

        public ICommand EditorFocusCommand { get; set; }

        /// <summary>
        /// Konuştuğu kişinin profil resmine tıklayınca profiline gitmesi için command
        /// </summary>
        public ICommand GotoProfileCommand
        {
            get { return gotoProfileCommand ?? (gotoProfileCommand = new Command(async () => await ExecuteGotoProfilePageCommand())); }
        }

        /// <summary>
        /// Listview scroll aşağıya çeker
        /// </summary>
        public ICommand ListViewScrollToBottomCommand { get; set; }

        /// <summary>
        /// Mesaj gönder command
        /// </summary>
        public ICommand SendMessageCommand
        {
            get { return sendMessageCommand ?? (sendMessageCommand = new Command(async () => await ExecuteSendMessageCommand())); }
        }

        #endregion Commands

        #region Navigations

        public override void OnNavigatingTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("UserName")) _userName = parameters.GetValue<string>("UserName");
            if (parameters.ContainsKey("FullName")) _fullName = Title = parameters.GetValue<string>("FullName");
            if (parameters.ContainsKey("SenderUserId")) SenderUserId = parameters.GetValue<string>("SenderUserId");
            if (parameters.ContainsKey("ConversationId")) ConversationId = parameters.GetValue<long>("ConversationId");
            if (parameters.ContainsKey("SenderProfilePicturePath")) SenderProfilePicturePath = parameters.GetValue<string>("SenderProfilePicturePath");

            StartHub();
            //base.OnNavigatingTo(parameters);
        }

        #endregion Navigations
    }
}
