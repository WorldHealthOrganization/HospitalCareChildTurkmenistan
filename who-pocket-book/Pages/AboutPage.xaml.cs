using Newtonsoft.Json;
using System;
using who_pocket_book.Models;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Contact = who_pocket_book.Models.Contact;

namespace who_pocket_book.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();
            LoadAboutInfo();
        }

        private async void LoadAboutInfo()
        {
            var fileManager = DependencyService.Get<IFileManager>();
            string json = await fileManager.Read("about");
            if (json != null)
            {
                var contact = JsonConvert.DeserializeObject<Contact>(json);
                email.Text = contact.Email;
                email.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => OnEmailClick()) });
                address.Text = string.Join("\n", contact.Address);
                title2.Text = contact.Title2; 
            }
            else
            {
                email.Text = "unknown";
                address.Text = "unknown";
               
                title2.Text = "unknown";
               
            }
        }

        private void OnEmailClick()
        {
            Device.OpenUri(new Uri($"mailto:{email.Text}"));
        }
    }
}