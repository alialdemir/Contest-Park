using ContestPark.Mobile.Models.Chat;
using ContestPark.Mobile.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ContestPark.Mobile.Views
{
    public partial class ChatDetailView : ContentPage
    {
        #region Constructor

        public ChatDetailView()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, true);
        }

        #endregion Constructor

        #region ViewModel

        private ChatDetailViewModel ViewModel
        {
            get
            {
                return BindingContext as ChatDetailViewModel;
            }
        }

        #endregion ViewModel

        #region Methods

        protected override void OnDisappearing()
        {
            ViewModel.GotoBackCommand.Execute(false);

            base.OnDisappearing();
        }

        protected override void OnAppearing()
        {
            ViewModel.ListViewScrollToBottomCommand = new Command<int>((index) =>
            {
                if (ViewModel.Items.Count > 0 && index >= 0 && index < ViewModel.Items.Count)
                    lstMessages.ScrollTo(ViewModel.Items[index], ScrollToPosition.End, true);
            });

            ViewModel.EditorFocusCommand = new Command(() => txtChatbox.Focus());
        }

        #endregion Methods

        #region Events

        private void OnItemAppearing(object Sender, ItemVisibilityEventArgs e)
        {
            var currentItem = (ChatDetailModel)e.Item;
            if (!ViewModel.IsInitialized || !ViewModel.ServiceModel.HasNextPage || !(currentItem is ChatDetailModel))
                return;

            if (ViewModel.Items.FirstOrDefault().Equals(currentItem))
                ViewModel.InitializeCommand.Execute(null);
        }

        /// <summary>
        /// Enrty'e tıklanıncca scroll'u aşaya çeker
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtChatbox_Focused(object sender, FocusEventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Task.Delay(1000);

                ViewModel.ListViewScrollToBottomCommand.Execute(ViewModel.Items.Count - 1);
            });
        }

        /// <summary>
        /// Entry dolu ise gönder buttonu aktif boş ise pasif olsun
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtChatbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnBtnSendMessage.Opacity = txtChatbox.Text.Trim().Length <= 0 ? 0.20 : 1;
        }

        #endregion Events
    }
}
