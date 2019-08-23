using ContestPark.Mobile.Enums;
using ContestPark.Mobile.ViewModels.Base;
using Xamarin.Forms;

namespace ContestPark.Mobile.Models.MenuItem
{
    public class MenuItem : ExtendedBindableObject
    {
        public string Icon { get; set; }

        public MenuTypes MenuType { get; set; }

        public CornerRadius CornerRadius { get; set; } = new CornerRadius(0, 0, 0, 0);
    }
}
