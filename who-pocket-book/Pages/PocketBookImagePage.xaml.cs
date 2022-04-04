using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace who_pocket_book.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PocketBookImagePage : ContentPage
    {
        public PocketBookImagePage(string title, ImageSource imageSource)
        {
            InitializeComponent();
            TextLabel.Text = title;
            ImageView.Source = imageSource;
        }
    }
}