namespace ContestPark.Mobile.Models.MenuItem
{
    public class InputMenuItem : MenuItem
    {
        private string _text;
        public bool IsPassword { get; set; }
        public string Placeholder { get; set; }

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                RaisePropertyChanged(() => Text);
            }
        }
    }
}