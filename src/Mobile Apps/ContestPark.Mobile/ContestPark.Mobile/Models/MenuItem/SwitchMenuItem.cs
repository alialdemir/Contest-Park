using System.Windows.Input;

namespace ContestPark.Mobile.Models.MenuItem
{
    public class SwitchMenuItem : MenuItem
    {
        private bool _isToggled;
        public object CommandParameter { get; set; }

        public bool IsToggled
        {
            get { return _isToggled; }
            set
            {
                _isToggled = value;
                RaisePropertyChanged(() => IsToggled);
            }
        }

        public ICommand SingleTap { get; set; }
    }
}
