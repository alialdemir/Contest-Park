using System.Windows.Input;

namespace ContestPark.Mobile.Models.MenuItem
{
    public class TextMenuItem : MenuItem
    {
        private string _title;

        public object CommandParameter { get; set; }

        public ICommand SingleTap { get; set; }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged(() => Title);
            }
        }
    }
}