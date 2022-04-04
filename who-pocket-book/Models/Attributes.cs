using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace who_pocket_book.Models
{
    public class Attributes
    {
        [JsonProperty("MEASURE_TYPE")]
        public string MeasureType { get; set; }
    }
}
