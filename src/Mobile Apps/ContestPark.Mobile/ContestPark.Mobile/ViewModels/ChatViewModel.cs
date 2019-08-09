using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Models.Chat;
using ContestPark.Mobile.Services.Chat;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Prism.Events;
using Prism.Navigation;
using Prism.Services;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class ChatViewModel : ViewModelBase<ChatModel>
    {
        #region Private variables

        private readonly IChatService _chatService;
        private readonly IEventAggregator _eventAggregator;

        #endregion Private variables

        #region Constructor

        public ChatViewModel(IChatService chatService,
                             INavigationService navigationService,
                             IEventAggregator eventAggregator,
                             IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            Title = ContestParkResources.Chat;
            _chatService = chatService;
            _eventAggregator = eventAggregator;
        }

        #endregion Constructor

        #region Properties

        public bool IsDeleteMessageBusy { get; set; }

        private string _messageBadgeCount;

        public string MessageBadgeCount
        {
            get { return _messageBadgeCount; }
            set
            {
                if (value != "0")
                {
                    _messageBadgeCount = value;

                    RaisePropertyChanged(() => MessageBadgeCount);
                }
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Mesaj sil
        /// </summary>
        /// <param name="chatId">Chat Id</param>
        public async Task ExecuteDeleteItemCommandAsync(long conversationId)
        {
            // TODO: örneğin 3 mesaj olsun 2. sıradaki mesajı silip daha sonra 3. mesajı silince sanki 2. mesajın id si geliyor
            if (IsDeleteMessageBusy)
                return;

            var selectedModel = Items.FirstOrDefault(i => i.ConversationId == conversationId);
            if (selectedModel == null)
                return;

            IsDeleteMessageBusy = true;

            bool isDelete = await DisplayAlertAsync(
                                          ContestParkResources.DeleteMessageTitle,
                                          string.Format(ContestParkResources.DeleteMessage, selectedModel.UserFullName),
                                          ContestParkResources.Delete,
                                          ContestParkResources.Cancel);

            if (isDelete)
            {
                Items.Remove(selectedModel);

                bool isSuccess = await _chatService.DeleteAsync(conversationId);
                if (!isSuccess)
                {
                    Items.Add(selectedModel);
                    Items.OrderByDescending(x => x.Date);

                    await DisplayAlertAsync("",
                        ContestParkResources.WeHadaProblemDeletingTheMessagePleaseTryAgain,
                        ContestParkResources.Okay);
                }
            }

            IsDeleteMessageBusy = false;
        }

        /// <summary>
        /// Mesaj detayına git
        /// </summary>
        /// <param name="receiverUserId">alıcının kullanıcı id</param>
        public async Task GotoChatDetail(long conversationId)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            var selectedModel = Items.FirstOrDefault(i => i.ConversationId == conversationId);
            if (selectedModel != null)
            {
                await PushNavigationPageAsync(nameof(ChatDetailView), new NavigationParameters
                {
                    { "UserName", selectedModel.UserName},
                    { "FullName", selectedModel.UserFullName},
                    { "SenderUserId", selectedModel.SenderUserId},
                    { "ConversationId", selectedModel.ConversationId},
                    {"SenderProfilePicturePath", selectedModel.UserProfilePicturePath }
                });
            }

            IsBusy = false;
        }

        protected override async Task InitializeAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            ServiceModel = await _chatService.UserChatList(ServiceModel);
            await base.InitializeAsync();

            //////if (!string.IsNullOrEmpty(BadgeCount))
            //////{
            //////    bool isSuccess = await _chatService.ChatSeenAsync();
            //////    if (isSuccess)
            //////        BadgeCount = string.Empty;
            //////}

            SubscriptionReflesh();

            IsBusy = false;
        }

        /// <summary>
        /// Mesaj silme eventini dinler
        /// örneğin mesaj detayından tüm mesajlar silinirse buradaki sayfadanda silmesi için event tanımladık
        /// </summary>
        private void SubscriptionReflesh()
        {
            _eventAggregator.GetEvent<MessageRefleshEvent>()
                .Subscribe((conversationId) =>
                {
                    var removedItem = Items.FirstOrDefault(i => i.ConversationId == conversationId);
                    if (removedItem != null)
                    {
                        Items.Remove(removedItem);
                    }
                });
        }

        /// <summary>
        /// Profile sayfasına git
        /// </summary>
        /// <param name="userName">Profili açılacak kullanıcının kullanıcı adı</param>
        private async Task ExecuteGotoProfilePageCommand(string userName)
        {
            if (IsBusy || string.IsNullOrEmpty(userName))
                return;

            IsBusy = true;

            await PushNavigationPageAsync(nameof(ProfileView), new NavigationParameters
                {
                    {"UserName", userName }
                });

            IsBusy = false;
        }

        /// <summary>
        /// Kullanıcının görülmemiş mesaj sayısı
        /// </summary>
        private async Task UserChatVisibilityCountCommandAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            MessageBadgeCount = (await _chatService.UserChatVisibilityCountAsync()).ToString();

            IsBusy = false;
        }

        #endregion Methods

        #region Commands

        private ICommand _gotoProfilePageCommand;
        private ICommand deleteItemCommand;
        private ICommand gotoChatDetailCommand;

        public ICommand DeleteItemCommand
        {
            get
            {
                return deleteItemCommand ?? (deleteItemCommand = new Command<long>(async (conversationId) => await ExecuteDeleteItemCommandAsync(conversationId)));
            }
        }

        public ICommand GotoChatDetailCommand
        {
            get { return gotoChatDetailCommand ?? (gotoChatDetailCommand = new Command<long>(async (conversationId) => await GotoChatDetail(conversationId))); }
        }

        public ICommand GotoProfilePageCommand =>
            _gotoProfilePageCommand ?? (_gotoProfilePageCommand = new Command<string>(async (userName) => await ExecuteGotoProfilePageCommand(userName)));

        public ICommand UserChatVisibilityCountCommand => new Command(async () => await UserChatVisibilityCountCommandAsync());

        #endregion Commands

        #region Navigation

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (IsInitialized)
                return;

            InitializeCommand.Execute(null);
            IsInitialized = true;
        }

        public override void OnNavigatingTo(INavigationParameters parameters)
        {
            // tabs sayfalarında ilk açılışta tüm dataları çekmesin sayfaya gelirse çeksin diye base methodu ezdik
        }

        #endregion Navigation
    }
}
