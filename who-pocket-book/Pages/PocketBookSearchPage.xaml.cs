using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using who_pocket_book.Helpers;
using who_pocket_book.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace who_pocket_book.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PocketBookSearchPage : ContentPage
    {
        private readonly List<InternalContentSearchItem> allItems;
        private readonly object searchLock = new object();
        private readonly object padlock = new object();
        private readonly List<int> searchesList = new List<int>(20);
        private List<InternalContentSearchItem> items;
        private static string searchText = "";
        private long lastPush = 0;

        public PocketBookSearchPage()
        {
            InitializeComponent();
            allItems = DependencyService.Get<IJsonReaderHelper>().GetSearchPageItems().Select(x => new InternalContentSearchItem(x)).OrderBy(x => x.Title).ToList();
            GroupedView.ItemsSource = allItems;
            SearchBarView.Text = searchText;
        }

        private void Search()
        {
            Task searchTask = new Task(() =>
            {
                if (string.IsNullOrWhiteSpace(SearchBarView.Text))
                {
                    items = allItems;
                }
                else
                {
                    string searchText = SearchBarView.Text;
                    string[] searchArray = searchText.Split(' ', ',').Select(x => x.Trim().ToLowerInvariant()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                    items = allItems.Where(x => searchArray.All(s => x.SearchKeys.Contains(s))).ToList();
                }

                lock (searchLock)
                {
                    int index = Task.CurrentId.HasValue ? searchesList.IndexOf(Task.CurrentId.Value) : -1;
                    if (index >= 0)
                    {
                        searchesList.RemoveRange(0, index + 1);
                        Device.BeginInvokeOnMainThread(() => { GroupedView.ItemsSource = items; });
                    }
                }
            });
            lock (searchLock)
            {
                searchesList.Add(searchTask.Id);
            }
            searchTask.Start();
        }

        private void SearchBar_SearchButtonPressed(object sender, EventArgs e)
        {
            Search();
            searchText = SearchBarView.Text;
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            Search();
            searchText = SearchBarView.Text;
        }

        private async void GroupedView_ItemTapped(object sender, ItemTappedEventArgs e)
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
            InternalContentSearchItem searchItem = e.Item as InternalContentSearchItem;
            await Navigation.PopModalAsync();
            switch (searchItem.ChildElementType)
            {
                case ChildElementType.Content:
                    await (MainPage.Instance.CurrentPage as NavigationPage).PushAsync(new PocketBookContentPage(searchItem.Title, searchItem.ChildElement));
                    break;
                case ChildElementType.Chapter:
                    await (MainPage.Instance.CurrentPage as NavigationPage).PushAsync(new PocketBookPage(searchItem.Title, searchItem.ChildElement));
                    break;
                default:
                    break;
            }
        }
    }
}