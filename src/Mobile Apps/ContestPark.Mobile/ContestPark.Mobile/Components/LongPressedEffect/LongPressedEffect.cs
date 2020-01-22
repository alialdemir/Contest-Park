using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.Components.LongPressedEffect
{
    public class LongPressedEffect : RoutingEffect
    {
        public LongPressedEffect() : base("ContestPark.LongPressedEffect")
        {
        }

        public static readonly BindableProperty LongPressedProperty = BindableProperty.CreateAttached("LongPressed",
                                                                                                  typeof(ICommand),
                                                                                                  typeof(LongPressedEffect),
                                                                                                  (object)null);

        public static ICommand GetLongPressed(BindableObject view)
        {
            return (ICommand)view.GetValue(LongPressedProperty);
        }

        public static void SetLongPressed(BindableObject view, ICommand value)
        {
            view.SetValue(LongPressedProperty, value);
        }

        public static readonly BindableProperty SingleTapProperty = BindableProperty.CreateAttached("SingleTap",
                                                                                                  typeof(ICommand),
                                                                                                  typeof(LongPressedEffect),
                                                                                                  (object)null);

        public static ICommand GetSingleTap(BindableObject view)
        {
            return (ICommand)view.GetValue(SingleTapProperty);
        }

        public static void SetSingleTap(BindableObject view, ICommand value)
        {
            view.SetValue(SingleTapProperty, value);
        }

        public static readonly BindableProperty CommandParameterProperty = BindableProperty.CreateAttached("CommandParameter",
                                                                                                           typeof(object),
                                                                                                           typeof(LongPressedEffect),
                                                                                                           (object)null);

        public static object GetCommandParameter(BindableObject view)
        {
            return view.GetValue(CommandParameterProperty);
        }

        public static void SetCommandParameter(BindableObject view, object value)
        {
            view.SetValue(CommandParameterProperty, value);
        }
    }
}
