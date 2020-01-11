using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.GestureRecognizer
{
    public class LongPressGestureRecognizer : Element, IGestureRecognizer
    {
        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand),
                                                                                          typeof(LongPressGestureRecognizer),
                                                                                          (object)null,
                                                                                          BindingMode.OneWay,
                                                                                          (BindableProperty.ValidateValueDelegate)null,
                                                                                          (BindableProperty.BindingPropertyChangedDelegate)null,
                                                                                          (BindableProperty.BindingPropertyChangingDelegate)null,
                                                                                          (BindableProperty.CoerceValueDelegate)null,
                                                                                          (BindableProperty.CreateDefaultValueDelegate)null);

        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter),
                                                                                                   typeof(object),
                                                                                                   typeof(LongPressGestureRecognizer),
                                                                                                   (object)null,
                                                                                                   BindingMode.TwoWay,
                                                                                                   (BindableProperty.ValidateValueDelegate)null,
                                                                                                   (BindableProperty.BindingPropertyChangedDelegate)null,
                                                                                                   (BindableProperty.BindingPropertyChangingDelegate)null,
                                                                                                   (BindableProperty.CoerceValueDelegate)null,
                                                                                                   (BindableProperty.CreateDefaultValueDelegate)null);

        public ICommand Command
        {
            get { return (ICommand)this.GetValue(CommandProperty); }
            set { this.SetValue(CommandProperty, (object)value); }
        }

        public object CommandParameter
        {
            get { return this.GetValue(CommandParameterProperty); }
            set { this.SetValue(CommandParameterProperty, value); }
        }
    }
}
