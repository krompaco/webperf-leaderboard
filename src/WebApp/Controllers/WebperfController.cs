using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WebApp.Services.Generated;

namespace WebApp.Controllers
{
    public class WebperfController : Controller
    {
        private readonly ILogger<WebperfController> logger;

        private readonly IConfiguration config;

        public WebperfController(ILogger<WebperfController> logger, IConfiguration config)
        {
            this.logger = logger;
            this.config = config;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // We do it API first and call our own endpoint using the generated nswag ApiClient
            var currentUrl = this.Request.GetEncodedUrl();
            var currentUri = new Uri(currentUrl);

            using var httpClientHandler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            };
            using var client = new HttpClient(httpClientHandler);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var apiClient = new ApiClient(currentUri.GetLeftPart(UriPartial.Authority), client);
            var model = await apiClient.WebperfIndexAsync().ConfigureAwait(false);

            return this.View(model);
        }
    }
}
