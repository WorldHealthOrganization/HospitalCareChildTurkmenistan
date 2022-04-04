using Microcharts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using who_pocket_book.Api;
using who_pocket_book.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Entry = Microcharts.ChartEntry;

namespace who_pocket_book.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InfectiousDiseasesPage : ContentPage
    {

        private ApiService apiService = ApiService.GetInstance();
        private Random random = new Random();

        public InfectiousDiseasesPage()
        {
            InitializeComponent();
            SetInfo();
            LoadInfectiousDiseases();
        }

        private void SetInfo()
        {
          
            hivRatesTitle.Text = (string)Application.Current.Resources["estimated_hiv_rates"];
            vaccinationCoverageTitle.Text = (string)Application.Current.Resources["vaccination_coverage"];
            diarrhoeaTitle.Text = (string)Application.Current.Resources["diarrhoea"];
            tuberculosisTitle.Text = (string)Application.Current.Resources["tuberculosis"];
        }

        private async void LoadInfectiousDiseases()
        {
            List<Measure> allMeasures = await App.Database.GetMeasuresAsync();
            var measures = allMeasures.FindAll(measure => ApiService.INFECTIOUS_DESEASES_MEASURES.Contains(measure.Code));
            var isUpdated = measures != null && measures.Count > 0 ? DateTime.Now.AddMonths(-3) < measures[0].LastUpdated : false;
            if (isUpdated)
            {
                SetInfectiousDiseases(measures);
            }
            else
            {
                measures = await apiService.GetInfectiousDiseases();
                if (measures != null && measures.Count > 0)
                {
                    SetInfectiousDiseases(measures);
                    await App.Database.SaveMeasuresAsync(measures);
                }
            }
        }

        private void SetInfectiousDiseases(List<Measure> measures)
        {
            SetHivRates(measures.Where(measure => ApiService.HIV_MEASURES.Contains(measure.Code)).ToList());
            setVaccinationCoverage(measures.Where(measure => ApiService.VACCINES_MEASURES.Contains(measure.Code)).ToList());
            setDiarrhoea(measures.Where(measure => ApiService.DIARRHOEA_MEASURES.Contains(measure.Code)).ToList());
            setTuberculosis(measures.Where(measure => ApiService.TUBERCULOSIS_MEASURES.Contains(measure.Code)).ToList());
        }

        private void SetHivRates(List<Measure> measures)
        {
            if (measures[0].Data[0].Value.Display != "")
            {
                estimatedHIVRates.Text = measures[0].ShortName + ", " + (string)Application.Current.Resources["all"];
                SetRates(measures[0].Data, estimatedHIVRatesData, estimatedHIVRatesYears);
            }
            else
            {
                hivRatesTitle.IsVisible = false;
                estimatedHIVRatesContainer.IsVisible = false;
            estimatedHIVRates.IsVisible = false;
            estimatedHIVRatesMaleContainer.IsVisible = false;
        }


            if (measures[1].Data.Count > 0)
            {
                var femaleMeasures = measures[1].Data.Where(measure => measure.Dimensions.Sex == "FEMALE").ToList();
                estimatedHIVRatesFemale.Text = measures[1].ShortName + ", " + (string)Application.Current.Resources["female"];
                SetRates(femaleMeasures, estimatedHIVRatesFemaleData, estimatedHIVRatesFemaleYears);

                var maleMeasures = measures[1].Data.Where(measure => measure.Dimensions.Sex == "MALE").ToList();
                estimatedHIVRatesMale.Text = measures[1].ShortName + ", " + (string)Application.Current.Resources["male"];
                SetRates(femaleMeasures, estimatedHIVRatesMaleData, estimatedHIVRatesMaleYears);
            }
            else
            {
                estimatedHIVRatesFemaleContainer1.IsVisible = false;
                estimatedHIVRatesFemaleContainer2.IsVisible = false;
                estimatedHIVRatesContainer1.IsVisible = false;
                estimatedHIVRatesContainer2.IsVisible = false;
            }
        }

        private void SetRates(List<Data> measures, Label labelData, Label labelYears)
        {
            var startYear = "2000";
            var middleYear = "2007";
            var lastYear = measures.Count > 0 ? measures.Last().Dimensions.Year : "2013";
            var rates = new Dictionary<string, Data>
            {
                { startYear, measures.Find(data => data.Dimensions.Year == startYear)},
                { middleYear, measures.Find(data => data.Dimensions.Year == middleYear) },
                { lastYear, measures.Find(data => data.Dimensions.Year == lastYear)}
            };
            labelData.Text = String.Join("\n", rates.Values.Select(data => data == null ? (string)Application.Current.Resources["data_not_available"] : data.Value._Numeric == null ? (string)Application.Current.Resources["data_not_available"] : data.Value.Display + (data.Attributes.MeasureType == "PROPORTION" ? "%" : "")).ToList());
            labelYears.Text = String.Join("\n", rates.Keys);
        }

        private void setVaccinationCoverage(List<Measure> measures)
        {
            vaccinationDiphtheria.Text = measures[3].ShortName;
            var chartColor = String.Format("#{0:X6}", random.Next(0x1000000));
            var entriesDiphtheria = measures[3].Data
                .Where(measure => measure.Value._Numeric != null)
                .OrderBy(measure => Int32.Parse(measure.Dimensions.Year))
                .Select(measure => new Entry(measure.Value.Numeric)
                {
                    Label = measure.Dimensions.Year,
                    ValueLabel = measure.Value.Display + "%",
                    Color = SKColor.Parse(chartColor)
                })
                .ToArray();
            chartViewDiphtheria.WidthRequest = entriesDiphtheria.Length * 32;
            chartViewDiphtheria.Chart = new LineChart() { Entries = entriesDiphtheria };

            vaccinationMeasles.Text = measures[4].ShortName;
            chartColor = String.Format("#{0:X6}", random.Next(0x1000000));
            var entriesMeasles = measures[4].Data
                .Where(measure => measure.Value._Numeric != null)
                .OrderBy(measure => Int32.Parse(measure.Dimensions.Year))
                .Select(measure => new Entry(measure.Value.Numeric)
                {
                    Label = measure.Dimensions.Year,
                    ValueLabel = measure.Value.Display + "%",
                    Color = SKColor.Parse(chartColor)
                })
                .ToArray();
            chartViewMeasles.WidthRequest = entriesMeasles.Length * 32;
            chartViewMeasles.Chart = new LineChart() { Entries = entriesMeasles };

            vaccinationHaemophilus.Text = measures[5].ShortName;
            chartColor = String.Format("#{0:X6}", random.Next(0x1000000));
            var entriesHaemophilus = measures[5].Data
                .Where(measure => measure.Value._Numeric != null)
                .OrderBy(measure => Int32.Parse(measure.Dimensions.Year))
                .Select(measure => new Entry(measure.Value.Numeric)
                {
                    Label = measure.Dimensions.Year,
                    ValueLabel = measure.Value.Display + "%",
                    Color = SKColor.Parse(chartColor)
                })
                .ToArray();
            chartViewHaemophilus.Chart = new LineChart() { Entries = entriesHaemophilus };





            vaccinationHPV.Text = measures[2].ShortName;
            if (measures[2].Data.Where(measure => measure.Value._Numeric != null).ToList().Count > 0)
            {
                chartViewHPV.IsVisible = true;
                vaccinationHPVNotAvailable.IsVisible = false;
                chartColor = String.Format("#{0:X6}", random.Next(0x1000000));
                var entriesHPV = measures[2].Data
                    .Where(measure => measure.Value._Numeric != null)
                    .OrderBy(measure => Int32.Parse(measure.Dimensions.Year))
                    .Select(measure => new Entry(measure.Value.Numeric)
                    {
                        Label = measure.Dimensions.Year,
                        ValueLabel = measure.Value.Display + "%",
                        Color = SKColor.Parse(chartColor)
                    })
                    .ToArray();
                chartViewHPV.Chart = new LineChart() { Entries = entriesHPV };
            }
            else
            {
                vaccinationHPV1.IsVisible = false;
                vaccinationHPV2.IsVisible = false;
                //chartViewHPV.IsVisible = false;
                //vaccinationHPVNotAvailable.IsVisible = true;
            }


            vaccinationRotavirus.Text = measures[1].ShortName;
            if (measures[1].Data.Where(measure => measure.Value._Numeric != null).ToList().Count > 0)
            {
                chartViewRotavirus.IsVisible = true;
                vaccinationRotavirusNotAvailable.IsVisible = false;
                chartColor = String.Format("#{0:X6}", random.Next(0x1000000));
                var entriesRotavirus = measures[1].Data
                    .Where(measure => measure.Value._Numeric != null)
                    .OrderBy(measure => Int32.Parse(measure.Dimensions.Year))
                    .Select(measure => new Entry(measure.Value.Numeric)
                    {
                        Label = measure.Dimensions.Year,
                        ValueLabel = measure.Value.Display + "%",
                        Color = SKColor.Parse(chartColor)
                    })
                    .ToArray();
                chartViewRotavirus.Chart = new LineChart() { Entries = entriesRotavirus };
            }
            else
            {
                vaccinationRotavirus1.IsVisible = false;
                vaccinationRotavirus2.IsVisible = false;
                //chartViewRotavirus.IsVisible = false;
                //vaccinationRotavirusNotAvailable.IsVisible = true;
            }




            if (measures[1].Data.Where(measure => measure.Value._Numeric != null).ToList().Count > 0)
            {
                vaccinationPCVNotAvailable.IsVisible = true;
                chartViewPCV.IsVisible = false;
                vaccinationPCV.Text = measures[0].ShortName;
                chartColor = String.Format("#{0:X6}", random.Next(0x1000000));
                var entriesPCV = measures[0].Data
                    .Where(measure => measure.Value._Numeric != null)
                    .OrderBy(measure => Int32.Parse(measure.Dimensions.Year))
                    .Select(measure => new Entry(measure.Value.Numeric)
                    {
                        Label = measure.Dimensions.Year,
                        ValueLabel = measure.Value.Display + "%",
                        Color = SKColor.Parse(chartColor)
                    })
                    .ToArray();
                chartViewPCV.Chart = new LineChart() { Entries = entriesPCV };
                // vaccinationPCVNotAvailable.IsVisible = entriesPCV.Count() == 0;
                // chartViewPCV.IsVisible = entriesPCV.Count() > 0;
            }
            else
            {
                //chartViewPCV.IsVisible = false;
                // vaccinationPCV.Text = measures[0].ShortName;
                vaccinationPCV1.IsVisible = false;
                vaccinationPCV2.IsVisible = false;
                //chartViewRotavirus.IsVisible = false;
                //vaccinationRotavirusNotAvailable.IsVisible = true;
            }

        }

        private void setDiarrhoea(List<Measure> measures)
        {
            var data = measures[0].Data;
            if (data.Count == 0)
            {
                diarrhoeaTitle.IsVisible = false;
                diarrhoeaContainer.IsVisible = false;
                //chartViewDiarrhoea.IsVisible = false;
                //diarrhoeaNotAvailable.IsVisible = true;
            }
            else
            {
                diarrhoeaNotAvailable.IsVisible = false;
                chartViewDiarrhoea.IsVisible = true;
                var entriesDiarrhoea = data.Where(measure => measure.Value._Numeric != null)
                .OrderBy(measure => Int32.Parse(measure.Dimensions.Year))
                .Select(measure => new Entry(measure.Value.Numeric)
                {
                    Label = measure.Dimensions.Year,
                    ValueLabel = measure.Value.Display + "%",
                    Color = SKColor.Parse("#266489")
                })
                .ToArray();
                chartViewDiarrhoea.Chart = new LineChart() { Entries = entriesDiarrhoea };
            }
        }

        private void setTuberculosis(List<Measure> measures)
        {
            if (measures.All(measure => measure.Data.Count == 0))
            {
                tuberculosisTitle.IsVisible = false;
                tuberculosisContainer.IsVisible = false;
                //chartViewTuberculosis.IsVisible = false;
                //tuberculosisNotAvailable.IsVisible = true;
            }
            else
            {
                tuberculosisNotAvailable.IsVisible = false;
                chartViewTuberculosis.IsVisible = true;

                var entriesTuberculosis = new List<Entry>();
                measures.Where(measure => measure.Data.Count > 0)
                    .ToList()
                    .ForEach(measure =>
                    {
                        var entry = measure.Data.Where(data => data.Value._Numeric != null)
                            .OrderBy(data => Int32.Parse(data.Dimensions.Year))
                            .Select(data => new Entry(data.Value.Numeric)
                            {
                                Label = measure.ShortName,
                                ValueLabel = String.Concat(data.Value.Display, data.Attributes.MeasureType == "PROPORTION" ? "%" : ""),
                                Color = SKColor.Parse("#266489")
                            })
                            .Last();
                        entriesTuberculosis.Add(entry);
                    });

                chartViewTuberculosis.Chart = new BarChart() { Entries = entriesTuberculosis };
            }
        }
    }
}