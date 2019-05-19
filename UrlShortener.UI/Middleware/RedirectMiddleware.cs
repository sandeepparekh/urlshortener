using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using UrlShortener.Services;

namespace UrlShortener.UI.Middleware
{

    public class RedirectMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IUrlShortService _urlShortService;
        public RedirectMiddleware(RequestDelegate next, IUrlShortService urlShortService)
        {
            _next = next;
            _urlShortService = urlShortService;
        }

        public async Task Invoke(HttpContext context)
        {
            var path = context.Request.Path.ToUriComponent();
            if (path?.Length == 7 && path != "/")
            {
                var result = await _urlShortService.GetUrl(path.Substring(1));
                if (result != null && result.Success)
                {
                    context.Response.Redirect(result.Data.LongUrl, true);
                    return;
                }

                context.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return;
            }
            //Let the next middleware (MVC routing) handle the request
            //In case the path was updated, the MVC routing will see the updated path
            await _next.Invoke(context);

        }
    }
}
