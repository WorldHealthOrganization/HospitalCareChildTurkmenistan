using CoreSpotlight;
using Foundation;
using System.Collections.Generic;
using System.Linq;
using UIKit;
using who_pocket_book.Helpers;
using who_pocket_book.Models;
using who_pocket_book.Pages;
using Xamarin.Forms;

namespace who_pocket_book.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        private List<ContentSearchItem> items;
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Xamarin.Forms.Forms.Init();
            SetupSearchIndexes();
            if (UIScreen.MainScreen.Bounds.Width <= 375)
            {
                UITabBarItem.Appearance.SetTitleTextAttributes(new UITextAttributes { Font = UIFont.SystemFontOfSize(9) }, UIControlState.Normal);
                UITabBarItem.Appearance.SetTitleTextAttributes(new UITextAttributes { Font = UIFont.SystemFontOfSize(9) }, UIControlState.Selected);
            }
            LoadApplication(new App());
            UIDevice.CurrentDevice.BeginGeneratingDeviceOrientationNotifications();
            return base.FinishedLaunching(app, options);
        }

        private async void SetupSearchIndexes()
        {
            items = DependencyService.Get<IJsonReaderHelper>().GetSearchPageItems();
            await CSSearchableIndex.DefaultSearchableIndex.DeleteAllAsync();
            List<CSSearchableItem> searchableItems = new List<CSSearchableItem>();
            foreach (ContentSearchItem item in items)
            {
                CSSearchableItemAttributeSet attributes = new CSSearchableItemAttributeSet
                {
                    ContentType = "Text",
                    Title = item.Title,
                    ContentDescription = item.Description,
                    Keywords = new string[] { item.SearchKey }
                };
                searchableItems.Add(new CSSearchableItem(item.Id.ToString(), item.ChildElement, attributes));
            }
            await CSSearchableIndex.DefaultSearchableIndex.IndexAsync(searchableItems.ToArray());
        }

        public override bool ContinueUserActivity(UIApplication application, NSUserActivity userActivity, UIApplicationRestorationHandler completionHandler)
        {
            string id = (userActivity.UserInfo[CSSearchableItem.ActivityIdentifier] as NSString).ToString();
            if (items != null && int.TryParse(id, out int Id) && MainPage.Instance != null)
            {
                ContentSearchItem searchItem = items.FirstOrDefault(x => x.Id == Id);
                if (searchItem != null)
                {
                    if (MainPage.Instance.CurrentPage != MainPage.Instance.Children.First())
                    {
                        MainPage.Instance.CurrentPage = MainPage.Instance.Children.First();
                    }
                    switch (searchItem.ChildElementType)
                    {
                        case ChildElementType.Content:
                            DismissSearchPage();
                            (MainPage.Instance.CurrentPage as NavigationPage).PushAsync(new PocketBookContentPage(searchItem.Title, searchItem.ChildElement));
                            break;
                        case ChildElementType.Chapter:
                            DismissSearchPage();
                            (MainPage.Instance.CurrentPage as NavigationPage).PushAsync(new PocketBookPage(searchItem.Title, searchItem.ChildElement));
                            break;
                        default:
                            break;
                    }
                }
            }
            return true;
        }

        private void DismissSearchPage()
        {
            while ((MainPage.Instance.CurrentPage as NavigationPage).Navigation.ModalStack.Count > 0 && (MainPage.Instance.CurrentPage as NavigationPage).Navigation.ModalStack.Last() is PocketBookSearchPage)
            {
                (MainPage.Instance.CurrentPage as NavigationPage).Navigation.PopModalAsync(false);
            }
        }

        public static UIViewController GetTopController()
        {
            UIViewController currentController = UIApplication.SharedApplication.KeyWindow.RootViewController;
            do
            {
                if (currentController is UITabBarController tabBarController)
                {
                    currentController = tabBarController.SelectedViewController;
                }
                else if (currentController is UINavigationController navController)
                {
                    currentController = navController.ViewControllers.Last();
                }
                else if (currentController.PresentedViewController != null)
                {
                    currentController = currentController.PresentedViewController;
                }
                else
                {
                    break;
                }
            }
            while (true);
            return currentController;
        }
    }
}