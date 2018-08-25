using Prism.Events;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterDetailView : Xamarin.Forms.MasterDetailPage
    {
        #region Constructor

        public MasterDetailView(IEventAggregator eventAggregator)
        {
            InitializeComponent();
            //eventAggregator
            //            .GetEvent<MasterDetailPageIsPresentedEvent>()
            //            .Subscribe((isPresented) => IsPresented = isPresented);
        }

        #endregion Constructor
    }
}