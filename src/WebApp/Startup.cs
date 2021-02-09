using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace WebApp
{
    public class Startup
    {
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
            services.AddControllersWithViews();

            // Blazor
            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Webperf Core Leaderboard", Version = "v1" });

                var xmlDocumentationFile = this.Configuration.GetValue<string>("AppSettings:XmlDocumentationFile");
                c.IncludeXmlComments(xmlDocumentationFile);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // ReSharper disable once UnusedMember.Global
        #pragma warning disable CA1822 // Mark members as static
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        #pragma warning restore CA1822 // Mark members as static
        {
            if (env.IsDevelopment())
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
                endpoints.MapFallbackToPage("", "/_Host");
            });
        }
    }
}
