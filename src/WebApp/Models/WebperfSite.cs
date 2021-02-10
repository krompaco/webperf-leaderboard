using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WebApp.Models
{
    public class WebperfSite
    {
        public WebperfSite()
        {
            this.Tests = new List<WebperfTest>();
        }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("rating")]
        public double Rating { get; set; }

        [JsonProperty("tests")]
        public List<WebperfTest> Tests { get; set; }

        [JsonProperty("siteId")]
        public int SiteId { get; set; }
    }
}
