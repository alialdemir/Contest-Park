using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Dependencies;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Chat;
using ContestPark.Mobile.Services.Analytics;
using ContestPark.Mobile.Services.Blocking;
using ContestPark.Mobile.Services.Chat;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Prism.Events;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class ChatDetailViewModel : ViewModelBase<ChatDetailModel>
    {
        #region Private variable

        public readonly ISettingsService _settingsService;
        private readonly IAnalyticsService _analyticsService;
        private readonly IBlockingService _blockingService;
        private readonly IChatService _chatService;
        private readonly IEventAggregator _eventAggregator;
        private string _fullName, _userName;
        // private readonly IChatsSignalRService _chatsSignalRService;

        #endregion Private variable

        #region Constructors

        public ChatDetailViewModel(INavigationService navigationService,
                                   IPageDialogService pageDialogService,
                                   IChatService chatService,
                                   IAnalyticsService analyticsService,
                                   IBlockingService blockingService,
                                   IEventAggregator eventAggregator,
                                   ISettingsService settingsService) : base(navigationService, pageDialogService)
        {
            _chatService = chatService;
            _analyticsService = analyticsService;
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

        /// <summary>
        /// Konuştuğu kullanıcı id
        /// </summary>
        public string SenderUserId { get; private set; }

        #endregion Properties

        #region Methods

        protected override async Task InitializeAsync(INavigationParameters parameters = null)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            if (parameters.ContainsKey("UserName")) _userName = parameters.GetValue<string>("UserName");
            if (parameters.ContainsKey("FullName")) _fullName = Title = parameters.GetValue<string>("FullName");
            if (parameters.ContainsKey("SenderUserId")) SenderUserId = parameters.GetValue<string>("SenderUserId");
            if (parameters.ContainsKey("SenderProfilePicturePath")) SenderProfilePicturePath = parameters.GetValue<string>("SenderProfilePicturePath");

            StartHub();

            ServiceModel = await _chatService.ChatDetailAsync(SenderUserId, ServiceModel);

            await base.InitializeAsync(parameters);

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

            bool isSuccess;
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

            List<string> buttons = new List<string>()
            {
                ContestParkResources.DeleteConversation
            };

            if (Items.Count != 0)
            {
                buttons.Add(await IsBlockingAsync() == true ? ContestParkResources.RemoveBlock : ContestParkResources.Block);
            }

            string selectedActionSheet = await DisplayActionSheetAsync(
                ContestParkResources.ChatSettings,
                ContestParkResources.Cancel, null,
                buttons.ToArray());

            IsBusy = false;

            if (selectedActionSheet == ContestParkResources.DeleteConversation)
                DeleteMessageCommand.Execute(null);
            else if (buttons.Count != 1 && selectedActionSheet == buttons[1])
                BlockingProgressCommand.Execute(await IsBlockingAsync());
        }

        /// <summary>
        /// İki kullanıcı arasındaki mesajları siler
        /// </summary>
        private async Task ExecuteDeleteMessageCommandAsync()
        {
            if (IsBusy || Items == null || Items.Count == 0 || Items.FirstOrDefault().ConversationId == 0)
                return;

            IsBusy = true;

            bool isDelete = await DisplayAlertAsync(
                ContestParkResources.DeleteThisEntireConversation,
                ContestParkResources.IfYouDeleteTheCopyOfThisSpeechThatCanNotBeUndone,
                ContestParkResources.DeleteConversation,
                ContestParkResources.Cancel);

            if (isDelete)
            {
                long conversationId = Items.FirstOrDefault().ConversationId;
                if (conversationId == 0)
                    return;

                ChatDetailModel[] items = new ChatDetailModel[Items.Count];
                Items.CopyTo(items, 0);

                Items.Clear();

                bool isRemoveMessages = await _chatService.DeleteAsync(conversationId);

                if (!isRemoveMessages)// eğer hata olursa mesajları geri yüklüyoruz
                {
                    _analyticsService.SendEvent("Mesaj", "Mesaj Detay Sil", "Fail");

                    Items.AddRange(items);

                    await DisplayAlertAsync("",
                           ContestParkResources.GlobalErrorMessage,
                           ContestParkResources.Okay);
                }
                else
                {
                    _analyticsService.SendEvent("Mesaj", "Mesaj Detay Sil", "Success");

                    _eventAggregator
                                .GetEvent<MessageRefleshEvent>()
                                .Publish(conversationId);
                }
            }

            IsBusy = false;
        }

        /// <summary>
        /// Profile sayfasına git
        /// </summary>
        /// <param name="userName">Profili açılacak kullanıcının kullanıcı adı</param>
        private void ExecuteGotoProfilePageCommand()
        {
            if (IsBusy || string.IsNullOrEmpty(_userName))
                return;

            IsBusy = true;

            NavigateToAsync<ProfileView>(new NavigationParameters
                {
                    {"UserName", _userName }
                });

            IsBusy = false;
        }

        /// <summary>
        /// Mesaj gönder
        /// </summary>
        private async Task ExecuteSendMessageCommand()
        {
            if (IsBusy || string.IsNullOrEmpty(Message))
                return;

            IsBusy = true;

            _analyticsService.SendEvent("Mesaj", "Mesaj Gönderildi", $"{_settingsService.CurrentUser.UserName} - {_userName}");

            var lastMessage = new ChatDetailModel
            {
                Date = DateTime.Now,
                Message = Message,
                IsIncoming = true,
                SenderId = _settingsService.CurrentUser.UserId
            };

            Items.Add(lastMessage);

            bool isSuccess = await _chatService.SendMessage(new MessageModel
            {
                Text = Message.Trim(),
                ReceiverUserId = SenderUserId,
            });

            if (!isSuccess)
            {
                Items.Remove(lastMessage);
            }

            ListViewScrollToBottomCommand?.Execute(Items.Count - 1);

            EditorFocusCommand?.Execute(null);

            Message = String.Empty;

            IsBusy = false;

            return;
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

        public override Task GoBackAsync(INavigationParameters parameters = null, bool? useModalNavigation = false)
        {
            Xamarin.Forms.DependencyService.Get<IDevice>().DismissKeyboard();

            return Task.CompletedTask;
        }

        #endregion Methods

        #region Commands

        private ICommand _blockingProgressCommand;

        /// <summary>
        /// Sohbet ayaralrı toolbaritem command
        /// </summary>
        private ICommand _chatSettingsCommand;

        private ICommand _deleteMessageCommand;

        private ICommand _gotoProfileCommand;

        private ICommand _sendMessageCommand;

        /// <summary>
        /// Engelleme işlemi command
        /// </summary>
        public ICommand BlockingProgressCommand
        {
            get { return _blockingProgressCommand ?? (_blockingProgressCommand = new CommandAsync<bool>(ExecuteBlockingProgressCommandAsync)); }
        }

        public ICommand ChatSettingsCommand
        {
            get { return _chatSettingsCommand ?? (_chatSettingsCommand = new CommandAsync(ExecuteChatSettingsCommandAsync)); }
        }

        public ICommand DeleteMessageCommand
        {
            get { return _deleteMessageCommand ?? (_deleteMessageCommand = new CommandAsync(ExecuteDeleteMessageCommandAsync)); }
        }

        public ICommand EditorFocusCommand { get; set; }

        /// <summary>
        /// Konuştuğu kişinin profil resmine tıklayınca profiline gitmesi için command
        /// </summary>
        public ICommand GotoProfileCommand
        {
            get { return _gotoProfileCommand ?? (_gotoProfileCommand = new Command(ExecuteGotoProfilePageCommand)); }
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
            get { return _sendMessageCommand ?? (_sendMessageCommand = new CommandAsync(ExecuteSendMessageCommand)); }
        }

        #endregion Commands
    }
}
