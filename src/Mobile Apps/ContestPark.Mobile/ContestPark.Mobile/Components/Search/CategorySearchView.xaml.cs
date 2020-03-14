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

        public static readonly BindableProperty PushCategoryDetailCommandProperty = BindableProperty.Create(propertyName: nameof(PushCategoryDetailCommand),
                                                                                                            returnType: typeof(ICommand),
                                                                                                            declaringType: typeof(CategorySearchView),
                                                                                                            defaultValue: null);

        public ICommand PushCategoryDetailCommand
        {
            get { return (ICommand)GetValue(PushCategoryDetailCommandProperty); }
            set { SetValue(PushCategoryDetailCommandProperty, value); }
        }

        #endregion Properties
    }
}
