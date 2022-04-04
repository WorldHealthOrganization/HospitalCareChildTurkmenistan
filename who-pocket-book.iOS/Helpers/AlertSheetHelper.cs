using System;
using System.Linq;
using UIKit;
using who_pocket_book.Helpers;
using who_pocket_book.iOS.Helpers;
using Xamarin.Forms;

[assembly: Dependency(typeof(AlertSheetHelper))]
namespace who_pocket_book.iOS.Helpers
{
    public class AlertSheetHelper : IAlertSheet
    {
        public void ShowAlert(string title, Action<string> handler, params string[] buttons)
        {
            UIAlertController alertController = UIAlertController.Create(title, null, UIAlertControllerStyle.ActionSheet);
            foreach (string button in buttons)
            {
                alertController.AddAction(UIAlertAction.Create(button, UIAlertActionStyle.Default, (x) => { handler(button); }));
            }
            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
            {
                alertController.AddAction(UIAlertAction.Create((string)Xamarin.Forms.Application.Current.Resources["cancel"], UIAlertActionStyle.Default, null));
            }
            UIViewController vc = UIApplication.SharedApplication.KeyWindow.RootViewController.ChildViewControllers[0].ChildViewControllers[0].ChildViewControllers[0].ChildViewControllers[0].ChildViewControllers[0];
            vc = vc.NavigationController.VisibleViewController;
            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
            {
                alertController.PopoverPresentationController.BarButtonItem = vc.NavigationItem.RightBarButtonItems.Skip(1).FirstOrDefault();
                alertController.PopoverPresentationController.PermittedArrowDirections = UIPopoverArrowDirection.Up;
            }
            vc.PresentViewController(alertController, true, null);
        }
    }
}