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
        private UILongPressGestureRecognizer longPressGestureRecognizer;
        private UITapGestureRecognizer tapGestureRecognizer;

        protected override void OnElementChanged(ElementChangedEventArgs<CustomGrid> e)
        {
            longPressGestureRecognizer = longPressGestureRecognizer ??
                new UILongPressGestureRecognizer(() =>
                {
                    Element.LongPressed?.Execute(Element.CommandParameter);
                });

            tapGestureRecognizer = tapGestureRecognizer ??
                new UITapGestureRecognizer(() =>
                {
                    Element.SingleTap?.Execute(Element.CommandParameter);
                });

            if (longPressGestureRecognizer != null && tapGestureRecognizer != null)
            {
                if (e.NewElement == null)
                {
                    this.RemoveGestureRecognizer(longPressGestureRecognizer);
                    this.RemoveGestureRecognizer(tapGestureRecognizer);
                }
                else if (e.OldElement == null)
                {
                    this.AddGestureRecognizer(longPressGestureRecognizer);
                    this.AddGestureRecognizer(tapGestureRecognizer);
                }
            }
        }
    }
}
