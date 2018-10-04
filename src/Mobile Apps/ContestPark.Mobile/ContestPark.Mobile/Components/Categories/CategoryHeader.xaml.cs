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

        #region Override

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (lblSeeAll.GestureRecognizers.Count == 0)
            {
                lblCategoryName.Text = CategoryName;

                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += (s, e) => SeeAllPressed?.Execute(SeeAllCommandParameter);
                lblSeeAll.GestureRecognizers.Add(tapGestureRecognizer);
            }
        }

        #endregion Override
    }
}