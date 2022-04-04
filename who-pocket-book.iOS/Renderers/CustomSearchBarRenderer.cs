using System.Linq;
using UIKit;
using who_pocket_book.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(SearchBar), typeof(CustomSearchBarRenderer))]
namespace who_pocket_book.iOS.Renderers
{
    public class CustomSearchBarRenderer : SearchBarRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<SearchBar> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                Control.TextChanged += (s, ea) =>
                {
                    //Control.ShowsCancelButton = true;
                    SetCancelButtonText();
                };

                Control.OnEditingStarted += (s, ea) => //when control receives focus
                {
                    //Control.ShowsCancelButton = true;
                    SetCancelButtonText();
                };

                Control.OnEditingStopped += (s, ea) => //when control looses focus 
                {
                    //Control.ShowsCancelButton = false;
                };
            }
        }

        private void SetCancelButtonText()
        {
            UIButton cancelButton = Control.Descendants().OfType<UIButton>().FirstOrDefault();
            if (cancelButton != null)
            {
                cancelButton.SetTitle((string)Xamarin.Forms.Application.Current.Resources["cancel"], UIControlState.Normal);
            }
        }
    }
}