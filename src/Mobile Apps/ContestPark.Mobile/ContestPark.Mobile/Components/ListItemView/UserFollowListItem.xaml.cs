using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Components
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserFollowListItem : ContentView
    {
        #region Constructor

        public UserFollowListItem()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Properties

        public static readonly BindableProperty GotoProfilePageCommandProperty = BindableProperty.Create(propertyName: nameof(GotoProfilePageCommand),
                                                                                                    returnType: typeof(ICommand),
                                                                                                    declaringType: typeof(UserFollowListItem),
                                                                                                    defaultValue: null);

        public static readonly BindableProperty IconStatusProperty = BindableProperty.Create(propertyName: nameof(IconStatus),
                                                                                                           returnType: typeof(bool),
                                                                                                   declaringType: typeof(UserFollowListItem),
                                                                                                   defaultValue: false);

        public static readonly BindableProperty RightButtonCommandProperty = BindableProperty.Create(propertyName: nameof(RightButtonCommand),
                                                                                                              returnType: typeof(ICommand),
                                                                                                      declaringType: typeof(UserFollowListItem),
                                                                                                      defaultValue: null);

        public ICommand GotoProfilePageCommand
        {
            get { return (ICommand)GetValue(GotoProfilePageCommandProperty); }
            set
            {
                SetValue(GotoProfilePageCommandProperty, value);
            }
        }

        public bool IconStatus
        {
            get { return (bool)GetValue(IconStatusProperty); }
            set { SetValue(IconStatusProperty, value); }
        }

        public ICommand RightButtonCommand
        {
            get { return (ICommand)GetValue(RightButtonCommandProperty); }
            set
            {
                SetValue(RightButtonCommandProperty, value);
            }
        }

        #endregion Properties
    }
}