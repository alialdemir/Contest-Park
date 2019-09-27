using System.Windows.Input;

namespace ContestPark.Mobile.Models.MenuItem
{
    public class TextMenuItem : MenuItem
    {
        public object CommandParameter { get; set; }

        public ICommand SingleTap { get; set; }
    }
}
