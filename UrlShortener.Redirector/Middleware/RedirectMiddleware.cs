using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using UrlShortener.Models.Common;
using UrlShortener.Services;

namespace UrlShortener.Redirector.Middleware
{
    public class RedirectMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IUrlShortService _urlShortService;
        private readonly IAppSettings _settings;
        private readonly ILogger _logger;
        public RedirectMiddleware(RequestDelegate next
            , IUrlShortService urlShortService
            , IAppSettings appSettings,
            ILogger<RedirectMiddleware> logger)
        {
            _next = next;
            _urlShortService = urlShortService;
            _settings = appSettings;
            _logger = logger;
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
                    _logger.LogDebug($"Redirecting {shortUrlCode} to {result.Data}");
                    context.Response.Redirect(result.Data, true);
                    return;
                }
                _logger.LogError($"Short Url Code:{shortUrlCode}  not found.");
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }
            //Let the next middleware (MVC routing) handle the request
            //In case the path was updated, the MVC routing will see the updated path
            await _next.Invoke(context);
        }
    }
}
