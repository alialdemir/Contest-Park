using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Components.Search
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CategorySearchView : ContentView
    {
        #region Constructor

        public CategorySearchView()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Properties

        public ICommand PushCategoryDetailCommand
        {
            set
            {
                thumListItem.SingleTap = imgCategory.Command = value;
            }
        }

        #endregion Properties
    }
}
