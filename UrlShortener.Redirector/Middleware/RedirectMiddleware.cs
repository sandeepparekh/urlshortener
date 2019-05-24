using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using UrlShortener.Models.Azure;
using UrlShortener.Models.Common;
using UrlShortener.Services;

namespace UrlShortener.Redirector.Middleware
{
    public class RedirectMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IUrlShorteningService _urlShortService;
        private readonly IAppSettings _settings;
        private readonly ILogger _logger;
        private readonly CloudQueue _urlAnalyticsQueue;
        private readonly string UrlAnalyticsQueueName = "urlanalytics";

        public RedirectMiddleware(RequestDelegate next
            , IUrlShorteningService urlShortService
            , IAppSettings appSettings
            , ILogger<RedirectMiddleware> logger)
        {
            _next = next;
            _urlShortService = urlShortService;
            _settings = appSettings;
            _logger = logger;
            _urlAnalyticsQueue = CloudStorageAccount
                .Parse(_settings.DbConnectionString)
                ?.CreateCloudQueueClient()
                ?.GetQueueReference(UrlAnalyticsQueueName);
        }

        public async Task Invoke(HttpContext context)
        {
            var path = context.Request.Path.ToUriComponent();
            if (path?.Length - 1 == _settings.ShortUrlCodeLength && path != "/")
            {
                var shortUrlCode = path.Substring(1);
                _logger.LogDebug($"Trying to redirect {path}");
                var result = await _urlShortService.GetLongUrl(shortUrlCode);
                if (result != null && result.Success)
                {
                    // send message to queue for url analytics
                    await _urlAnalyticsQueue.AddMessageAsync(new CloudQueueMessage(JsonConvert.SerializeObject(
                        new UrlAnalyticsQueueMessage
                        {
                            ShortUrlCode = shortUrlCode,
                            Referer = context.Request.Headers.ContainsKey("Referer") ? context.Request.Headers["Referer"].ToString() : "",
                            UserAgent = context.Request.Headers.ContainsKey("user-agent") ? context.Request.Headers["user-agent"].ToString() : ""
                        })));

                    _logger.LogDebug($"Redirecting {shortUrlCode} to {result.Data}");
                    context.Response.Redirect(result.Data, true);
                    return;
                }
                _logger.LogError($"Short Url Code:{shortUrlCode}  not found.");
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }
            context.Response.Redirect(_settings.UiDomain);
        }
    }
}