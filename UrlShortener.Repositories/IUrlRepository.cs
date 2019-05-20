using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;
using UrlShortener.Models.Azure;

namespace UrlShortener.Repositories
{
    public interface IUrlRepository
    {
        Task<bool> CreateUrl(Url redirectOptimizedEntity, Url readOptimizedEntity);

        Task<bool> RedirectOptimizedEntityExists(Url redirectOptimizedEntity);

        Task<bool> ReadOptimizedEntityExists(Url readOptimizedEntity);

        Task<Url> GetRedirectOptimizedUrl(string partitionKey, string rowKey);

        Task<Url> GetReadOptimizedUrl(string partitionKey, string rowKey);

        Task<UrlListResult> GetUrls(string userId, TableContinuationToken token = null);

        Task<bool> DeleteUrl(string shortUrlCode, string userId);
    }
}