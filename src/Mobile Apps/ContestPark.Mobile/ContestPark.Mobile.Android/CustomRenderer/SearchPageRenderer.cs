using Android.Runtime;
using Android.Text;
using Android.Views.InputMethods;
using Android.Widget;
using ContestPark.Mobile.Components;
using ContestPark.Mobile.Droid.CustomRenderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using SearchView = Android.Support.V7.Widget.SearchView;

[assembly: ExportRenderer(typeof(SearchPage), typeof(SearchPageRenderer))]

namespace ContestPark.Mobile.Droid.CustomRenderer
{
    public class SearchPageRenderer : PageRenderer
    {
        private SearchView _searchView;

        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged(e);

            if (e?.NewElement == null || e.OldElement != null)
            {
                return;
            }

            AddSearchToToolBar();
        }

        protected override void Dispose(bool disposing)
        {
            if (_searchView != null)
            {
                _searchView.QueryTextChange += searchView_QueryTextChange;
                _searchView.QueryTextSubmit += searchView_QueryTextSubmit;
            }
            MainActivity.ToolBar?.Menu?.RemoveItem(Resource.Layout.SearchMenu);
            base.Dispose(disposing);
        }

        private void AddSearchToToolBar()
        {
            if (MainActivity.ToolBar == null || Element == null)
            {
                return;
            }

            MainActivity.ToolBar.Title = Element?.Title;
            MainActivity.ToolBar.InflateMenu(Resource.Layout.SearchMenu);

            _searchView = MainActivity.ToolBar.Menu?.FindItem(Resource.Id.action_search)?.ActionView?.JavaCast<SearchView>();

            if (_searchView == null)
            {
                return;
            }

            SearchPage searchPage = Element as SearchPage;

            _searchView.QueryTextChange += searchView_QueryTextChange;
            _searchView.QueryTextSubmit += searchView_QueryTextSubmit;
            _searchView.QueryHint = searchPage?.SearchPlaceHolderText;
            _searchView.ImeOptions = (int)ImeAction.Search;
            _searchView.InputType = (int)InputTypes.TextVariationNormal;
            _searchView.MaxWidth = int.MaxValue;        //Hack to go full width - http://stackoverflow.com/questions/31456102/searchview-doesnt-expand-full-width

            // Search text color
            SearchView.SearchAutoComplete theTextArea = (SearchView.SearchAutoComplete)_searchView.FindViewById(Resource.Id.search_src_text);
            theTextArea.SetTextColor(searchPage.SeachTextColor.ToAndroid());
            theTextArea.SetHintTextColor(searchPage.SearchPlaceHolderColor.ToAndroid());

            // Close button color
            ImageView searchClose = (ImageView)_searchView.FindViewById(Resource.Id.search_close_btn);
            searchClose.SetColorFilter(searchPage.SearchCloseIconColor.ToAndroid());

            ImageView search = (ImageView)_searchView.FindViewById(Resource.Id.search_button);
            search.SetColorFilter(searchPage.SearchIconColor.ToAndroid());
        }

        private void searchView_QueryTextSubmit(object sender, SearchView.QueryTextSubmitEventArgs e)
        {
            if (e == null)
            {
                return;
            }

            var searchPage = Element as SearchPage;
            if (searchPage == null)
            {
                return;
            }
            searchPage.SearchText = e.Query;
            searchPage.SearchCommand?.Execute(e.Query);
            e.Handled = true;
        }

        private void searchView_QueryTextChange(object sender, SearchView.QueryTextChangeEventArgs e)
        {
            var searchPage = Element as SearchPage;
            if (searchPage == null)
            {
                return;
            }
            searchPage.SearchText = e?.NewText;
        }
    }
}