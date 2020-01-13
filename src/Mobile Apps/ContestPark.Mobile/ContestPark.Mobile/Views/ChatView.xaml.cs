using ContestPark.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatView : ContentPage
    {
        #region Constructor

        public ChatView()
        {
            InitializeComponent();
            Shell.SetBackButtonBehavior(this, new BackButtonBehavior() { IconOverride = ImageSource.FromFile("menuicon.png") });
        }

        #endregion Constructor

        #region Methods

        protected override void OnAppearing()
        {
            base.OnAppearing();

            ChatViewModel viewModel = ((ChatViewModel)BindingContext);

            if (viewModel == null || viewModel.IsInitialized)
                return;

            viewModel.InitializeCommand.Execute(null);
            viewModel.IsInitialized = true;
        }

        #endregion Methods
    }
}
