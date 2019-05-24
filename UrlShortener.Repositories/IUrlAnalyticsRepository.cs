using System.Threading.Tasks;
using UrlShortener.Models.Azure;

namespace UrlShortener.Repositories
{
    public interface IUrlAnalyticsRepository
    {
        Task<bool> CreateOrUpdateUrlAnalytics(UrlAnalytics urlAnalytics);
        Task<UrlAnalytics> GetUrlAnalytics(string partitionKey, string rowKey);
    }
}