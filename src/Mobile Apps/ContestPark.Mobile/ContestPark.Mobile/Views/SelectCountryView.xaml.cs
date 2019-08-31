using ContestPark.Mobile.Models.Country;
using ContestPark.Mobile.ViewModels;
using Rg.Plugins.Popup.Pages;
using System;
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

        #region Events

        public EventHandler<CountryModel> CountryEventHandler { get; set; }

        private void ListView_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is CountryModel countryModel)
            {
                CountryEventHandler.Invoke(countryModel, countryModel);
            }

           ((SelectCountryViewModel)BindingContext).CloseCommand.Execute(null);
        }

        #endregion Events
    }
}
