using ContestPark.Mobile.GestureRecognizer;
using ContestPark.Mobile.iOS.CustomRenderer;
using System.Linq;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Layout<View>), typeof(LongPressGestureRecognizerRenderer))]

namespace ContestPark.Mobile.iOS.CustomRenderer
{
    public class LongPressGestureRecognizerRenderer : ViewRenderer<Layout<View>, UIView>
    {
        private UILongPressGestureRecognizer _longPressGestureRecognizer;

        protected override void OnElementChanged(ElementChangedEventArgs<Layout<View>> e)
        {
            _longPressGestureRecognizer = _longPressGestureRecognizer ??
                new UILongPressGestureRecognizer(() =>
                {
                    if (Element.GestureRecognizers.Where(x =>
                    {
                        return x.GetType() == typeof(LongPressGestureRecognizer);
                    }).FirstOrDefault() is LongPressGestureRecognizer gesture && gesture.Command != null)
                    {
                        gesture.Command?.Execute(gesture.CommandParameter);
                    }
                });

            if (_longPressGestureRecognizer != null)
            {
                if (e.NewElement == null)
                {
                    this.RemoveGestureRecognizer(_longPressGestureRecognizer);
                }
                else if (e.OldElement == null)
                {
                    this.AddGestureRecognizer(_longPressGestureRecognizer);
                }
            }
        }
    }
}
