using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.GestureRecognizer
{
    public class LongPressGestureRecognizer : Element, IGestureRecognizer
    {
        public static readonly BindableProperty CommandProperty = BindableProperty.Create(propertyName: nameof(Command),
                                                                                                       returnType: typeof(ICommand),
                                                                                                       declaringType: typeof(LongPressGestureRecognizer),
                                                                                                       defaultValue: null);

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set
            {
                SetValue(CommandProperty, value);
            }
        }

        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(propertyName: nameof(CommandParameter),
                                                                                                                returnType: typeof(object),
                                                                                                                declaringType: typeof(LongPressGestureRecognizer),
                                                                                                                defaultValue: null);

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set
            {
                SetValue(CommandParameterProperty, value);
            }
        }
    }
}
