using Microcharts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using who_pocket_book.Api;
using who_pocket_book.Models;
using who_pocket_book.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ChartEntry = Microcharts.ChartEntry;

namespace who_pocket_book.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CountryProfilePage : ContentPage
    {

        private ApiService apiService = ApiService.GetInstance();

        public CountryProfilePage()
        {
            InitializeComponent();
            SetInfo();
            LoadCountryProfile();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (Device.RuntimePlatform == Device.Android)
            {
                DependencyService.Get<IOrientationHandler>().ForcePortrait();
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (Device.RuntimePlatform == Device.Android)
            {
                DependencyService.Get<IOrientationHandler>().ForceDefault();
            }
        }

        private void SetInfo()
        {
            title.Text = (string)Application.Current.Resources["child_and_adolescent_health_title"];
            description.Text = (string)Application.Current.Resources["child_and_adolescent_health_description"];
            subtitle.Text = (string)Application.Current.Resources["population_and_health_system_title"];
            footerTitle.Text = (string)Application.Current.Resources["health_expenditure"];
            noAvailableLabel.Text = (string)Application.Current.Resources["data_not_available"];
            demographicsTitle.Text = (string)Application.Current.Resources["demographics"];
            healthSystemTitle.Text = (string)Application.Current.Resources["health_system"];
            policiesTitle.Text = (string)Application.Current.Resources["policies"];
        }

        private async void LoadCountryProfile()
        {
            List<Measure> allMeasures = await App.Database.GetMeasuresAsync();
            var measures = allMeasures.FindAll(measure => ApiService.DEMOGRAPHICS_MEASURES.Concat(ApiService.HEALTH_SYSTEM_MEASURES).Concat(ApiService.POLICIES_MEASURES).Contains(measure.Code));
            var isUpdated = measures != null && measures.Count > 0 ? DateTime.Now.AddMonths(-3) < measures[0].LastUpdated : false;
            if (isUpdated)
            {
                SetCountryProfile(measures);
            }
            else
            {
                measures = await apiService.GetCountryProfile(DateTime.Now.Year);
                if (measures != null && measures.Count > 0)
                {
                    await App.Database.SaveMeasuresAsync(measures);
                    SetCountryProfile(measures);
                }
            }
        }

        private void SetCountryProfile(List<Measure> measures)
        {
            SetClickListener(demographicsContainer, demographicsMeasures, valueLabel1, expand1);
            SetClickListener(policiesContainer, policiesMeasures, valueLabel2, expand2);
            SetClickListener(healthSystemContainer, healthSystemMeasures, valueLabel3, expand3);
            SetClickListener(healthExpenditureContainer, healthExpenditureMeasures, null, expand4, true);
            SetMeasures(measures.Where(measure => ApiService.DEMOGRAPHICS_MEASURES.Contains(measure.Code)).ToList(), demographicsMeasures);
            SetMeasures(measures.Where(measure => ApiService.HEALTH_SYSTEM_MEASURES.Contains(measure.Code)).ToList(), healthSystemMeasures);
            SetMeasures(measures.Where(measure => ApiService.POLICIES_MEASURES.Contains(measure.Code)).ToList(), policiesMeasures);
            SetHealthExpenditure(measures.Where(measure => ApiService.HEALTH_EXPENDITURE_MEASURES.Contains(measure.Code) && measure.Data.Count != 0).ToList());
        }

        private void SetMeasures(List<Measure> measures, StackLayout container)
        {
            var measureItems = new List<MeasureItem>();
            for (int i = 0; i < measures.Count; i++)
            {
                var measure = measures[i];
                if (measure.Data.Count > 0)
                {
                    if (measure.Code == "CAH_3")
                    {
                        measureItems.Add(new MeasureItem(measure));
                        measureItems.Add(new MeasureItem(MapCAH_3(measure, 0)));
                        measureItems.Add(new MeasureItem(MapCAH_3(measure, 1)));
                        continue;
                    }
                    measureItems.Add(new MeasureItem(measure));
                }
            }
            for (int i = 0; i < measureItems.Count; i++)
            {
                container.Children.Add(measureItems[i]);
                if (i != measureItems.Count - 1)
                {
                    container.Children.Add(new BoxView { Color = Color.Gray, HorizontalOptions = LayoutOptions.FillAndExpand, HeightRequest = 1 });
                }
            }
        }

        private void SetHealthExpenditure(List<Measure> measures)
        {
            if (measures.Count != ApiService.HEALTH_EXPENDITURE_MEASURES.Length)
            {
                healthExpenditureName.IsVisible = false;
                //noAvailableLabel.IsVisible = true;
                //chartView.IsVisible = false;
            } else
            {
                noAvailableLabel.IsVisible = false;
                chartView.IsVisible = true;

                var entries = new List<ChartEntry>();
                var total = measures.Find(measure => measure.Code == ApiService.TOTAL_HEALTH_EXPENDITURE_MEASURE).Data[0].Value.Numeric;
                var random = new Random();
                measures.ForEach(measure =>
                {
                    var data = measure.Data[0];
                    var colorHex = String.Format("#{0:X6}", random.Next(0x1000000));
                    if (measure.Code == ApiService.TOTAL_HEALTH_EXPENDITURE_MEASURE)
                    {
                        entries.Add(new ChartEntry(data.Value.Numeric)
                        {
                            Label = Truncate(measure.ShortName, 25),
                            ValueLabel = data.Value.Display,
                            Color = SKColor.Parse(colorHex)
                        });
                    }
                    else
                    {
                        entries.Add(new ChartEntry(data.Value.Numeric / 100f * total)
                        {
                            Label = Truncate(measure.ShortName, 25),
                            ValueLabel = data.Value.Display + "%",
                            Color = SKColor.Parse(colorHex)
                        });
                    }
                });

                chartView.Chart = new DonutChart() { Entries = entries };
            }
        }

        private async void SetClickListener(StackLayout container, StackLayout child, Label label, Image image, bool isScrolled = false)
        {
            container.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => {
                    child.IsVisible = !child.IsVisible;
                    image.IsVisible = !image.IsVisible;
                    if (label != null)
                    {
                        label.IsVisible = !label.IsVisible;
                    }
                    if (isScrolled)
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await scrollView.ScrollToAsync(healthExpenditureMeasures, ScrollToPosition.End, Device.RuntimePlatform == Device.iOS);
                        });
                    }
                })
            }
           );
        }

        private Measure MapCAH_3(Measure measure, int index)
        {
            switch (index)
            {
                case 0:
                    measure.ShortName = (string)Application.Current.Resources["MapCAH_3_0_14"];
                    if (measure.Data.Count > 0)
                    {
                        measure.Data[0] = measure.Data.Find(data => data.Dimensions.AgeGroup == "LT15");
                    }
                    break;
                case 1:
                    measure.ShortName = (string)Application.Current.Resources["MapCAH_3_15_19"];
                    if (measure.Data.Count > 0)
                    {
                        measure.Data[0] = measure.Data.Find(data => data.Dimensions.AgeGroup == "15_19");
                    }
                    break;
            }

            return measure;
        }

        public string Truncate(string value, int maxChars)
        {
            return value.Length <= maxChars ? value : value.Substring(0, maxChars) + "...";
        }
    }
}