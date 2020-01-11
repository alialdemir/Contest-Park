using Android.Content;
using Android.Views;
using ContestPark.Mobile.Droid.CustomRenderer;
using ContestPark.Mobile.GestureRecognizer;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Layout<Xamarin.Forms.View>), typeof(LongPressGestureRecognizerRenderer))]

namespace ContestPark.Mobile.Droid.CustomRenderer
{
    public class LongPressGestureRecognizerRenderer : ViewRenderer<Layout<Xamarin.Forms.View>, Android.Views.View>
    {
        private readonly GestureDetector _detector;
        private readonly FancyGestureListener _listener;

        public LongPressGestureRecognizerRenderer(Context context) : base(context)

        {
            _listener = new FancyGestureListener();
            _detector = new GestureDetector(context, _listener);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Layout<Xamarin.Forms.View>> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null)
            {
                var lbl = e.NewElement;

                if (lbl.GestureRecognizers.Where(x =>
                {
                    return x.GetType() == typeof(LongPressGestureRecognizer);
                }).FirstOrDefault() is LongPressGestureRecognizer gesture && gesture.Command != null)
                {
                    _listener.LongPressed = gesture.Command;

                    _listener.CommandParameter = gesture.CommandParameter;
                }
            }

            if (e.NewElement == null)
            {
                this.GenericMotion -= HandleGenericMotion;
                this.Touch -= HandleTouch;
            }

            if (e.OldElement == null)
            {
                this.GenericMotion += HandleGenericMotion;
                this.Touch += HandleTouch;
            }
        }

        private void HandleGenericMotion(object sender, GenericMotionEventArgs e)
        {
            _detector.OnTouchEvent(e.Event);
        }

        private void HandleTouch(object sender, TouchEventArgs e)
        {
            _detector.OnTouchEvent(e.Event);
        }
    }
}

namespace ContestPark.Mobile.Droid.CustomRenderer
{
    public class FancyGestureListener : GestureDetector.SimpleOnGestureListener
    {
        public object CommandParameter { get; set; }
        public ICommand LongPressed { get; set; }

        public override void OnLongPress(MotionEvent e)
        {
            LongPressed?.Execute(CommandParameter);

            base.OnLongPress(e);
        }
    }
}
