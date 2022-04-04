using Acr.UserDialogs;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using System.IO;
using who_pocket_book.Db;
using who_pocket_book.Pages;
using Xamarin.Forms;

namespace who_pocket_book
{
    public partial class App : Application
    {
        public static Database Database = new Database(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "measures.db"));

        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new MainPage(UserDialogs.Instance));
        }

        protected override void OnStart()
        {
#if !DEBUG
            AppCenter.Start("ios=04095ad7-b8c1-452f-ba94-578f97778d1d;" + "android=aa7b19a5-96ed-4123-9242-9e7ebdf825d9;", typeof(Analytics), typeof(Crashes));
#endif
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
