using Prism.Navigation;

namespace ContestPark.Mobile.Models.PageNavigation
{
    public class PageNavigation
    {
        public PageNavigation(string pageName)
        {
            PageName = pageName;
        }

        public string PageName { get; set; }
        public INavigationParameters Parameters { get; set; }
    }
}