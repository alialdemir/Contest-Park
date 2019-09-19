using ContestPark.Mobile.Models.Chat;
using ContestPark.Mobile.ViewModels;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatDetailView : ContentPage
    {
        #region Constructor

        public ChatDetailView()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, true);
            Shell.SetTabBarIsVisible(this, false);// Altta tabbar gözükmemesi için ekledim
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

        protected override void OnAppearing()
        {
            if (ViewModel.IsInitialized)
                return;

            ViewModel.InitializeCommand.Execute(null);

            ViewModel.ListViewScrollToBottomCommand = new Command<int>((index) =>
            {
                if (ViewModel.Items.Count > 0 && index >= 0 && index < ViewModel.Items.Count)
                    lstMessages.ScrollTo(ViewModel.Items[index], ScrollToPosition.Center, true);
            });

            ViewModel.EditorFocusCommand = new Command(() => txtChatbox.Focus());

            ViewModel.IsInitialized = true;
        }

        #endregion Methods

        #region Events

        private void OnItemAppearing(object Sender, ItemVisibilityEventArgs e)
        {
            var currentItem = (ChatDetailModel)e.Item;
            if (!ViewModel.IsInitialized || ViewModel.ServiceModel.IsLastPage || !(currentItem is ChatDetailModel))
                return;

            if (ViewModel.Items.FirstOrDefault().Equals(currentItem))
                ViewModel.InitializeCommand.Execute(null);
        }

        /// <summary>
        /// Mesaj gönder
        /// </summary>
        /// <param name="sender">Entry objesi</param>
        /// <param name="e">EventArgs</param>
        private void txtChatbox_Completed(object sender, EventArgs e)
        {
            txtChatbox.Focus();

            ViewModel.SendMessageCommand.Execute(null);
        }

        /// <summary>
        /// Enrty'e tıklanıncca scroll'u aşaya çeker
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtChatbox_Focused(object sender, FocusEventArgs e)
        {
            ViewModel.ListViewScrollToBottomCommand.Execute(0);
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
