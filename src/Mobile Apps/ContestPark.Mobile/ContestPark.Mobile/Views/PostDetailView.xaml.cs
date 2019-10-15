using ContestPark.Mobile.Models.Post;
using ContestPark.Mobile.ViewModels;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PostDetailView : ContentPage
    {
        #region Constructor

        public PostDetailView()
        {
            InitializeComponent();
            Shell.SetTabBarIsVisible(this, false);// Altta tabbar gözükmemesi için ekledim
        }

        #endregion Constructor

        #region ViewModel

        private PostDetailViewModel ViewModel
        {
            get
            {
                return BindingContext as PostDetailViewModel;
            }
        }

        #endregion ViewModel

        #region Methods

        protected override void OnAppearing()
        {
            if (ViewModel.ListViewScrollToBottomCommand != null && ViewModel.EditorFocusCommand != null)
                return;

            ViewModel.ListViewScrollToBottomCommand = new Command<int>((index) =>
            {
                if (ViewModel.Items.Count > 0 && index >= 0 && index < ViewModel.Items.Count)
                    lstMessages.ScrollTo(ViewModel.Items[index], ScrollToPosition.Center, true);
            });

            ViewModel.EditorFocusCommand = new Command(() => txtChatbox.Focus());
        }

        #endregion Methods

        #region Events

        private void OnItemAppearing(object Sender, ItemVisibilityEventArgs e)
        {
            var currentItem = (PostCommentModel)e.Item;
            if (!ViewModel.IsInitialized || !ViewModel.ServiceModel.HasNextPage || !(currentItem is PostCommentModel))
                return;

            if (ViewModel.Items.FirstOrDefault().Equals(currentItem))
                ViewModel.InitializeCommand?.Execute(null);
        }

        /// <summary>
        /// Mesaj gönder
        /// </summary>
        /// <param name="sender">Entry objesi</param>
        /// <param name="e">EventArgs</param>
        private void txtChatbox_Completed(object sender, EventArgs e)
        {
            txtChatbox.Focus();

            ViewModel.SendCommentCommand?.Execute(null);
        }

        /// <summary>
        /// Enrty'e tıklanıncca scroll'u aşaya çeker
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtChatbox_Focused(object sender, FocusEventArgs e)
        {
            ViewModel.ListViewScrollToBottomCommand?.Execute(0);
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
