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
using UrlShortener.Services.Memory;
using UrlShortener.Services.Redis;

namespace UrlShortener.Redirector
{
    public class Startup
    {
        private IHostingEnvironment _environment;
        private static readonly IAppSettings AppSettings = new UrlShortenerSettings();
        private IConfiguration _configuration;

        public Startup(IHostingEnvironment env, IConfiguration configuration)
        {
            _environment = env;
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            _configuration.GetSection("AppSettings").Bind(AppSettings);
            services.AddSingleton(AppSettings);

            if (_environment.IsDevelopment())
            {
                services.AddSingleton<ICacheService, MemoryCacheService>();
            }
            else
            {
                services.AddTransient<ICacheService>(r => new RedisCacheService(AppSettings.CacheConnectionString));
            }
            services.AddTransient<IUrlRepository>(r => new UrlRepository(AppSettings.DbConnectionString));
            services.AddTransient<IUrlShorteningService, UrlShorteningService>();
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