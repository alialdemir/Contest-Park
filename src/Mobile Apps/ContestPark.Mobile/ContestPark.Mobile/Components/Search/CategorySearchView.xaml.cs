using ContestPark.Mobile.Models.Categories;
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
                TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer
                {
                    Command = value,
                    CommandParameter = ((SearchModel)BindingContext).SubCategoryId
                };

                thumListItem.GestureRecognizers.Add(tapGestureRecognizer);

                imgCategory.GestureRecognizers.Add(tapGestureRecognizer);
            }
        }

        #endregion Properties
    }
}
