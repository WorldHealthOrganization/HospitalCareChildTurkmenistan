using Android.Content;
using who_pocket_book.Android.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using who_pocket_book.Views;
using WebView = global::Android.Webkit.WebView;
using Android.Webkit;
using Android.Annotation;
using Android.Net;
using System;
using Uri = Android.Net.Uri;
using System.Linq;

[assembly: ExportRenderer(typeof(HybridWebView), typeof(HybridWebViewRenderer))]
namespace who_pocket_book.Android.Renderers
{
    class HybridWebViewRenderer : ViewRenderer<HybridWebView, WebView>
    {
        private Context _context;

        public HybridWebViewRenderer(Context context) : base(context)
        {
            _context = context;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<HybridWebView> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    WebView webView = new WebView(_context);
                    webView.Settings.JavaScriptEnabled = true;
                    webView.Settings.JavaScriptCanOpenWindowsAutomatically = true;
                    webView.Settings.DomStorageEnabled = true;
                    webView.Settings.DatabaseEnabled = true;
                    webView.SetWebViewClient(new CustomWebViewClient(_context, e.NewElement.Redirect, e.NewElement.Browser, e.NewElement.Mail));
                    Element.onGoBack = webView.CanGoBack;
                    Element.goBack = webView.GoBack;
                    SetNativeControl(webView);
                }
                Control.LoadUrl($"file:///android_asset/{Element.Uri}");
            }
        }
    }

    class CustomWebViewClient : WebViewClient
    {
        private readonly Action<string> Redirect;
        private readonly Action<string> Browser;
        private readonly Action<string> Mail;
        private Context _context;

        public CustomWebViewClient(Context context, Action<string> redirect, Action<string> browser, Action<string> mail)
        {
            _context = context;
            Redirect = redirect;
            Browser = browser;
            Mail = mail;
        }

        public override void OnPageFinished(WebView view, string url)
        {
            base.OnPageFinished(view, url);
            view.EvaluateJavascript("document.body.style.marginBottom=\"80px\";", null);
        }

        [System.Obsolete]
        public override bool ShouldOverrideUrlLoading(WebView view, string url)
        {
            return HandleUri(Uri.Parse(url), view);
        }

        [TargetApi(Value = 24)]
        public override bool ShouldOverrideUrlLoading(WebView view, IWebResourceRequest request)
        {
            return HandleUri(request.Url, view);
        }

        private bool HandleUri(Uri uri, WebView view)
        {
          

            if (uri.Scheme.StartsWith("http"))
            {
                Browser.Invoke(uri.ToString());
                return true;
            }

            if (uri.Scheme.StartsWith("mailto"))
            {
                Mail.Invoke(uri.SchemeSpecificPart);
                return true;
            }

            //if (uri.Path.Contains("/android_asset/settings.html"))
            //{
            //    return false;
            //}
            if (view.Url.Contains(uri.Path))
            {
                return false;
            }

            if (uri.Path.Contains("/android_asset"))
            {
                Redirect.Invoke(uri.LastPathSegment);
                return true;
            }

            return false;
        }
    }
}