using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using who_pocket_book.Models;
using Xamarin.Forms;

namespace who_pocket_book.Api
{
    class ApiService
    {
        public static readonly string BASE_URL = "https://dw.euro.who.int/api/v3/";

        public static readonly string[] DEMOGRAPHICS_MEASURES = { "HFA_1", "CAH_65", "CAH_3", "HFA_16", "HFA_592", "HFA_78", "HFA_61", "CAH_9", "HFA_618" };
        public static readonly string[] HEALTH_SYSTEM_MEASURES = { "HFA_33", "HFA_570", "HFA_583", "HFA_584", "CAH_15", "CAH_16", "CAH_17" };
        public static readonly string[] HEALTH_EXPENDITURE_MEASURES = { "HFA_570", "HFA_583", "HFA_584" };
        public static readonly string TOTAL_HEALTH_EXPENDITURE_MEASURE = "HFA_570";
        public static readonly string[] POLICIES_MEASURES = { "CAH_20", "CAH_21", "CAH_19", "HFA_463", "HFA_466" };

        public static readonly string[] INFECTIOUS_DESEASES_MEASURES = { "CAH_51", "CAH_31", "CAH_32", "CAH_63", "HFA_606", "HFA_609", "HFA_611", "CAH_46", "CAH_47", "CAH_48", "CAH_49", "CAH_50" };
        public static readonly string[] DIARRHOEA_MEASURES = { "CAH_51" };
        public static readonly string[] VACCINES_MEASURES = { "CAH_31", "CAH_32", "CAH_63", "HFA_606", "HFA_609", "HFA_611" };
        public static readonly string[] HIV_MEASURES = { "CAH_46", "CAH_47" };
        public static readonly string[] TUBERCULOSIS_MEASURES = { "CAH_48", "CAH_49", "CAH_50" };

        private static readonly string COUNTRY_PROFILE_URL = "batch/measures?codes={0}&output=data&filter=COUNTRY:{1};YEAR:~{2};SEX:ALL&lang={3}";
        private static readonly string INFECTIOUS_DISEASES_URL = "batch/measures?codes={0}&output=data&filter=COUNTRY:{1}&lang={2}";

        private static ApiService Instance = null;

        private HttpClient http = new HttpClient();

        private ApiService()
        {
        }

        public static ApiService GetInstance()
        {
            if (Instance == null)
            {
                Instance = new ApiService();
            }

            return Instance;
        }

        public async Task<List<Measure>> GetCountryProfile(int year)
        {
            try
            {
                var url = BASE_URL + String.Format(COUNTRY_PROFILE_URL, String.Join(",", DEMOGRAPHICS_MEASURES.Concat(HEALTH_SYSTEM_MEASURES).Concat(POLICIES_MEASURES)), Configs.COUNTRY, year, Configs.LANGUAGE);
                var response = await http.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string jsonResult = await response.Content.ReadAsStringAsync();
                    var lastUpdated = DateTime.Now;
                    List<Measure> measures = JsonConvert.DeserializeObject<List<Measure>>(jsonResult)
                        .Select(measure => mapMeasure(measure, lastUpdated))
                        .ToList();

                    return (measures);
                }
            }
            catch (Exception e)
            {
            }

            return (null);
        }

        public async Task<List<Measure>> GetInfectiousDiseases()
        {
            try
            {
                var url = BASE_URL + String.Format(INFECTIOUS_DISEASES_URL, String.Join(",", INFECTIOUS_DESEASES_MEASURES), Configs.COUNTRY, Configs.LANGUAGE);
                var response = await http.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string jsonResult = await response.Content.ReadAsStringAsync();
                    var lastUpdated = DateTime.Now;
                    List<Measure> measures = JsonConvert.DeserializeObject<List<Measure>>(jsonResult)
                        .Select(measure => mapMeasure(measure, lastUpdated))
                        .ToList();

                    return (measures);
                }
            }
            catch (Exception e)
            {
            }


