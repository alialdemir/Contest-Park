using Android.Webkit;
using ContestPark.Mobile.Components;
using ContestPark.Mobile.Droid.CustomRenderer;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomWebView), typeof(CustomWebViewRenderer))]

namespace ContestPark.Mobile.Droid.CustomRenderer
{
    [Obsolete]
    public class CustomWebViewRenderer : WebViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.WebView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {
                Control.SetWebViewClient(new TitleWebViewClient(this));
            }
        }

        internal class TitleWebViewClient : WebViewClient
        {
            private readonly CustomWebViewRenderer _titleWebViewRenderer;

            internal TitleWebViewClient(CustomWebViewRenderer titleWebViewRenderer)
            {
                this._titleWebViewRenderer = titleWebViewRenderer;
            }

            public override void OnPageFinished(Android.Webkit.WebView view, string url)
            {
                base.OnPageFinished(view, url);
                if (_titleWebViewRenderer != null && _titleWebViewRenderer.Element != null && _titleWebViewRenderer.Element is IElementController)
                {
                    ((IElementController)_titleWebViewRenderer.Element).SetValueFromRenderer(CustomWebView.PageTitleProperty, view.Title);
                }
            }
        }
    }
}
