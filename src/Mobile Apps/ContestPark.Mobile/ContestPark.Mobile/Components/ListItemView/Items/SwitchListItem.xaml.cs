using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Components
{
    /// <summary>
    /// Defines the <see cref="SwitchListItem"/>
    /// </summary>
    
    public partial class SwitchListItem : ContentView
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SwitchListItem"/> class.
        /// </summary>
        public SwitchListItem()
        {
            InitializeComponent();
        }

        #endregion Constructors

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            // TODO: eğer icon null değilse gride icon label ekle

            //Models.MenuItem.MenuItem menuItem = (Models.MenuItem.MenuItem)BindingContext;
            //if (menuItem == null)
            //    return;

            //bool isIconNull = string.IsNullOrEmpty(menuItem.Icon);
            //if (!isIconNull)
            //{


            //    //switchGrid.Children.Add()
            //}
        }
    }
}