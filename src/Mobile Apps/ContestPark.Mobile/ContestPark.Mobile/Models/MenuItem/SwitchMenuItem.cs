using System.Windows.Input;

namespace ContestPark.Mobile.Models.MenuItem
{
    public class SwitchMenuItem : MenuItem
    {
        private bool _isToggled;
        private string _title;
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

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged(() => Title);
            }
        }

        public ICommand SingleTap { get; set; }
    }
}