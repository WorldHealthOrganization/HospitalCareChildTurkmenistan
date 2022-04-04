using System;
using who_pocket_book.Db;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace who_pocket_book.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PocketBookNotePage : ContentPage
    {
        public PocketBookNotePage(string title)
        {
            InitializeComponent();
            this.title.Text = title;
            editorField.Text = NotesDatabase.GetNote(title)?.Text ?? "";
        }

        private void GoBack(object sender, EventArgs e)
        {
            Navigation?.PopModalAsync();
        }

        private async void Save(object sender, EventArgs e)
        {
            await NotesDatabase.UpsertNote(title.Text, editorField.Text);
            await Navigation?.PopModalAsync();
        }
    }
}