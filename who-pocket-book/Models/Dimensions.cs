using Newtonsoft.Json;

namespace who_pocket_book.Models
{
    public class Dimensions
    {
        [JsonProperty("AGE_GRP_LIST")]
        public string AgeGroupList { get; set; }
        [JsonProperty("AGE_GRP_3")]
        public string AgeGroup { get; set; }
        [JsonProperty("COUNTRY")]
        public string Country { get; set; }
        [JsonProperty("PLACE_RESIDENCE")]
        public string PlaceResidence { get; set; }
        [JsonProperty("SEX")]
        public string Sex { get; set; }
        [JsonProperty("YEAR")]
        public string Year { get; set; }
    }
}
