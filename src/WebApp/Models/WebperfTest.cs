using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    }
}
