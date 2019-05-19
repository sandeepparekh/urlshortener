using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UrlShortener.Models.Common;
using UrlShortener.Redirector.Middleware;
using UrlShortener.Repositories;
using UrlShortener.Repositories.Azure;
using UrlShortener.Services;
using UrlShortener.Services.Azure;

namespace UrlShortener.Redirector
{
    public class Startup
    {
        private static readonly IAppSettings AppSettings = new UrlShortenerSettings();
        static Startup()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true);

            var configuration = builder.Build();
            configuration.GetSection("AppSettings").Bind(AppSettings);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(AppSettings);
            services.AddTransient<IUrlRepository>(r => new AzureStorageUrlRepository(AppSettings.DbConnectionString));
            services.AddTransient<IUrlShortService, AzureStorageUrlShortService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMiddleware<RedirectMiddleware>();
            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
