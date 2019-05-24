using System.Threading.Tasks;
using UrlShortener.Models.Azure;

namespace UrlShortener.Services
{
    public interface IUrlAnalyticsService
    {
        Task<Result<bool>> CreateOrUpdateUrlAnalytics(UrlAnalyticsQueueMessage message);
        Task<Result<UrlAnalytics>> GetUrlAnalytics(string partitionKey, string rowKey);
    }
}
