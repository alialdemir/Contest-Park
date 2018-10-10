using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Models.Base;
using System.Collections.Generic;
using System.ComponentModel;

namespace ContestPark.Mobile.Models.MenuItem
{
    public class MenuItem : INotifyPropertyChanged
    {
        public string Title { get; set; }

        public string Icon { get; set; }

        public string PageName { get; set; }

        public MenuTypes MenuType { get; set; }

        private bool _isToggled;

        public bool IsToggled
        {
            get { return _isToggled; }
            set
            {
                _isToggled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsToggled)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
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