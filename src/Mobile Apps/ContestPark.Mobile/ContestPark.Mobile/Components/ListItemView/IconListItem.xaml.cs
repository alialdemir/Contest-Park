using ContestPark.Mobile.Enums;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Components
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IconListItem : ContentView
    {
        #region Constructor

        public IconListItem()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Properties

        public bool IsFirstTogle { get; set; }

        public static readonly BindableProperty PushPageCommandProperty = BindableProperty.Create(propertyName: nameof(PushPageCommand),
                                                                                                    returnType: typeof(ICommand),
                                                                                                    declaringType: typeof(IconListItem),
                                                                                                    defaultValue: null);

        public ICommand PushPageCommand
        {
            get { return (ICommand)GetValue(PushPageCommandProperty); }
            set
            {
                SetValue(PushPageCommandProperty, value);
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Menu tipine göre sağ tarafdaki componentleri gösterir
        /// </summary>
        /// <param name="menuType">Gösterilmen istenen component tipi</param>
        private void RightIconVisibility(MenuTypes menuType)
        {
            switch (menuType)
            {
                case MenuTypes.Icon:
                    imgIcon.IsVisible = true;
                    break;

                case MenuTypes.Switch:
                    switchIcon.IsVisible = true;
                    break;

                default:
                    return;
            }
        }

        #endregion Methods

        #region Override

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            Models.MenuItem.MenuItem menuItem = (Models.MenuItem.MenuItem)BindingContext;
            if (menuItem == null)
                return;

            RightIconVisibility(menuItem.MenuType);

            if (gridIconList.GestureRecognizers.Count == 0 && menuItem.MenuType != MenuTypes.Switch)
            {
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += (s, e) => PushPageCommand?.Execute(menuItem?.PageName);

                gridIconList.GestureRecognizers.Add(tapGestureRecognizer);
            }
        }

        #endregion Override

        private void switchIcon_Toggled(object sender, ToggledEventArgs e)
        {
            Models.MenuItem.MenuItem menuItem = (Models.MenuItem.MenuItem)BindingContext;
            if (menuItem == null)
                return;

            if (IsFirstTogle)
            {
                PushPageCommand?.Execute(menuItem?.PageName);
            }

            IsFirstTogle = true;
        }
    }
}