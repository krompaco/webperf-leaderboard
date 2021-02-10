using System;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Route("api")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private static readonly object WebperfIndexLock = new object();

        private readonly ILogger<ApiController> logger;

        private readonly IConfiguration config;

        private readonly IMemoryCache memoryCache;

        public ApiController(ILogger<ApiController> logger, IConfiguration config, IMemoryCache memoryCache)
        {
            this.logger = logger;
            this.config = config;
            this.memoryCache = memoryCache;
        }

        [HttpGet("webperf-index")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<WebperfIndexResponse> WebperfIndex()
        {
            const string ck = "WebperfIndex";

            // ReSharper disable once InconsistentlySynchronizedField
            if (this.memoryCache.TryGetValue(ck, out WebperfIndexResponse model))
            {
                return model;
            }

            lock (WebperfIndexLock)
            {
                if (this.memoryCache.TryGetValue(ck, out model))
                {
                    return model;
                }

                var dataSource = this.config.GetValue<string>("AppSettings:SqliteDataSource");

                var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = dataSource };

                using var connection = new SqliteConnection(connectionStringBuilder.ConnectionString);
                connection.Open();

                var selectCmd = connection.CreateCommand();
                selectCmd.CommandText =
                    "SELECT t.site_id, t.test_date, t.check_report, t.json_check_data, t.rating, s.title, s.website, t.type_of_test FROM sitetests t INNER JOIN sites s ON s.id = t.site_id WHERE s.active = 1 AND most_recent = 1 AND type_of_test IN (1, 2, 4, 5, 6, 7, 8, 9, 10, 17, 20, 21) ORDER BY site_id, type_of_test";

                var currentSiteId = -1;
                WebperfSite currentSite = null;
                model = new WebperfIndexResponse();

                using var reader = selectCmd.ExecuteReader();
                while (reader.Read())
                {
                    var siteId = reader.GetInt32(0);

                    if (siteId != currentSiteId)
                    {
                        SetRating(currentSite);

                        currentSite = new WebperfSite { Title = reader.GetString(5), Url = reader.GetString(6) };
                        model.Sites.Add(currentSite);
                    }

                    var test = new WebperfTest
                    {
                        TestDate = reader.GetDateTime(1),
                        ReportText = reader.GetString(2),
                        ReportJson = reader.GetString(3),
                        Rating = double.Parse(reader.GetString(4), new CultureInfo("en-US")),
                        TypeOfTest = reader.GetInt32(7),
                        SiteId = siteId,
                    };

                    // ReSharper disable once PossibleNullReferenceException
                    currentSite.Tests.Add(test);

                    currentSiteId = siteId;
                }

                // Rating calculation for last site
                SetRating(currentSite);

                // Sort by rating
                model.Sites = model.Sites
                    .OrderBy(x => x.Tests.Count == 12 ? 0 : 1)
                    .ThenByDescending(x => x.Rating)
                    .ThenByDescending(x => x.Tests.FirstOrDefault()?.Rating ?? 0d)
                    .ToList();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(1));

                this.memoryCache.Set(ck, model, cacheEntryOptions);
            }

            return model;
        }

        [HttpGet("webperf-tests/{id?}")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<WebperfTestsResponse> WebperfTests(int? id)
        {
            if (!id.HasValue)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var ck = "WebperfTests-" + id.Value;

            // ReSharper disable once InconsistentlySynchronizedField
            if (this.memoryCache.TryGetValue(ck, out WebperfTestsResponse model))
            {
                return model;
            }

            lock (WebperfIndexLock)
            {
                if (this.memoryCache.TryGetValue(ck, out model))
                {
                    return model;
                }

                model = new WebperfTestsResponse();

                var dataSource = this.config.GetValue<string>("AppSettings:SqliteDataSource");

                var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = dataSource };

                var currentFrom = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0).AddDays(1);
                var slots = 0;

                using var connection = new SqliteConnection(connectionStringBuilder.ConnectionString);
                connection.Open();

                while (slots < 100)
                {
                    slots++;
                    currentFrom = currentFrom.AddDays(-1);
                    var to = currentFrom.AddDays(1).AddMilliseconds(-1);

                    var selectCmd = connection.CreateCommand();
                    selectCmd.CommandText =
                        $"SELECT t.site_id, t.test_date, t.check_report, t.json_check_data, t.rating, s.title, s.website, t.type_of_test FROM sitetests t INNER JOIN sites s ON s.id = t.site_id WHERE t.most_recent = 0 AND s.active = 1 AND s.id = {id.Value} AND t.type_of_test IN (1, 2, 4, 5, 6, 7, 8, 9, 10, 17, 20, 21) AND t.test_date BETWEEN '{GetDbFormat(currentFrom)}' AND '{GetDbFormat(to)}' ORDER BY t.type_of_test";

                    Console.WriteLine(selectCmd.CommandText);

                    var currentSiteId = -1;
                    WebperfSite currentSite = null;

                    using var reader = selectCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var siteId = reader.GetInt32(0);

                        if (siteId != currentSiteId)
                        {
                            currentSiteId = siteId;
                            currentSite = new WebperfSite { Title = reader.GetString(5), Url = reader.GetString(6) };
                        }

                        var test = new WebperfTest
                        {
                            TestDate = reader.GetDateTime(1),
                            ReportText = reader.GetString(2),
                            ReportJson = reader.GetString(3),
                            Rating = double.Parse(reader.GetString(4), new CultureInfo("en-US")),
                            TypeOfTest = reader.GetInt32(7),
                        };

                        // ReSharper disable once PossibleNullReferenceException
                        currentSite.Tests.Add(test);
                    }

                    if (currentSite == null || currentSite.Tests.Count > 12)
                    {
                        continue;
                    }

                    if (currentSite.Tests.Count < 12)
                    {
                        break;
                    }

                    // Rating calculation for last site
                    SetRating(currentSite);

                    model.TestRuns.Add(currentSite);
                }

                // Sort by rating
                model.TestRuns = model.TestRuns
                    .OrderByDescending(x => x.Tests.Max(y => y.TestDate))
                    .ToList();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(1));

                this.memoryCache.Set(ck, model, cacheEntryOptions);
            }

            return model;
        }

        private static string GetDbFormat(DateTime x)
        {
            return x.ToString("yyyy-MM-dd'T'HH:mm:ss");
        }

        private static void SetRating(WebperfSite site)
        {
            if (site != null && site.Tests.Count == 12)
            {
                var excludedTypes = new[] { 7, 8 };

                site.Rating = site.Tests
                    .Where(x => !excludedTypes.Contains(x.TypeOfTest))
                    .Average(x => x.Rating);
            }
        }
    }
}
