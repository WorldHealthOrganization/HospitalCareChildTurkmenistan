using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace who_pocket_book.Models
{
    class Contact
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("address")]
        public string[] Address { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("phone2")]
        public string Phone2 { get; set; }

        [JsonProperty("phone3")]
        public string Phone3 { get; set; }

        [JsonProperty("fax")]
        public string Fax { get; set; }

        [JsonProperty("webpage")]
        public string WebPage { get; set; }

        [JsonProperty("follow")]
        public string FollowUs { get; set; }

        [JsonProperty("skype")]
        public string Skype { get; set; }

        [JsonProperty("contactPerson")]
        public string ContactPerson { get; set; }

        [JsonProperty("title2")]
        public string Title2 { get; set; }

        [JsonProperty("person2")]
        public string Person2 { get; set; }

        [JsonProperty("email2")]
        public string Email2 { get; set; }

        [JsonProperty("email3")]
        public string Email3 { get; set; }

        [JsonProperty("title3")]
        public string Title3 { get; set; }

        [JsonProperty("name3")]
        public string Name3 { get; set; }

        [JsonProperty("address3")]
        public string Address3 { get; set; }

        [JsonProperty("address4")]
        public string Address4 { get; set; }

        [JsonProperty("phone4")]
        public string Phone4 { get; set; }

        [JsonProperty("phone5")]
        public string Phone5 { get; set; }

        [JsonProperty("fax2")]
        public string Fax2 { get; set; }
    }
}
