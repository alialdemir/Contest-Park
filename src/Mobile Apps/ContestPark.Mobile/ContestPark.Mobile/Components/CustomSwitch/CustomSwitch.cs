using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace ContestPark.Mobile.Components.CustomSwitch
{
    public class CustomSwitch : Switch
    {
        public static readonly BindableProperty SwitchThumbImageActiveProperty =
           BindableProperty.Create(
               nameof(SwitchThumbImageActive),
               typeof(string),
               typeof(CustomSwitch),
               "switchactive.png");

        // Gets or sets BorderColor value
        public string SwitchThumbImageActive
        {
            get { return (string)GetValue(SwitchThumbImageActiveProperty); }
            set { SetValue(SwitchThumbImageActiveProperty, value); }
        }

        public static readonly BindableProperty SwitchThumbImagePassiveProperty =
           BindableProperty.Create(
               nameof(SwitchThumbImagePassive),
               typeof(string),
               typeof(CustomSwitch),
               "switchpassive.png");

        // Gets or sets BorderColor value
        public string SwitchThumbImagePassive
        {
            get { return (string)GetValue(SwitchThumbImagePassiveProperty); }
            set { SetValue(SwitchThumbImagePassiveProperty, value); }
        }
    }
}
