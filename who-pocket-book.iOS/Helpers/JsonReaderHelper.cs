using Foundation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using who_pocket_book.Helpers;
using who_pocket_book.iOS.Helpers;
using who_pocket_book.Models;
using Xamarin.Forms;

[assembly: Dependency(typeof(JsonReaderHelper))]
namespace who_pocket_book.iOS.Helpers
{
    public class JsonReaderHelper : IJsonReaderHelper
    {
        public List<List<ContentPageItem>> GetContentPageItems(string title)
        {
            string json = string.Empty;
            using (StreamReader s = new StreamReader(Path.Combine(NSBundle.MainBundle.BundlePath, "assets", title)))
            {
                json = s.ReadToEnd();
            }
            return JsonConvert.DeserializeObject<List<List<ContentPageItem>>>(json);
        }

        public List<ContentSearchItem> GetSearchPageItems()
        {
            string json = string.Empty;
            using (StreamReader s = new StreamReader(Path.Combine(NSBundle.MainBundle.BundlePath, "assets", "search.json")))
            {
                json = s.ReadToEnd();
            }
            return JsonConvert.DeserializeObject<List<ContentSearchItem>>(json);
        }

        public Dictionary<string, Tuple<int, string>> GetTitles()
        {
            string json = string.Empty;
            using (StreamReader s = new StreamReader(Path.Combine(NSBundle.MainBundle.BundlePath, "assets", "filesTitles.json")))
            {
                json = s.ReadToEnd();
            }
            return JsonConvert.DeserializeObject<List<ContentTitleItem>>(json).ToDictionary(x => x.Path, x => new Tuple<int, string>(x.Id, x.Title));
        }
    }
}