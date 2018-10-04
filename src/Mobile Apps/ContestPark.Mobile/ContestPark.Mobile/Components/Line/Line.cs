using Xamarin.Forms;

namespace ContestPark.Mobile.Components
{
    public class Line : BoxView
    {
        public Line()
        {
            HeightRequest = 1;
            VerticalOptions = LayoutOptions.Center;
            HorizontalOptions = LayoutOptions.FillAndExpand;
            BackgroundColor = Color.FromHex("#333333");
        }
    }
}