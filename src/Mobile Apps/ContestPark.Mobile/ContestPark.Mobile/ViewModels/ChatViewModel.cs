using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Models.Chat;
using ContestPark.Mobile.Services.Chat;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
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

        #endregion Private variables

        #region Constructor

        public ChatViewModel(IChatService chatService,
                             INavigationService navigationService,
                             IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            Title = ContestParkResources.Chat;
            _chatService = chatService;
        }

        #endregion Constructor

        #region Properties

        private string _badgeCount;

        public string BadgeCount
        {
            get { return _badgeCount; }
            set
            {
                _badgeCount = value;

                RaisePropertyChanged(() => BadgeCount);
            }
        }

        public bool IsDeleteMessageBusy { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Mesaj sil
        /// </summary>
        /// <param name="chatId">Chat Id</param>
        public async Task ExecuteDeleteItemCommandAsync(string receiverUserId)
        {
            // TODO: örneğin 3 mesaj olsun 2. sıradaki mesajı silip daha sonra 3. mesajı silince sanki 2. mesajın id si geliyor
            if (IsDeleteMessageBusy)
                return;

            var selectedModel = Items.FirstOrDefault(i => i.SenderUserId == receiverUserId);
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

                bool isSuccess = await _chatService.DeleteAsync(receiverUserId);
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
        public async Task GotoChatDetail(string receiverUserId)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            var selectedModel = Items.FirstOrDefault(i => i.SenderUserId == receiverUserId);
            if (selectedModel != null)
            {
                await PushNavigationPageAsync(nameof(ChatDetailView), new NavigationParameters
                {
                    { "UserName", selectedModel.UserName},
                    { "FullName", selectedModel.UserFullName},
                    { "SenderUserId", selectedModel.SenderUserId},
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

            if (!string.IsNullOrEmpty(BadgeCount))
            {
                bool isSuccess = await _chatService.ChatSeenAsync();
                if (isSuccess)
                    BadgeCount = string.Empty;
            }

            IsBusy = false;
        }

        /// <summary>
        /// Profile sayfasına git
        /// </summary>
        /// <param name="userName">Profili açılacak kullanıcının kullanıcı adı</param>
        private async Task ExecuteGotoProfilePageCommand(string userName)
        {
            if (!string.IsNullOrEmpty(userName))
            {
                await PushNavigationPageAsync(nameof(ProfileView), new NavigationParameters
                {
                    {"UserName", userName }
                });
            }
        }

        /// <summary>
        /// Kullanıcının görülmemiş mesaj sayısı
        /// </summary>
        private async Task UserChatVisibilityCountCommandAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            BadgeCount = (await _chatService.UserChatVisibilityCountAsync()).ToString();

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
                return deleteItemCommand ?? (deleteItemCommand = new Command<string>(async (senderUserId) => await ExecuteDeleteItemCommandAsync(senderUserId)));
            }
        }

        public ICommand GotoChatDetailCommand
        {
            get { return gotoChatDetailCommand ?? (gotoChatDetailCommand = new Command<string>(async (senderUserId) => await GotoChatDetail(senderUserId))); }
        }

        public ICommand GotoProfilePageCommand =>
            _gotoProfilePageCommand ?? (_gotoProfilePageCommand = new Command<string>(async (userName) => await ExecuteGotoProfilePageCommand(userName)));

        public ICommand UserChatVisibilityCountCommand => new Command(async () => await UserChatVisibilityCountCommandAsync());

        #endregion Commands
    }
}