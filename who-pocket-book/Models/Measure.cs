using Newtonsoft.Json;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace who_pocket_book.Models
{
    public class Measure
    {
        [JsonIgnore]
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("short_name")]
        public string ShortName { get; set; }
        [JsonProperty("full_name")]
        public string FullName { get; set; }
        [TextBlob("DataBlobbed")]
        [JsonProperty("data")]
        public List<Data> Data { get; set; }
        [JsonIgnore]
        public string DataBlobbed { get; set; }
        [JsonIgnore]
        public DateTime LastUpdated { get; set; }
    }
}