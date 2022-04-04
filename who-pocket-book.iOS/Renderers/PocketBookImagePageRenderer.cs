using CoreGraphics;
using System;
using UIKit;
using who_pocket_book.iOS.Renderers;
using who_pocket_book.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(PocketBookImagePage), typeof(PocketBookImagePageRenderer))]
[assembly: ExportRenderer(typeof(PocketBookSearchPage), typeof(PocketBookSearchPageRenderer))]
[assembly: ExportRenderer(typeof(PocketBookNotePage), typeof(PocketBookSearchPageRenderer))]
[assembly: ExportRenderer(typeof(PocketBookPicturePage), typeof(PocketBookSearchPageRenderer))]
namespace who_pocket_book.iOS.Renderers
{
    public class PocketBookImagePageRenderer: PageRenderer
    {
        public override void WillMoveToParentViewController(UIViewController parent)
        {
            base.WillMoveToParentViewController(parent);
            if (parent != null)
            {
                parent.ModalInPopover = false;
                parent.ModalInPresentation = false;
                parent.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                CGSize fullSize = UIScreen.MainScreen.Bounds.Size;
                if (fullSize != null && UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
                {
                    parent.PreferredContentSize = new CGSize(Math.Max(fullSize.Height, fullSize.Width), Math.Max(fullSize.Height, fullSize.Width));
                    //parent.PreferredContentSize = new CGSize(fullSize.Height * 0.75, fullSize.Width * 0.8);
                }
            }
        }
    }

    public class PocketBookSearchPageRenderer : PageRenderer
    {
        public override void WillMoveToParentViewController(UIViewController parent)
        {
            base.WillMoveToParentViewController(parent);
            if (parent != null)
            {
                parent.ModalInPopover = false;
                parent.ModalInPresentation = false;
                parent.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                CGSize fullSize = UIScreen.MainScreen.Bounds.Size;
                if (fullSize != null && UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
                {
                    parent.PreferredContentSize = new CGSize(Math.Max(fullSize.Height, fullSize.Width), Math.Max(fullSize.Height, fullSize.Width));
                    //parent.PreferredContentSize = new CGSize(fullSize.Width * 0.6, fullSize.Height * 0.65);
                }
            }
        }
    }
}