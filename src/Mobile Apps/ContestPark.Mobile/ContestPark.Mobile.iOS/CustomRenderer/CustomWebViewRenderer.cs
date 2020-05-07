using ContestPark.Mobile.Components;
using ContestPark.Mobile.iOS.CustomRenderer;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomWebView), typeof(CustomWebViewRenderer))]

namespace ContestPark.Mobile.iOS.CustomRenderer
{
    [Obsolete]
    public class CustomWebViewRenderer : WebViewRenderer
    {
        public CustomWebViewRenderer()
        {
            this.LoadFinished += (object sender, EventArgs e) =>
            {
                if (Element != null)
                {
                    ((IElementController)Element).SetValueFromRenderer(CustomWebView.PageTitleProperty,
                        EvaluateJavascript("document.title"));
                }
            };
        }
    }
}
