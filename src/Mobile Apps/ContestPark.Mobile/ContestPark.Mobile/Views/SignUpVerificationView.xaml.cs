using ContestPark.Mobile.Components;
using ContestPark.Mobile.Models.Notification;
using ContestPark.Mobile.ViewModels;
using Rg.Plugins.Popup.Pages;

namespace ContestPark.Mobile.Views
{
    public partial class SignUpVerificationView : PopupPage
    {
        #region Constructor

        public SignUpVerificationView()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Properties

        public SmsInfoModel SmsInfo { get; set; }

        #endregion Properties

        #region Methods

        protected override void OnAppearing()
        {
            base.OnAppearing();

            var viewModel = ((SignUpVerificationViewModel)BindingContext);

            if (viewModel == null || viewModel.IsInitialized)
                return;

            viewModel.SmsInfo = SmsInfo;
            viewModel.InitializeCommand.Execute(null);
            viewModel.IsInitialized = true;
        }

        private void Code_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            CustomEntry entry = (CustomEntry)sender;
            if (string.IsNullOrEmpty(entry.Text))
                return;

            if (sender == code1)
            {
                code2.Focus();
            }
            else if (sender == code2)
            {
                code3.Focus();
            }
            else if (sender == code3)
            {
                code4.Focus();
            }
        }

        #endregion Methods
    }
}
