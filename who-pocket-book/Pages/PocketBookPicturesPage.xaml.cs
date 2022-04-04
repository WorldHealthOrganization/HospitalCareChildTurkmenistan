using System.Collections.Generic;
using System.IO;
using System.Linq;
using who_pocket_book.Db;
using who_pocket_book.Helpers;
using who_pocket_book.Models;
using who_pocket_book.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace who_pocket_book.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PocketBookPicturesPage : ContentPage
    {
        private readonly Dictionary<string, int> sortMapper;
        private readonly object padlock = new object();
        private double _width;
        private double _height;
        private bool loading = false;
        private View _selectedGroup;

        public PocketBookPicturesPage()
        {
            InitializeComponent();
            _width = Width;
            _height = Height;
            PicturesDatabase.SubscribeForPictures(ReloadData);
            NotesDatabase.SubscribeForNotes(ReloadData);
            sortMapper = DependencyService.Get<IJsonReaderHelper>().GetTitles().Select(x => new { x.Value.Item2, x.Value.Item1 }).ToDictionary(x => x.Item2, x => x.Item1);
            sortMapper.Add((string)Application.Current.Resources["bookNavTitle"], 0);

            MessagingCenter.Subscribe<MainPage, string>(this, "ShowAttachedImages", (sender, title) => ShowSelectedGroup(title));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (_selectedGroup != null)
            {
                scroll.ScrollToAsync(_selectedGroup, ScrollToPosition.MakeVisible, true);
            }
        }

        protected override void OnDisappearing()
        {
            _selectedGroup = null;

            base.OnDisappearing();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            var oldWidth = _width;
            const double sizenotallocated = -1;

            base.OnSizeAllocated(width, height);
            if (Equals(_width, width) && Equals(_height, height))
            {
                return;
            }

            _width = width;
            _height = height;

            // ignore if the previous height was size unallocated
            if (Equals(oldWidth, sizenotallocated))
            {
                return;
            }
            if (!Equals(width, oldWidth))
            {
                ReloadData();
            }
        }

        private async void ReloadData()
        {
            lock (padlock)
            {
                if (loading)
                {
                    return;
                }
                loading = true;
            }
            List<string> expanded = mainStack.Children.Where(x => ((StackLayout)x).Children.Skip(1).FirstOrDefault()?.IsVisible ?? false).Select(x => ((Label)((StackLayout)((StackLayout)x).Children.FirstOrDefault()).Children.FirstOrDefault()).Text).ToList();
            mainStack.Children.Clear();
            List<UsersPicture> pictures = await PicturesDatabase.GetAllPictures();
            List<UsersNote> notes = await NotesDatabase.GetAllNotes();
            IEnumerable<IGrouping<string, UsersPicture>> groups = pictures.GroupBy(x => x.Category).OrderBy(x => sortMapper[x.Key]);
            List<CategoryDisplayInfo> categories = groups.Select(x => new CategoryDisplayInfo { Category = x.Key, Text = string.Empty, Pictures = x.ToList() }).ToList();
            foreach (UsersNote note in notes)
            {
                if (categories.FirstOrDefault(x => x.Category == note.Category) != null)
                {
                    categories.FirstOrDefault(x => x.Category == note.Category).Text = note.Text;
                }
                else
                {
                    categories.Add(new CategoryDisplayInfo { Category = note.Category, Text = note.Text, Pictures = new List<UsersPicture> { } });
                }
            }
            categories = categories.OrderBy(x => sortMapper[x.Category]).ToList();
            foreach (CategoryDisplayInfo category in categories)
            {
                mainStack.Children.Add(GetSectionStack(category, expanded.Contains(category.Category)));
            }
            lock (padlock)
            {
                loading = false;
            }
        }

        private CustomStackLayout GetSectionStack(CategoryDisplayInfo pictures, bool expanded = false)
        {
            CustomStackLayout result = new CustomStackLayout { };
            StackLayout titleStack = new StackLayout { BackgroundColor = (Color)Application.Current.Resources["colorPageTitle"], Orientation = StackOrientation.Horizontal, Padding = new Thickness(8) };
            titleStack.Children.Add(new Label
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                TextColor = Color.White,
                FontSize = 16,
                VerticalOptions = LayoutOptions.Center,
                Text = pictures.Category
            });
            Image expandImage = new Image
            {
                Source = ImageSource.FromFile(expanded ? "ic_expandedt.png" : "ic_expandedb.png")
            };
            if (Device.RuntimePlatform == Device.Android)
            {
                expandImage.HeightRequest = 35;
                expandImage.WidthRequest = 35;
                expandImage.VerticalOptions = LayoutOptions.Center;
            }
            titleStack.Children.Add(expandImage);
            result.Children.Add(titleStack);
            if (!string.IsNullOrWhiteSpace(pictures.Text))
            {
                StackLayout noteStack = new StackLayout { IsVisible = expanded, Orientation = StackOrientation.Horizontal, Padding = new Thickness(12) };
                noteStack.Children.Add(new Label
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    TextColor = Color.Black,
                    FontSize = 16,
                    VerticalOptions = LayoutOptions.Center,
                    Text = pictures.Text
                });
                noteStack.GestureRecognizers.Add(new TapGestureRecognizer()
                {
                    Command = new Command(async () =>
                    {
                        await Navigation.PushModalAsync(new PocketBookNotePage(pictures.Category));
                    })
                });
                result.Children.Add(noteStack);
                SetClickListener(noteStack, titleStack);
            }
            if (pictures.Pictures.Count() > 0)
            {
                StackLayout imagesLayout = new StackLayout { IsVisible = expanded, HorizontalOptions = LayoutOptions.Fill, Spacing = 6, Padding = new Thickness(16) };
                if (Width > 450)
                {
                    int columns = (int)(Width / 225);
                    double columnWidth = (Width - 32 + 6) / columns - 6;
                    int rows = (pictures.Pictures.Count() + columns - 1) / columns;
                    for (int i = 0; i < rows; i++)
                    {
                        StackLayout rowLayout = new StackLayout { Spacing = 6, Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.StartAndExpand };
                        foreach (UsersPicture picture in pictures.Pictures.Skip(i * columns).Take(columns))
                        {
                            MemoryStream stream = new MemoryStream(picture.Picture);
                            Image image = new Image
                            {
                                HorizontalOptions = LayoutOptions.FillAndExpand,
                                BackgroundColor = Color.LightGray,
                                HeightRequest = 300,
                                WidthRequest = columnWidth,
                                Aspect = Aspect.AspectFit,
                                Source = ImageSource.FromStream(() => stream)
                            };
                            image.GestureRecognizers.Add(new TapGestureRecognizer()
                            {
                                Command = new Command(async () =>
                                {
                                    if (Navigation.ModalStack.Count == 0)
                                    {
                                        await Navigation.PushModalAsync(new PocketBookPicturePage(picture));
                                    }
                                })
                            });
                            rowLayout.Children.Add(image);
                        }
                        imagesLayout.Children.Add(rowLayout);
                    }
                }
                else
                {
                    foreach (UsersPicture picture in pictures.Pictures)
                    {
                        MemoryStream stream = new MemoryStream(picture.Picture);
                        Image image = new Image
                        {
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            BackgroundColor = Color.LightGray,
                            HeightRequest = 300,
                            Aspect = Aspect.AspectFit,
                            Source = ImageSource.FromStream(() => stream)
                        };
                        image.GestureRecognizers.Add(new TapGestureRecognizer()
                        {
                            Command = new Command(async () =>
                            {
                                if (Navigation.ModalStack.Count == 0)
                                {
                                    await Navigation.PushModalAsync(new PocketBookPicturePage(picture));
                                }
                            })
                        });
                        imagesLayout.Children.Add(image);
                    }
                }
                result.Children.Add(imagesLayout);
                SetClickListener(imagesLayout, titleStack);
            }

            result.Title = pictures.Category;

            return result;
        }

        private void SetClickListener(StackLayout toShow, StackLayout image)
        {
            image.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() =>
                {
                    toShow.IsVisible = !toShow.IsVisible;
                    ((Image)image.Children.Skip(1).FirstOrDefault()).Source = ImageSource.FromFile(toShow.IsVisible ? "ic_expandedt.png" : "ic_expandedb.png");
                })
            });
        }

        private void ShowSelectedGroup(string title)
        {
            if (title != null)
            {
                if (mainStack.Children.FirstOrDefault(group => group is CustomStackLayout stack && stack.Title == title) is CustomStackLayout selectedGroup)
                {
                    var titleStack = selectedGroup.Children[0] as StackLayout;
                    var imageLayout = selectedGroup.Children[1] as StackLayout;
                    var expandIcon = titleStack.Children[1] as Image;

                    expandIcon.Source = ImageSource.FromFile("ic_expandedt.png");
                    imageLayout.IsVisible = true;
                    View st = selectedGroup.Children.Skip(2).FirstOrDefault();
                    if (st != null)
                    {
                        st.IsVisible = true;
                    }

                    _selectedGroup = selectedGroup;
                }
            }
        }
    }
}