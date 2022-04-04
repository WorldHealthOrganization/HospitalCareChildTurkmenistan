using who_pocket_book.Models;
using Xamarin.Forms;

namespace who_pocket_book.Views
{
    class MeasureItem : Grid
    {
        public MeasureItem(Measure measure)
        {
            RowDefinitions = new RowDefinitionCollection { new RowDefinition { Height = GridLength.Star } };
            ColumnDefinitions = new ColumnDefinitionCollection {
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Auto }
            };
            SetMeasure(measure);
        }

        private void SetMeasure(Measure measure)
        {
            var textColor = (Color)App.Current.Resources["colorPageTitle"];
            var shortNameLabel = new Label()
            {
                Text = measure.ShortName,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Start,
                Margin = new Thickness(8, 0, 12, 0),
                TextColor = textColor
            };
            Children.Add(shortNameLabel, 0, 0);

            if (measure.Data.Count == 0)
            {
                var valueLabel = new Label()
                {
                    Text = (string)Application.Current.Resources["data_not_available"],
                    Margin = new Thickness(0, 0, 8, 0),
                    TextColor = textColor
                };
                Children.Add(valueLabel, 1, 0);
            }
            else
            {
                var data = measure.Data[0];
                var valueLabel = new Label()
                {
                    Text = data.Value._Numeric.HasValue ? data.Value.Display : (string)Application.Current.Resources["data_not_available"],
                    TextColor = textColor
                };
                var typeLabel = new Label()
                {
                    Text = data.Attributes.MeasureType == "PROPORTION" ? "%" : "",
                    TextColor = textColor
                };
                var yearLabel = new Label()
                {
                    Text = $"({data.Dimensions.Year})",
                    Margin = new Thickness(4, 0, 8, 0),
                    TextColor = textColor
                };
                Children.Add(new StackLayout {
                    Orientation = StackOrientation.Horizontal,
                    //WidthRequest = 190.0,
                    Children = { valueLabel, typeLabel, yearLabel }
                }, 1, 0);
            }
        }
    }
}