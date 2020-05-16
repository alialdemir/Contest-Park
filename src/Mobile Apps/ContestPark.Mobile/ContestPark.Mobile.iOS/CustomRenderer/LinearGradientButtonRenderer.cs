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
            LinearGradientButton button = (LinearGradientButton)this.Element;
            CGColor startColor = button.StartColor.ToCGColor();
            CGColor endColor = button.EndColor.ToCGColor();

            var gradientLayer = new CAGradientLayer()
            {
                CornerRadius = button.CornerRadius,

                StartPoint = new CGPoint(0, 0.5),
                EndPoint = new CGPoint(1, 0.5)
            };

            if (button.CornerRadiusPosition == LinearGradientButton.CornerRadiusPositions.Bottom)
            {
                gradientLayer.MaskedCorners = CACornerMask.MaxXMaxYCorner | CACornerMask.MinXMaxYCorner;
            }
            else if (button.CornerRadiusPosition == LinearGradientButton.CornerRadiusPositions.Top)
            {
                gradientLayer.MaskedCorners = CACornerMask.MaxXMinYCorner | CACornerMask.MinXMinYCorner;
            }

            gradientLayer.Frame = rect;
            gradientLayer.Colors = new CGColor[] {
                startColor,
                endColor
            };

            if (button.IsUpperCase)
            {
                this.Element.Text = button.Text.ToUpper();
            }

            NativeView.Layer.InsertSublayer(gradientLayer, 0);
        }
    }
}
