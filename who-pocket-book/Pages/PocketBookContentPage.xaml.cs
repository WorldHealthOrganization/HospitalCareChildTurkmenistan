using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using who_pocket_book.Helpers;
using who_pocket_book.Models;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.Xaml;

namespace who_pocket_book.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PocketBookContentPage : ContentPage
    {
        private readonly List<List<ContentPageItem>> items;
        private readonly object padlock = new object();
        private long lastPush = 0;

        public PocketBookContentPage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, (string)Application.Current.Resources["back"]);
            Title = (string)Application.Current.Resources["bookNavTitle"];
            List<List<ContentPageItem>> temp = DependencyService.Get<IJsonReaderHelper>().GetContentPageItems("main_content.json");
            items = temp.Where(x => x.FirstOrDefault(y => y.ChildElementType == ChildElementType.Html || y.ChildElementType == ChildElementType.Link || y.ChildElementType == ChildElementType.Image || y.ChildElementType == ChildElementType.Url) == null).ToList();
            GroupedView.ItemsSource = items;
        }

        public PocketBookContentPage(string title, string jsonPath)
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, (string)Application.Current.Resources["back"]);
            if (MainPage.Instance.Titles.TryGetValue(jsonPath, out Tuple<int, string> dicTitle))
            {
                Title = dicTitle.Item2;
            }
            else
            {
                Title = title;
            }
            List<List<ContentPageItem>> temp = DependencyService.Get<IJsonReaderHelper>().GetContentPageItems(jsonPath);
            items = temp.Where(x => x.FirstOrDefault(y => y.ChildElementType == ChildElementType.Html || y.ChildElementType == ChildElementType.Link || y.ChildElementType == ChildElementType.Image || y.ChildElementType == ChildElementType.Url) == null).ToList();
            temp = temp.Where(x => x.FirstOrDefault(y => y.ChildElementType == ChildElementType.Html || y.ChildElementType == ChildElementType.Link || y.ChildElementType == ChildElementType.Image || y.ChildElementType == ChildElementType.Url) != null).ToList();
            if (temp.Count == 1)
            {
                StackLayout stack = new StackLayout
                {
                    Padding = new Thickness(10, 0, 10, 70)
                };
                Label label = null;
                foreach (ContentPageItem item in temp.First())
                {
                    if (item.ChildElementType == ChildElementType.Html || item.ChildElementType == ChildElementType.Bold)
                    {
                        if (label == null)
                        {
                            label = new Label
                            {
                                FormattedText = new FormattedString()
                            };
                        }
                        if (Device.RuntimePlatform == Device.iOS)
                        {
                            label.FormattedText.Spans.Add(new Span() { Text = item.Title, FontAttributes = ((item.ChildElementType == ChildElementType.Bold) ? FontAttributes.Bold : FontAttributes.None) });
                        }
                        else if (Device.RuntimePlatform == Device.Android)
                        {
                            label.FormattedText.Spans.Add(new Span() { Text = item.Title, FontAttributes = ((item.ChildElementType == ChildElementType.Bold) ? FontAttributes.Bold : FontAttributes.None), TextColor = Color.FromHex("#292929"), LineHeight = 1.07 });
                        }
                    }
                    if (item.ChildElementType == ChildElementType.Link || item.ChildElementType == ChildElementType.Url)
                    {
                        if (label == null)
                        {
                            label = new Label
                            {
                                FormattedText = new FormattedString()
                            };
                        }
                        Span newSpan;
                        if (Device.RuntimePlatform == Device.iOS)
                        {
                            newSpan = new Span() { Text = item.Title, TextColor = Color.Blue, TextDecorations = TextDecorations.Underline };
                        }
                        else if (Device.RuntimePlatform == Device.Android)
                        {
                            newSpan = new Span() { Text = item.Title, TextColor = (Color)Application.Current.Resources["colorPageTitle"], TextDecorations = TextDecorations.Underline, LineHeight = 1.07 };
                        }
                        else
                        {
                            newSpan = null;
                        }
                        newSpan.GestureRecognizers.Add(new TapGestureRecognizer
                        {
                            Command = new Command(async () =>
                            {
                                if (Device.RuntimePlatform == Device.Android)
                                {
                                    lock (padlock)
                                    {
                                        if (lastPush != 0 && lastPush + 700 * TimeSpan.TicksPerMillisecond > DateTime.UtcNow.Ticks)
                                        {
                                            return;
                                        }
                                        lastPush = DateTime.UtcNow.Ticks;
                                    }
                                }
                                if (item.ChildElementType == ChildElementType.Link)
                                {
                                    if (item.ChildElement.EndsWith(".json"))
                                    {
                                        await Navigation.PushAsync(new PocketBookContentPage(item.Title, item.ChildElement));
                                    }
                                    else
                                    {
                                        await Navigation.PushAsync(new PocketBookPage(item.Title, item.ChildElement));
                                    }
                                }
                                else
                                {
                                    await Browser.OpenAsync(item.ChildElement);
                                }
                            })
                        });
                        label.FormattedText.Spans.Add(newSpan);
                    }
                    if (item.ChildElementType == ChildElementType.Image)
                    {
                        if (label != null)
                        {
                            stack.Children.Add(label);
                            label = null;
                        }
                        Image image = new Image
                        {
                            Aspect = Aspect.AspectFit,
                            HeightRequest = 200
                        };
                        if (Device.RuntimePlatform == Device.iOS)
                        {
                            image.Source = "assets/" + item.ChildElement;
                        }
                        else if (Device.RuntimePlatform == Device.Android)
                        {
                            image.Source = ImageSource.FromStream(() => DependencyService.Get<IAssetPath>().GetFile(item.ChildElement));
                        }
                        image.GestureRecognizers.Add(new TapGestureRecognizer
                        {
                            Command = new Command(async () =>
                            {
                                if (Device.RuntimePlatform == Device.Android)
                                {
                                    lock (padlock)
                                    {
                                        if (lastPush != 0 && lastPush + 700 * TimeSpan.TicksPerMillisecond > DateTime.UtcNow.Ticks)
                                        {
                                            return;
                                        }
                                        lastPush = DateTime.UtcNow.Ticks;
                                    }
                                }
                                await Navigation.PushModalAsync(new PocketBookImagePage(item.Title, Device.RuntimePlatform == Device.iOS ? "assets/" + item.ChildElement : ImageSource.FromStream(() => DependencyService.Get<IAssetPath>().GetFile(item.ChildElement))));
                            })
                        });
                        stack.Children.Add(image);
                    }
                }
                if (label != null)
                {
                    stack.Padding = new Thickness(10, 0, 10, 85);
                    stack.Children.Add(label);
                    label = null;
                }
                GroupedView.Footer = stack;
            }
            GroupedView.ItemsSource = items;
        }

        private async void GroupedView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is ContentPageItem)
            {
                if (Device.RuntimePlatform == Device.Android)
                {
                    lock (padlock)
                    {
                        if (lastPush != 0 && lastPush + 700 * TimeSpan.TicksPerMillisecond > DateTime.UtcNow.Ticks)
                        {
                            return;
                        }
                        lastPush = DateTime.UtcNow.Ticks;
                    }
                }
                ContentPageItem selected = e.Item as ContentPageItem;
                switch (selected.ChildElementType)
                {
                    case ChildElementType.Content:
                        await Navigation.PushAsync(new PocketBookContentPage(selected.Title, selected.ChildElement));
                        break;
                    case ChildElementType.Chapter:
                        await Navigation.PushAsync(new PocketBookPage(selected.Title, selected.ChildElement));
                        break;
                    default:
                        break;
                }
            }
        }

        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new PocketBookNotePage(Title));
        }
    }
}