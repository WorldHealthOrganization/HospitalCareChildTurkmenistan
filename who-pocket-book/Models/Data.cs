using Newtonsoft.Json;
using SQLite;

namespace who_pocket_book.Models
{
    public class Data
    {
        [JsonProperty("fact_id")]
        public string FactId { get; set; }
        [JsonProperty("attributes")]
        public Attributes Attributes { get; set; }
        [JsonProperty("dimensions")]
        public Dimensions Dimensions { get; set; }
        [JsonProperty("value")]
        public Value Value { get; set; }
      
    }
}
