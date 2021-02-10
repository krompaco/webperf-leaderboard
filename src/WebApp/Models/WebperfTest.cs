using System;
using Newtonsoft.Json;

namespace WebApp.Models
{
    public class WebperfTest
    {
        [JsonProperty("testDate")]
        public DateTime TestDate { get; set; }

        [JsonProperty("reportText")]
        public string ReportText { get; set; }

        [JsonProperty("reportJson")]
        public string ReportJson { get; set; }

        [JsonProperty("rating")]
        public double Rating { get; set; }

        [JsonProperty("typeOfTest")]
        public int TypeOfTest { get; set; }

        [JsonProperty("siteId")]
        public int SiteId { get; set; }
    }
}
