using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.Helpers
{
    public partial class CommandAsync : Command
    {
        public CommandAsync(Func<Task> execute) : base(() => execute())
        {
        }

        public CommandAsync(Func<object, Task> execute) : base(() => execute(null))
        {
        }
    }

    public class CommandAsync<T> : ICommand
    {
        private readonly Func<T, Task> _execute;

        public CommandAsync(Func<T, Task> execute)
        {
            _execute = execute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _execute != null;
        }

        public void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                _execute.Invoke(parameter is T
                                ? (T)parameter
                                : default);

                if (CanExecuteChanged != null)
                {
                    CanExecuteChanged.Invoke(parameter is T
                                ? (T)parameter
                                : default, null);
                }
            }
        }
    }
}
