using System;
using who_pocket_book.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace who_pocket_book.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PocketBookPage : ContentPage
    {
        private readonly HybridWebView webView;

        public PocketBookPage()
        {
            Title = (string)Application.Current.Resources["bookNavTitle"];
            NavigationPage.SetBackButtonTitle(this, (string)Application.Current.Resources["back"]);
            webView = new HybridWebView
            {
                Uri = "ind.html",
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            RelativeLayout layout = new RelativeLayout();
            layout.Children.Add(webView, Constraint.Constant(0), Constraint.Constant(0), Constraint.RelativeToParent((x) => x.Width), Constraint.RelativeToParent((x) => x.Height));
            ImageButton button = new ImageButton
            {
                Source = ImageSource.FromFile("notes.png"),
                CornerRadius = 32,
                BackgroundColor = (Color)Application.Current.Resources["colorPageTitle"],
                Padding = new Thickness(17, 13, 12, 13)
            };
            button.Clicked += ImageButton_Clicked;
            layout.Children.Add(button, Constraint.RelativeToParent((x) => x.Width - 80), Constraint.RelativeToParent((x) => x.Height - 75), Constraint.Constant(64), Constraint.Constant(64));
            Content = layout;
        }

        public PocketBookPage(string title, string htmlPath)
        {
            if (MainPage.Instance.Titles.TryGetValue(htmlPath, out Tuple<int, string> dicTitle))
            {
                Title = dicTitle.Item2;
            }
            else
            {
                Title = title;
            }
            NavigationPage.SetBackButtonTitle(this, (string)Application.Current.Resources["back"]);
            webView = new HybridWebView
            {
                Uri = htmlPath,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            RelativeLayout layout = new RelativeLayout();
            layout.Children.Add(webView, Constraint.Constant(0), Constraint.Constant(0), Constraint.RelativeToParent((x) => x.Width), Constraint.RelativeToParent((x) => x.Height));
            ImageButton button = new ImageButton
            {
                Source = ImageSource.FromFile("notes.png"),
                CornerRadius = 32,
                BackgroundColor = (Color)Application.Current.Resources["colorPageTitle"],
                Padding = new Thickness(18, 14, 13, 14)
            };
            button.Clicked += ImageButton_Clicked;
            layout.Children.Add(button, Constraint.RelativeToParent((x) => x.Width - 80), Constraint.RelativeToParent((x) => x.Height - 75), Constraint.Constant(64), Constraint.Constant(64));
            Content = layout;
        }

        protected override bool OnBackButtonPressed()
        {
            if (webView.CanGoBack())
            {
                webView.GoBack();
                return true;
            }
            else
            {
                return base.OnBackButtonPressed();
            }
        }

        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new PocketBookNotePage(Title));
        }
    }
}