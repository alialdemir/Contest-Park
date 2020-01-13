using ContestPark.Mobile.Models.Base;
using System.Collections.Generic;

namespace ContestPark.Mobile.Models.MenuItem
{
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
