using ContestPark.Mobile.Components;
using ContestPark.Mobile.iOS.CustomRenderer;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomGrid), typeof(CustomGridRenderer))]

namespace ContestPark.Mobile.iOS.CustomRenderer
{
    public class CustomGridRenderer : ViewRenderer<CustomGrid, UIView>
    {
        private UILongPressGestureRecognizer _longPressGestureRecognizer;
        private UITapGestureRecognizer _tapGestureRecognizer;

        protected override void OnElementChanged(ElementChangedEventArgs<CustomGrid> e)
        {
            _longPressGestureRecognizer = _longPressGestureRecognizer ??
                new UILongPressGestureRecognizer(() =>
                {
                    Element.LongPressed?.Execute(Element.CommandParameter);
                });

            _tapGestureRecognizer = _tapGestureRecognizer ??
                new UITapGestureRecognizer(() =>
                {
                    Element.SingleTap?.Execute(Element.CommandParameter);
                });

            if (_longPressGestureRecognizer != null && _tapGestureRecognizer != null)
            {
                if (e.NewElement == null)
                {
                    this.RemoveGestureRecognizer(_longPressGestureRecognizer);
                    this.RemoveGestureRecognizer(_tapGestureRecognizer);
                }
                else if (e.OldElement == null)
                {
                    this.AddGestureRecognizer(_longPressGestureRecognizer);
                    this.AddGestureRecognizer(_tapGestureRecognizer);
                }
            }
        }
    }
}
