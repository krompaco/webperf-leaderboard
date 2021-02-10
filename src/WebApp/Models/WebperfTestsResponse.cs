using System.Collections.Generic;
using Newtonsoft.Json;

namespace WebApp.Models
{
    public class WebperfTestsResponse
    {
        public WebperfTestsResponse()
        {
            this.TestRuns = new List<WebperfSite>();
        }

        [JsonProperty("testRuns")]
        public List<WebperfSite> TestRuns { get; set; }
    }
}
