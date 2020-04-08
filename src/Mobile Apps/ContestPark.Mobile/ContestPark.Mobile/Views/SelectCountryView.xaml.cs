using ContestPark.Mobile.Models.Country;
using ContestPark.Mobile.ViewModels;
using Prism.Navigation;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectCountryView : PopupPage
    {
        #region Constructor

        public SelectCountryView()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Methods

        private void ListView_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            CountryModel countryModel = e.SelectedItem is CountryModel
                ? (CountryModel)e.SelectedItem
                : null;

            ((SelectCountryViewModel)BindingContext).GoBackAsync(new NavigationParameters
           {
               { "SelectedCountry", countryModel }
           }, true);
        }

        protected override bool OnBackButtonPressed()
        {
            ((SelectCountryViewModel)BindingContext).GotoBackCommand.Execute(true);
            return true;
        }

        #endregion Methods
    }
}
