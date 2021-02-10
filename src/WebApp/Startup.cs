using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using WebApp.Services.Generated;

namespace WebApp
{
    public class Startup
    {
        private IWebHostEnvironment env;

        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        #pragma warning disable CA1822 // Mark members as static
        public void ConfigureServices(IServiceCollection services)
        #pragma warning restore CA1822 // Mark members as static
        {
            // For API controller
            services
                .AddMvcCore()
                .AddApiExplorer()
                .AddDataAnnotations()
                .AddFormatterMappings();

            // For Blazor Server setup
            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Webperf Core Leaderboard", Version = "v1" });

                var xmlPath = Path.Combine(this.env.ContentRootPath, "WebApp.xml");
                c.IncludeXmlComments(xmlPath);
            });

            var apiBaseUrl = this.Configuration.GetValue<string>("AppSettings:ApiBaseUrl");

            services
                .AddHttpClient<IApiClient, ApiClient>(c =>
                {
                    c.BaseAddress = new Uri(apiBaseUrl);
                    c.DefaultRequestHeaders.Accept.Clear();
                    c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                })
                .ConfigurePrimaryHttpMessageHandler(messageHandler =>
                {
                    var handler = new HttpClientHandler();

                    if (handler.SupportsAutomaticDecompression)
                    {
                        handler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                    }

                    return handler;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // ReSharper disable once UnusedMember.Global
        #pragma warning disable CA1822 // Mark members as static
        public void Configure(IApplicationBuilder app, IWebHostEnvironment environment)
        #pragma warning restore CA1822 // Mark members as static
        {
            this.env = environment;

            if (this.env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // TODO: Change later
                app.UseDeveloperExceptionPage();

                // The default value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Webperf Core Leaderboard API v1");
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();

                // To not disturb API URL and since we only got a page on / which is = ""
                endpoints.MapFallbackToPage(string.Empty, "/_Host");
            });
        }
    }
}
