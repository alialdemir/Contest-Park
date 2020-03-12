using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Components
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RightThumListItem : ContentView
    {
        #region Constructor

        public RightThumListItem()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Properties

        public static readonly BindableProperty GotoProfilePageCommandProperty = BindableProperty.Create(propertyName: nameof(GotoProfilePageCommand),
                                                                                                        returnType: typeof(ICommand),
                                                                                                        declaringType: typeof(RightThumListItem),
                                                                                                        defaultValue: null);

        public ICommand GotoProfilePageCommand
        {
            get { return (ICommand)GetValue(GotoProfilePageCommandProperty); }
            set
            {
                SetValue(GotoProfilePageCommandProperty, value);
            }
        }

        #endregion Properties
    }
}
