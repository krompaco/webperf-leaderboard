using System.Collections.Generic;
using Newtonsoft.Json;

namespace WebApp.Shared
{
    public class TestTypes
    {
        public TestTypes()
        {
            this.AllHeadings = new[] { "Lighthouse Performance", "404 page", "Lighthouse SEO", "Lighthouse Best Practices", "W3C HTML", "W3C CSS", "Lighthouse PWA", "Standard files", "Lighthouse A11y", "Sitespeed.io", "Yellow Lab Tools", "Webbkoll", "HTTP & Tech" };
            this.ExcludedTypes = new[] { 7, 8 };
            this.Count = this.AllHeadings.Length;
        }

        public string[] AllHeadings { get; private set; }

        public int[] ExcludedTypes { get; private set; }

        public int Count { get; private set; }
    }
}
