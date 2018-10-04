using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Models.Base;
using System.Collections.Generic;

namespace ContestPark.Mobile.Models.MenuItem
{
    public class MenuItem
    {
        public string Title { get; set; }

        public string Icon { get; set; }

        public string PageName { get; set; }

        public MenuTypes MenuType { get; set; }

        public bool IsToggled { get; set; }
    }

    public class MenuItemList : List<MenuItem>, IModelBase
    {
        public MenuItemList()
        {
        }

        public MenuItemList(string heading)
        {
            Heading = heading;
        }

        public string Heading { get; set; }

        public List<MenuItem> MenuItems => this;
    }
}