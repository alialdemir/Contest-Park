﻿using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Components
{
    /// <summary>
    /// Defines the <see cref="ListItemFactory"/>
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public class ListItemFactory : ContentView
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ListItemFactory"/> class.
        /// </summary>
        public ListItemFactory()
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether IsFirstTogle
        /// </summary>
        public bool IsFirstTogle { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// The OnBindingContextChanged
        /// </summary>
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            Models.MenuItem.MenuItem menuItem = (Models.MenuItem.MenuItem)BindingContext;
            if (menuItem == null)
                return;

            switch (menuItem.MenuType)
            {
                case Enums.MenuTypes.Icon:
                    Content = new IconListItem { BindingContext = this.BindingContext };
                    break;

                case Enums.MenuTypes.Switch:
                    Content = new SwitchListItem { BindingContext = this.BindingContext };
                    break;

                case Enums.MenuTypes.Input:
                    Content = new InputListItem { BindingContext = this.BindingContext };
                    break;

                default:
                    Content = new TextListItem { BindingContext = this.BindingContext };
                    break;
            }
        }

        #endregion Methods
    }
}