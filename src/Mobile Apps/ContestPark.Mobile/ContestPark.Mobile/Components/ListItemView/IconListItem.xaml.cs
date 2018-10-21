﻿using ContestPark.Mobile.Enums;
using Prism.Behaviors;
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

        /// <summary>
        /// Eğer icon boş gelirse icon componentinizi gizlier
        /// </summary>
        private void IconHide(Models.MenuItem.MenuItem menuItem)
        {
            if (string.IsNullOrEmpty(menuItem.Icon) && gridIconList.ColumnDefinitions.Count > 0)
            {
                gridIconList.ColumnDefinitions[0].Width = 0;
            }
        }

        /// <summary>
        /// Binding den gelen commandtı TapGestureRecognizer olarak döndürür
        /// </summary>
        /// <returns>Command to TapGestureRecognizer</returns>
        private TapGestureRecognizer GetTapGesture(Models.MenuItem.MenuItem menuItem)
        {
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) => PushPageCommand?.Execute(menuItem?.PageName);

            return tapGestureRecognizer;
        }

        #endregion Methods

        #region Override

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            Models.MenuItem.MenuItem menuItem = (Models.MenuItem.MenuItem)BindingContext;
            if (menuItem == null)
                return;

            IconHide(menuItem);

            RightIconVisibility(menuItem.MenuType);

            if (gridIconList.GestureRecognizers.Count == 0 && menuItem.MenuType != MenuTypes.Switch)
            {
                gridIconList.GestureRecognizers.Add(GetTapGesture(menuItem));
            }

            if (switchIcon.Behaviors.Count == 0)
            {
                switchIcon.Behaviors.Add(new EventToCommandBehavior()
                {
                    EventName = "Toggled",
                    Command = PushPageCommand,
                    CommandParameter = menuItem.PageName
                });
            }
        }

        #endregion Override
    }
}