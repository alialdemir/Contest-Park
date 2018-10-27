﻿using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Models.Base;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

namespace ContestPark.Mobile.Models.MenuItem
{
    public class MenuItem : INotifyPropertyChanged
    {
        private bool _isToggled;

        public event PropertyChangedEventHandler PropertyChanged;

        public object CommandParameter { get; set; }

        public string Icon { get; set; }

        public bool IsToggled
        {
            get { return _isToggled; }
            set
            {
                _isToggled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsToggled)));
            }
        }

        public MenuTypes MenuType { get; set; }

        public ICommand SingleTap { get; set; }

        public string Title { get; set; }
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