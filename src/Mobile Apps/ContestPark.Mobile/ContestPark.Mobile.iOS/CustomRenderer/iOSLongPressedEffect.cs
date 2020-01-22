using ContestPark.Mobile.Components.LongPressedEffect;
using ContestPark.Mobile.iOS.CustomRenderer;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName("ContestPark")]
[assembly: ExportEffect(typeof(iOSLongPressedEffect), "LongPressedEffect")]

namespace ContestPark.Mobile.iOS.CustomRenderer
{
    public class iOSLongPressedEffect : PlatformEffect
    {
        private bool _attached;
        private readonly UILongPressGestureRecognizer _longPressRecognizer;
        private readonly UITapGestureRecognizer _tapGestureRecognizer;

        public iOSLongPressedEffect()
        {
            _longPressRecognizer = new UILongPressGestureRecognizer(HandleLongClick);
            _tapGestureRecognizer = new UITapGestureRecognizer(HandleTapClick);
        }

        protected override void OnAttached()
        {
            //because an effect can be detached immediately after attached (happens in listview), only attach the handler one time
            if (!_attached)
            {
                Container.AddGestureRecognizer(_longPressRecognizer);
                Container.AddGestureRecognizer(_tapGestureRecognizer);
                _attached = true;
            }
        }

        // Invoke the command if there is one
        private void HandleLongClick(UILongPressGestureRecognizer sender)
        {
            if (sender.State == UIGestureRecognizerState.Began)
            {
                var command = LongPressedEffect.GetLongPressed(Element);
                command?.Execute(LongPressedEffect.GetCommandParameter(Element));
            }
        }

        private void HandleTapClick(UITapGestureRecognizer sender)
        {
            if (sender.State == UIGestureRecognizerState.Began)
            {
                var command = LongPressedEffect.GetSingleTap(Element);
                command?.Execute(LongPressedEffect.GetCommandParameter(Element));
            }
        }

        protected override void OnDetached()
        {
            if (_attached)
            {
                Container.RemoveGestureRecognizer(_longPressRecognizer);
                Container.RemoveGestureRecognizer(_tapGestureRecognizer);
                _attached = false;
            }
        }
    }
}
