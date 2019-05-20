using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UrlShortener.Models.Azure;
using UrlShortener.Models.Common;
using UrlShortener.Services;
using UrlShortener.UI.Models;

namespace UrlShortener.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUrlShortService _urlShortService;
        private readonly IAppSettings _settings;

        public HomeController(IUrlShortService urlShortService, IAppSettings settings)
        {
            _urlShortService = urlShortService;
            _settings = settings;
        }

        public IActionResult Index()
        {
            return View(new UrlViewModel());
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ShortenUrl(UrlViewModel model)
        {
            if (ModelState.IsValid)
            {
                Result<string> result;
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                    result = await _urlShortService.CreateUrl(model.LongUrl);
                else
                {
                    result = await _urlShortService.CreateUrl(model.LongUrl, userId);
                }
                if (!result.Success)
                {
                    model.Error = result.Error;
                }
                model.ShortUrl = $"{_settings.ShortUrlDomain}/{result.Data}";
                return View("Index", model);
            }

            model.Error = string.Join(Environment.NewLine,
                ModelState.Values.SelectMany(m => m.Errors).Select(x => x.ErrorMessage));
            return View("Index", model);
        }

        [Authorize]
        public async Task<IActionResult> List()
        {
            var result = await _urlShortService.GetUrls(GetUserId());
            return View("List", result);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private string GetUserId()
        {
            return User.Identity.IsAuthenticated
                ? User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
                : "";
        }
    }
}