using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Components
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CategoryHeader : ContentView
    {
        #region Constructor

        public CategoryHeader()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Property

        public static readonly BindableProperty CategoryNameProperty = BindableProperty.Create(propertyName: nameof(CategoryName),
                                                                                                returnType: typeof(string),
                                                                                                declaringType: typeof(string),
                                                                                                defaultValue: string.Empty);

        public string CategoryName
        {
            get { return (string)GetValue(CategoryNameProperty); }
            set
            { SetValue(CategoryNameProperty, value); }
        }

        public static readonly BindableProperty SeeAllPressedProperty = BindableProperty.Create(propertyName: nameof(SeeAllPressed),
                                                                                                returnType: typeof(ICommand),
                                                                                                declaringType: typeof(ICommand),
                                                                                                defaultValue: null);

        public ICommand SeeAllPressed
        {
            get { return (ICommand)GetValue(SeeAllPressedProperty); }
            set { SetValue(SeeAllPressedProperty, value); }
        }

        public static readonly BindableProperty SeeAllCommandParameterProperty = BindableProperty.Create(propertyName: nameof(SeeAllCommandParameter),
                                                                                                returnType: typeof(object),
                                                                                                declaringType: typeof(object),
                                                                                                defaultValue: null);

        public object SeeAllCommandParameter
        {
            get { return (object)GetValue(SeeAllCommandParameterProperty); }
            set { SetValue(SeeAllCommandParameterProperty, value); }
        }

        #endregion Property

    }
}