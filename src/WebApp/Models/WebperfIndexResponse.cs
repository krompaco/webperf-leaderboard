using System.Collections.Generic;
using Newtonsoft.Json;

namespace WebApp.Models
{
    public class WebperfIndexResponse
    {
        public WebperfIndexResponse()
        {
            this.Sites = new List<WebperfSite>();
        }

        [JsonProperty("sites")]
        public List<WebperfSite> Sites { get; set; }
    }
}
