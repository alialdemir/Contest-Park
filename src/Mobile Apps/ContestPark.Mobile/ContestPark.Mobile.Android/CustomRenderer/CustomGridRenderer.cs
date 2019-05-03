using Android.Content;
using Android.Views;
using ContestPark.Mobile.Components;
using ContestPark.Mobile.Droid.CustomRenderer;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomGrid), typeof(CustomGridRenderer))]

namespace ContestPark.Mobile.Droid.CustomRenderer
{
    public class CustomGridRenderer : ViewRenderer<CustomGrid, Android.Views.View>
    {
        private readonly GestureDetector _detector;
        private readonly FancyGestureListener _listener;

        public CustomGridRenderer(Context context) : base(context)

        {
            _listener = new FancyGestureListener();
            _detector = new GestureDetector(context, _listener);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<CustomGrid> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null)
            {
                var lbl = e.NewElement;

                _listener.LongPressed = lbl.LongPressed;
                _listener.SingleTap = lbl.SingleTap;
                _listener.CommandParameter = lbl.CommandParameter;
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

        public ICommand SingleTap { get; set; }

        public override bool OnDoubleTap(MotionEvent e)
        {
            return base.OnDoubleTap(e);
        }

        public override bool OnDoubleTapEvent(MotionEvent e)
        {
            return base.OnDoubleTapEvent(e);
        }

        public override bool OnDown(MotionEvent e)
        {
            return base.OnDown(e);
        }

        public override bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
        {
            return base.OnFling(e1, e2, velocityX, velocityY);
        }

        public override void OnLongPress(MotionEvent e)
        {
            LongPressed?.Execute(CommandParameter);
            base.OnLongPress(e);
        }

        public override bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
        {
            return base.OnScroll(e1, e2, distanceX, distanceY);
        }

        public override void OnShowPress(MotionEvent e)
        {
            base.OnShowPress(e);
        }

        public override bool OnSingleTapConfirmed(MotionEvent e)
        {
            return base.OnSingleTapConfirmed(e);
        }

        public override bool OnSingleTapUp(MotionEvent e)
        {
            SingleTap?.Execute(CommandParameter);
            return base.OnSingleTapUp(e);
        }
    }
}