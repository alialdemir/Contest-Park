using ContestPark.Mobile.Enums;
using ContestPark.Mobile.ViewModels.Base;

namespace ContestPark.Mobile.Models.MenuItem
{
    public class MenuItem : ExtendedBindableObject
    {
        public string Icon { get; set; }

        public MenuTypes MenuType { get; set; }
    }
}