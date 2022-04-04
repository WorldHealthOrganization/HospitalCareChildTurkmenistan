using Newtonsoft.Json;
using System;

namespace who_pocket_book.Models
{
    public class Value
    {
        [JsonProperty("display")]
        public string Display { get; set; }
        [JsonProperty("numeric")]
        public float? _Numeric { get; set; }
        [JsonIgnore]
        public float Numeric
        {
            get { return _Numeric.Value; }
            set { _Numeric = new Nullable<float>(value); }
        }
    }
}