            return (null);
        }

        private Measure mapMeasure(Measure measure, DateTime lastUpdated)
        {
            measure.LastUpdated = lastUpdated;
            switch (measure.Code)
            {
                case "HFA_1":
                    measure.ShortName = (string)Application.Current.Resources["HFA_1"];
                    break;
                case "CAH_65":
                    measure.ShortName = (string)Application.Current.Resources["CAH_65"];
                    break;
                case "HFA_618":
                    measure.ShortName = (string)Application.Current.Resources["HFA_618"];
                    break;
                case "HFA_61":
                    measure.ShortName = (string)Application.Current.Resources["HFA_61"];
                    break;
                case "HFA_78":
                    measure.ShortName = (string)Application.Current.Resources["HFA_78"];
                    break;
                case "HFA_592":
                    measure.ShortName = (string)Application.Current.Resources["HFA_592"];
                    break;
                case "HFA_16":
                    measure.ShortName = (string)Application.Current.Resources["HFA_16"];
                    break;
                case "CAH_9":
                    measure.ShortName = (string)Application.Current.Resources["CAH_9"];
                    if (measure.Data.Count > 0)
                    {
                        measure.Data[0] = measure.Data.Find(data => data.Dimensions.AgeGroupList == "LT16");
                    }
                    break;
                case "CAH_3":
                    measure.ShortName = (string)Application.Current.Resources["CAH_3"];
                    if (measure.Data.Count > 0)
                    {
                        measure.Data[0] = measure.Data.Find(data => data.Dimensions.AgeGroup == "TOTAL");
                    }
                    break;
                case "CAH_20":
                    measure.ShortName = (string)Application.Current.Resources["CAH_20"];
                    break;
                case "CAH_21":
                    measure.ShortName = (string)Application.Current.Resources["CAH_21"];
                    if (measure.Data.Count > 0)
                    {
                        measure.Data[0].Value.Display = String.Join(" - ", measure.Data.Select(data => data.Value.Display).ToList());
                    }
                    break;
                case "CAH_19":
                    measure.ShortName = (string)Application.Current.Resources["CAH_19"];
                    break;
                case "HFA_463":
                    measure.ShortName = (string)Application.Current.Resources["HFA_463"];
                    if (measure.Data.Count > 0)
                    {
                        measure.Data[0] = measure.Data.Find(data => data.Dimensions.PlaceResidence == "RURAL");
                    }
                    break;
                case "HFA_466":
                    measure.ShortName = (string)Application.Current.Resources["HFA_466"];
                    if (measure.Data.Count > 0)
                    {
                        measure.Data[0] = measure.Data.Find(data => data.Dimensions.PlaceResidence == "RURAL");
                    }
                    break;
                case "HFA_33":
                    measure.ShortName = (string)Application.Current.Resources["HFA_33"];
                    break;
                case "HFA_570":
                    measure.ShortName = (string)Application.Current.Resources["HFA_570"];
                    break;
                case "HFA_583":
                    measure.ShortName = (string)Application.Current.Resources["HFA_583"];
                    break;
                case "HFA_584":
                    measure.ShortName = (string)Application.Current.Resources["HFA_584"];
                    break;
                case "CAH_15":
                    measure.ShortName = (string)Application.Current.Resources["CAH_15"];
                    break;
                case "CAH_16":
                    measure.ShortName = (string)Application.Current.Resources["CAH_16"];
                    break;
                case "CAH_17":
                    measure.ShortName = (string)Application.Current.Resources["CAH_17"];
                    break;
                case "CAH_46":
                    measure.ShortName = (string)Application.Current.Resources["CAH_46"];
                    break;
                case "CAH_47":
                    measure.ShortName = (string)Application.Current.Resources["CAH_47"];
                    break;
                case "HFA_606":
                    measure.ShortName = (string)Application.Current.Resources["HFA_606"];
                    break;
                case "HFA_609":
                    measure.ShortName = (string)Application.Current.Resources["HFA_609"];
                    break;
                case "HFA_611":
                    measure.ShortName = (string)Application.Current.Resources["HFA_611"];
                    break;
                case "CAH_31":
                    measure.ShortName = (string)Application.Current.Resources["CAH_31"];
                    break;
                case "CAH_63":
                    measure.ShortName = (string)Application.Current.Resources["CAH_63"];
                    break;
                case "CAH_32":
                    measure.ShortName = (string)Application.Current.Resources["CAH_32"];
                    break;
                default:
                    break;
            }

            return measure;
        }
    }
}
