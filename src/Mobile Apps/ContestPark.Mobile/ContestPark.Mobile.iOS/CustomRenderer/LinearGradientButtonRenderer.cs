using ContestPark.Mobile.Components;
using ContestPark.Mobile.iOS.CustomRenderer;
using CoreAnimation;
using CoreGraphics;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(LinearGradientButton), typeof(LinearGradientButtonRenderer))]

namespace ContestPark.Mobile.iOS.CustomRenderer
{
    public class LinearGradientButtonRenderer : ButtonRenderer
    {
        public override void Draw(CGRect rect)
        {
            base.Draw(rect);
            LinearGradientButton stack = (LinearGradientButton)this.Element;
            CGColor startColor = stack.StartColor.ToCGColor();
            CGColor endColor = stack.EndColor.ToCGColor();

            #region for Vertical Gradient

            //var gradientLayer = new CAGradientLayer();

            #endregion for Vertical Gradient

            #region for Horizontal Gradient

            var gradientLayer = new CAGradientLayer()
            {
                CornerRadius = stack.CornerRadius,

                StartPoint = new CGPoint(0, 0.5),
                EndPoint = new CGPoint(1, 0.5)
            };

            #endregion for Horizontal Gradient

            gradientLayer.Frame = rect;
            gradientLayer.Colors = new CGColor[] {
                startColor,
                endColor
            };

            NativeView.Layer.InsertSublayer(gradientLayer, 0);
        }
    }
}
