using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Events;
using Prism.Events;
using Prism.Ioc;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterDetailView : MasterDetailPage
    {
        #region Constructor

        public MasterDetailView()
        {
            InitializeComponent();

            RegisterTypesConfig.Container.Resolve<IEventAggregator>()
                    ?.GetEvent<MasterDetailPageIsPresentedEvent>()
                    .Subscribe((isPresented) => IsPresented = isPresented);
        }

        #endregion Constructor
    }
}
