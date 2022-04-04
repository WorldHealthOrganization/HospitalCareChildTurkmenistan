using Acr.UserDialogs;
using MongoDB.Bson;
using System;
using System.IO;
using who_pocket_book.Db;
using who_pocket_book.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace who_pocket_book.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PocketBookPicturePage : ContentPage
    {
        private readonly UsersPicture pic;

        public PocketBookPicturePage(UsersPicture picture)
        {
            InitializeComponent();
            pic = picture;
            title.Text = picture.Category;
            MemoryStream stream = new MemoryStream(picture.Picture);
            image.Source = ImageSource.FromStream(() => stream);
        }

        private void GoBack(object sender, EventArgs e)
        {
            Navigation?.PopModalAsync();
        }

        private void Delete(object sender, EventArgs e)
        {
            UserDialogs.Instance.Confirm(new ConfirmConfig { OkText = (string)Application.Current.Resources["yes"], CancelText = (string)Application.Current.Resources["no"], Message = (string)Application.Current.Resources["deleteConfirm"], OnAction = (x) => 
            {
                if (x)
                {
                    Navigation?.PopModalAsync();
                    PicturesDatabase.RemovePicture(pic);
                }
            }});
        }
    }
}