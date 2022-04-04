using Acr.UserDialogs;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using who_pocket_book.Db;
using who_pocket_book.Helpers;
using who_pocket_book.Models;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace who_pocket_book.Pages
{
    [DesignTimeVisible(false)]
    public partial class MainPage
    {
        private string _currentTitle;

        private IUserDialogs _dialogs;

        private readonly string PICK_FROM_GALLERY = (string)Application.Current.Resources["pickFromGallery"];

        private readonly string CAMERA_CAPTURE = (string)Application.Current.Resources["pickFromCamera"];

        private readonly string SHOW_ATTACHED_PHOTOS = (string)Application.Current.Resources["showPhotos"];

        private readonly string TOAST_MESSAGE = (string)Application.Current.Resources["photoAdded"];

        private static MainPage instance;

        public static MainPage Instance { get => instance; }

        public Dictionary<string, Tuple<int, string>> Titles { get; private set; }

        public MainPage(IUserDialogs dialogs)
        {
            InitializeComponent();
            instance = this;
            Titles = DependencyService.Get<IJsonReaderHelper>().GetTitles();
            if (Device.RuntimePlatform == Device.Android)
            {
                foreach (NavigationPage page in Children.Select(x => x as NavigationPage))
                {
                    page.BarBackgroundColor = Color.FromRgb(245, 245, 245);
                    page.BarTextColor = Color.Black;
                    BarBackgroundColor = Color.FromRgb(245, 245, 245);
                }
            }   
            _dialogs = dialogs;
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new PocketBookSearchPage());
        }

        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            _currentTitle = ContainerPage.CurrentPage.Title;

            string[] buttons = await PicturesDatabase.CategoryPicturesExist(_currentTitle) ? new string[] { PICK_FROM_GALLERY, CAMERA_CAPTURE, SHOW_ATTACHED_PHOTOS } : new string[] { PICK_FROM_GALLERY, CAMERA_CAPTURE };
            if (Device.RuntimePlatform == Device.iOS)
            {
                DependencyService.Get<IAlertSheet>().ShowAlert((string)Application.Current.Resources["addPhoto"], async (x) =>
                {
                    if (x == PICK_FROM_GALLERY)
                    {
                        await PickFromGallery();
                    }
                    else if (x == CAMERA_CAPTURE)
                    {
                        await CapturePhoto();
                    }
                    else if (x == SHOW_ATTACHED_PHOTOS)
                    {
                        GetPictures();
                    }
                }, buttons);
            }
            else
            {
                string x = await DisplayActionSheet((string)Application.Current.Resources["addPhoto"], null, null, buttons);
                if (x == PICK_FROM_GALLERY)
                {
                    await PickFromGallery();
                }
                else if (x == CAMERA_CAPTURE)
                {
                    await CapturePhoto();
                }
                else if (x == SHOW_ATTACHED_PHOTOS)
                {
                    GetPictures();
                }
            }
        }

        private async Task PickFromGallery()
        {
            try
            {
                var result = await MediaPicker.PickPhotoAsync();

                if (result != null)
                {
                    var stream = await result.OpenReadAsync();
                    byte[] data;

                    using (MemoryStream ms = new MemoryStream())
                    {
                        stream.CopyTo(ms);
                        data = ms.ToArray();
                    }

                    var picture = new UsersPicture(_currentTitle, data);

                    await PicturesDatabase.AddPicture(picture);

                    _dialogs.Alert(TOAST_MESSAGE, null, "Ок");
                }
            }
            catch (PermissionException e)
            {
                _dialogs.Alert((string)Application.Current.Resources["missingGalleryPermission"], null, "Ок");
            }
            catch (MediaPermissionException e)
            {
                _dialogs.Alert((string)Application.Current.Resources["missingGalleryPermission"], null, "Ок");
            }
        }

        private async Task CapturePhoto()
        {
            await CrossMedia.Current.Initialize();
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }
            MediaFile file = null;
            try
            {
                file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    SaveToAlbum = false
                });
            }
            catch (PermissionException e)
            {
                _dialogs.Alert((string)Application.Current.Resources["missingCameraPermission"], null, "Ок");
            }
            catch (MediaPermissionException e)
            {
                _dialogs.Alert((string)Application.Current.Resources["missingCameraPermission"], null, "Ок");
            }
            if (file != null)
            {
                var stream = file.GetStream();
                byte[] data;

                using (MemoryStream ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    data = ms.ToArray();
                }

                var picture = new UsersPicture(_currentTitle, data);

                await PicturesDatabase.AddPicture(picture);

                _dialogs.Alert(TOAST_MESSAGE, null, "Ок");
            }
        }

        private void GetPictures()
        {
            var masterPage = ((NavigationPage)Parent).RootPage as TabbedPage;

            masterPage.CurrentPage = masterPage.Children[3];

            MessagingCenter.Send(this, "ShowAttachedImages", _currentTitle);
        }
    }
}