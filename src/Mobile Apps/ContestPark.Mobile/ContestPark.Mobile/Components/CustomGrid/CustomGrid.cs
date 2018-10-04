using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Components
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public class CustomGrid : Grid
    {
        #region Properties

        public static readonly BindableProperty LongPressedProperty = BindableProperty.Create(propertyName: nameof(LongPressed),
                                                                                                     returnType: typeof(ICommand),
                                                                                                     declaringType: typeof(CustomGrid),
                                                                                                     defaultValue: null);

        public ICommand LongPressed
        {
            get { return (ICommand)GetValue(LongPressedProperty); }
            set
            {
                SetValue(LongPressedProperty, value);
            }
        }

        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(propertyName: nameof(CommandParameter),
                                                                                                           returnType: typeof(object),
                                                                                                           declaringType: typeof(CustomGrid),
                                                                                                           defaultValue: null);

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set
            {
                SetValue(CommandParameterProperty, value);
            }
        }

        public static readonly BindableProperty SingleTapProperty = BindableProperty.Create(propertyName: nameof(SingleTap),
                                                                                                  returnType: typeof(ICommand),
                                                                                                  declaringType: typeof(CustomGrid),
                                                                                                  defaultValue: null);

        public ICommand SingleTap
        {
            get { return (ICommand)GetValue(SingleTapProperty); }
            set
            {
                SetValue(SingleTapProperty, value);
            }
        }

        #endregion Properties
    }
}