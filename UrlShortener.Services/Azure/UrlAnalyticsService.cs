using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UAParser;
using UrlShortener.Models.Azure;
using UrlShortener.Repositories;

namespace UrlShortener.Services.Azure
{
    public class UrlAnalyticsService : IUrlAnalyticsService
    {
        private readonly IUrlAnalyticsRepository _urlAnalyticsRepository;
        private readonly ILogger _logger;


        public UrlAnalyticsService(IUrlAnalyticsRepository urlAnalyticsRepository, ILogger logger)
        {
            _urlAnalyticsRepository = urlAnalyticsRepository;
            _logger = logger;
        }


        public async Task<Result<bool>> CreateOrUpdateUrlAnalytics(UrlAnalyticsQueueMessage message)
        {
            try
            {
                if (string.IsNullOrEmpty(message?.UserAgent)) throw new Exception("user agent is empty");
                var ua = Parser.GetDefault()?.Parse(message?.UserAgent);
                var urlAnalytics = new UrlAnalytics
                {
                    Browser = ua?.UserAgent.Family,
                    BrowserVer = ua?.UserAgent.Major,
                    Os = ua?.OS?.Family,
                    OsVer = ua?.OS?.Major,
                    Referrer = message.Referer,
                    PartitionKey= message.ShortUrlCode.Substring(0,3),
                    RowKey = message.ShortUrlCode
                };

                var result = await _urlAnalyticsRepository.CreateOrUpdateUrlAnalytics(urlAnalytics);
                return new Result<bool>
                {
                    Success = true,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} - {ex}");
                return new Result<bool>
                {
                    Success = false,
                    Error = ex.Message
                };
            }
        }

        public async Task<Result<UrlAnalytics>> GetUrlAnalytics(string partitionKey, string rowKey)
        {
            try
            {
                var result = await _urlAnalyticsRepository.GetUrlAnalytics(partitionKey, rowKey);
                return new Result<UrlAnalytics>
                {
                    Success = true,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} - {ex}");
                return new Result<UrlAnalytics>
                {
                    Success = false,
                    Error = ex.Message
                };
            }
        }
    }
}