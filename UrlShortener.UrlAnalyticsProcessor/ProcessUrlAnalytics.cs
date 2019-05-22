using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using UrlShortener.Models.Azure;
using UrlShortener.Repositories.Azure;
using UrlShortener.Services;
using UrlShortener.Services.Azure;

namespace UrlShortener.UrlAnalyticsProcessor
{
    public static class ProcessUrlAnalytics
    {
        private static IUrlAnalyticsService _urlAnalyticsService;

        static ProcessUrlAnalytics()
        {
            _urlAnalyticsService = new UrlAnalyticsService(new UrlAnalyticsRepository(Environment.GetEnvironmentVariable("AzureWebJobsStorage"))
                , new LoggerFactory().CreateLogger("ProcessUrlAnalytics"));
        }

        [FunctionName("ProcessUrlAnalytics")]
        public static async Task Run([QueueTrigger("urlanalytics", Connection = "")]UrlAnalyticsQueueMessage message
            , ILogger log)
        {
            log.LogInformation($"ProcessUrlAnalytics triggerred for : {message}");
            await _urlAnalyticsService.CreateOrUpdateUrlAnalytics(message);
        }
    }
}
