using ContestPark.Mobile.Components;
using ContestPark.Mobile.iOS.CustomRenderer;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomWebView), typeof(CustomWebViewRenderer))]

namespace ContestPark.Mobile.iOS.CustomRenderer
{
    public class CustomWebViewRenderer : WebViewRenderer
    {
        public CustomWebViewRenderer()
        {
            this.LoadFinished += (object sender, EventArgs e) =>
            {
                if (Element is CustomWebView)
                {
                    CustomWebView titleWebView = (CustomWebView)Element;
                    ((IElementController)Element).SetValueFromRenderer(CustomWebView.PageTitleProperty,
                        EvaluateJavascript("document.title"));
                }
            };
        }
    }
}
