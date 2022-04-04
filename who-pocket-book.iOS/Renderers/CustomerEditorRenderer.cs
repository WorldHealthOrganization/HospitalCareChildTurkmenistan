using Foundation;
using System.ComponentModel;
using System.Linq;
using UIKit;
using who_pocket_book.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Editor), typeof(CustomerEditorRenderer))]
namespace who_pocket_book.iOS.Renderers
{
    public class CustomerEditorRenderer : EditorRenderer
    {
        private UILabel _placeholder;

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Editor.TextProperty.PropertyName)
            {
                UpdatePlaceholder();
                return;
            }

            base.OnElementPropertyChanged(sender, e);
        }

        private void UpdatePlaceholder()
        {
            if (_placeholder == null)
            {
                var subview = TextView.Subviews?.OfType<UILabel>().FirstOrDefault();

                _placeholder = subview;
            }

            if (string.IsNullOrEmpty(TextView.Text))
            {
                TextView.AddSubview(_placeholder);

                UpdateUIConstraints();
            }
            else
            {
                _placeholder.RemoveFromSuperview();
            }
        }

        private void UpdateUIConstraints()
        {
            var edgeInsets = TextView.TextContainerInset;
            var lineFragmentPadding = TextView.TextContainer.LineFragmentPadding;

            var vConstraints = NSLayoutConstraint.FromVisualFormat(
                "V:|-" + edgeInsets.Top + $"-[{nameof(_placeholder)}]-" + edgeInsets.Bottom + "-|", 0, new NSDictionary(),
                NSDictionary.FromObjectsAndKeys(
                    new NSObject[] { _placeholder }, new NSObject[] { new NSString(nameof(_placeholder)) })
            );

            var hConstraints = NSLayoutConstraint.FromVisualFormat(
                "H:|-" + lineFragmentPadding + $"-[{nameof(_placeholder)}]-" + lineFragmentPadding + "-|",
                0, new NSDictionary(),
                NSDictionary.FromObjectsAndKeys(
                    new NSObject[] { _placeholder }, new NSObject[] { new NSString(nameof(_placeholder)) })
            );

            _placeholder.TranslatesAutoresizingMaskIntoConstraints = false;

            Control.AddConstraints(hConstraints);
            Control.AddConstraints(vConstraints);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null && Control != null)
            {
                if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
                {
                    Control.TextContainerInset = new UIEdgeInsets(10, 0, UIScreen.MainScreen.Bounds.Height * 0.5f, 0);
                }
                else
                {
                    Control.TextContainerInset = new UIEdgeInsets(10, 0, (UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeLeft || UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeRight) ? 370 : 300, 0);
                    UIDevice.Notifications.ObserveOrientationDidChange(MyRotationCallback);
                }
            }
            if (Control != null)
            {
                UpdatePlaceholder();
            }
        }

        private void MyRotationCallback(object sender, NSNotificationEventArgs e)
        {
            if (Control != null)
            {
                Control.TextContainerInset = new UIEdgeInsets(10, 0, (UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeLeft || UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeRight) ? 370 : 300, 0);
            }
        }
    }
}