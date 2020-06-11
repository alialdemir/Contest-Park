using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.Views
{
    public partial class AppShellHeaderView : ContentView
    {
        #region Constructor

        public AppShellHeaderView()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Properties

        public static readonly BindableProperty MenuItemClickCommandProperty = BindableProperty.Create(propertyName: nameof(MenuItemClickCommand),
                                                                                                       returnType: typeof(ICommand),
                                                                                                       declaringType: typeof(AppShellHeaderView),
                                                                                                       defaultValue: null);

        public ICommand MenuItemClickCommand
        {
            get { return (ICommand)GetValue(MenuItemClickCommandProperty); }
            set { SetValue(MenuItemClickCommandProperty, value); }
        }

        #endregion Properties
    }
}
