using System;
using System.Linq;
using who_pocket_book.Pages;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace who_pocket_book.Views
{
    public class HybridWebView : View
    {
        public Func<bool> onGoBack;
        public Action goBack;
        public readonly Action<string> Redirect = (x) =>
        {
            if (x.EndsWith(".json"))
            {
                (MainPage.Instance.Children.First() as NavigationPage)?.PushAsync(new PocketBookContentPage(x, x));
            }
            else
            {
                (MainPage.Instance.Children.First() as NavigationPage)?.PushAsync(new PocketBookPage(x, x));
            }
        };
        public readonly Action<string> Browser = (x) =>
        {
            _ = Xamarin.Essentials.Browser.OpenAsync(x);
        };
        public readonly Action<string> Mail = (x) =>
        {
            try
            {
                _ = Email.ComposeAsync(new EmailMessage { To = new System.Collections.Generic.List<string> { x }, Body = string.Empty });
            } catch (Exception) { }
        };

        public static readonly BindableProperty UriProperty = BindableProperty.Create(propertyName: "Uri", returnType: typeof(string), declaringType: typeof(HybridWebView), defaultValue: default(string));

        public string Uri
        {
            get { return (string)GetValue(UriProperty); }
            set { SetValue(UriProperty, value); }
        }

        public bool CanGoBack()
        {
            if (onGoBack != null)
            {
                return onGoBack();
            }

            return false;
        }

        public void GoBack()
        {
            goBack?.Invoke();
        }
    }
}