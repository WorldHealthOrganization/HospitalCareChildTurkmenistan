using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using who_pocket_book.iOS.Renderers;
using who_pocket_book.Views;
using Foundation;
using System.IO;
using System;
using UIKit;

[assembly: ExportRenderer(typeof(HybridWebView), typeof(HybridWebViewRenderer))]
namespace who_pocket_book.iOS.Renderers
{
    class HybridWebViewRenderer : ViewRenderer<HybridWebView, WKWebView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<HybridWebView> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    WKWebViewConfiguration config = new WKWebViewConfiguration();
                    WKWebView webView = new WKWebView(Frame, config)
                    {
                        NavigationDelegate = new UrlRedirectionWebViewDelegate(e.NewElement.Redirect, e.NewElement.Browser, e.NewElement.Mail)
                    };
                    webView.ScrollView.ContentInset = new UIEdgeInsets(0, 0, 80, 0);
                    SetNativeControl(webView);
                }
                string url = Path.Combine(NSBundle.MainBundle.BundlePath, "assets", Element.Uri);
                Control.LoadRequest(new NSUrlRequest(new NSUrl(url, false)));
            }
        }
    }

    public class UrlRedirectionWebViewDelegate : WKNavigationDelegate
    {
        private readonly Action<string> Redirect;
        private readonly Action<string> Browser;
        private readonly Action<string> Mail;
        private bool isFirstLoad = true;

        public UrlRedirectionWebViewDelegate(Action<string> redirect, Action<string> browser, Action<string> mail)
        {
            Redirect = redirect;
            Browser = browser;
            Mail = mail;
        }

        public override void DecidePolicy(WKWebView webView, WKNavigationAction navigationAction, Action<WKNavigationActionPolicy> decisionHandler)
        {
            if (isFirstLoad)
            {
                isFirstLoad = false;
                decisionHandler(WKNavigationActionPolicy.Allow);
                return;
            }
            NSUrl url = navigationAction.Request.Url;
            if (navigationAction.NavigationType == WKNavigationType.LinkActivated)
            {
                if (url.Scheme.StartsWith("http"))
                {
                    Browser.Invoke(url.AbsoluteString);
                }
                else if (url.Scheme.StartsWith("mailto"))
                {
                    Mail.Invoke(url.ResourceSpecifier);
                }
            }
            else if (navigationAction.NavigationType == WKNavigationType.Reload)
            {
                decisionHandler(WKNavigationActionPolicy.Allow);
                return;
            }
            else if (url.Path.Contains("who-pocket-book.iOS.app"))
            {
                Redirect.Invoke(url.LastPathComponent);
            }
            decisionHandler(WKNavigationActionPolicy.Cancel);
        }
    }
}