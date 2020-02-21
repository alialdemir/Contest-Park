using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Components
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SubCategoryCard : ContentView
    {
        #region Constructor

        public SubCategoryCard()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Properties

        public static readonly BindableProperty SubCategoriesDisplayActionSheetCommandProperty = BindableProperty.Create(propertyName: nameof(SubCategoriesDisplayActionSheetCommand),
                                                                                                                        returnType: typeof(ICommand),
                                                                                                                        declaringType: typeof(SubCategoryCard),
                                                                                                                        defaultValue: null);

        /// <summary>
        /// Alt kategori display alert command
        /// </summary>
        public ICommand SubCategoriesDisplayActionSheetCommand
        {
            get { return (ICommand)GetValue(SubCategoriesDisplayActionSheetCommandProperty); }
            set { SetValue(SubCategoriesDisplayActionSheetCommandProperty, value); }
        }

        public static readonly BindableProperty PushCategoryDetailViewCommandProperty = BindableProperty.Create(propertyName: nameof(PushCategoryDetailViewCommand),
                                                                                                                returnType: typeof(ICommand),
                                                                                                                declaringType: typeof(SubCategoryCard),
                                                                                                                defaultValue: null);

        public ICommand PushCategoryDetailViewCommand
        {
            get { return (ICommand)GetValue(PushCategoryDetailViewCommandProperty); }
            set { SetValue(PushCategoryDetailViewCommandProperty, value); }
        }

        #endregion Properties
    }
}
