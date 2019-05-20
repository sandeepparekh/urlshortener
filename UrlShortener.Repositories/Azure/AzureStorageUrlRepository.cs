using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;
using UrlShortener.Models.Azure;

namespace UrlShortener.Repositories.Azure
{
    public class AzureStorageUrlRepository : IUrlRepository
    {
        private readonly CloudTable _urlRedirectTable;
        private readonly CloudTable _urlReadTable;
        private static readonly string UrlRedirectTableName = "UrlRedirect";
        private static readonly string UrlReadTableName = "UrlRead";

        public AzureStorageUrlRepository(string storageConnString)
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(storageConnString);
            var cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
            _urlRedirectTable = cloudTableClient.GetTableReference(UrlRedirectTableName);
            _urlReadTable = cloudTableClient.GetTableReference(UrlReadTableName);
        }

        public async Task<bool> CreateUrl(Url redirectOptimizedEntity, Url readOptimizedEntity)
        {
            await _urlRedirectTable.CreateIfNotExistsAsync();
            await _urlReadTable.CreateIfNotExistsAsync();
            await _urlRedirectTable.ExecuteAsync(TableOperation.Insert(redirectOptimizedEntity));
            await _urlReadTable.ExecuteAsync(TableOperation.Insert(readOptimizedEntity));
            return true;
        }

        public async Task<bool> RedirectOptimizedEntityExists(Url redirectOptimizedEntity)
        {
            var result = await _urlRedirectTable.ExecuteAsync(TableOperation.Retrieve<Url>(redirectOptimizedEntity.PartitionKey,
                redirectOptimizedEntity.RowKey));
            return result.Result != null &&
                   string.Equals((result.Result as Url)?.LongUrl, redirectOptimizedEntity.LongUrl);
        }

        public async Task<bool> ReadOptimizedEntityExists(Url readOptimizedEntity)
        {
            var result = await _urlReadTable.ExecuteAsync(TableOperation.Retrieve<Url>(readOptimizedEntity.PartitionKey,
                readOptimizedEntity.RowKey));
            return result.Result != null &&
                   string.Equals((result.Result as Url)?.LongUrl, readOptimizedEntity.LongUrl);
        }

        public async Task<Url> GetRedirectOptimizedUrl(string partitionKey, string rowKey)
        {
            var tableResult = await _urlRedirectTable.ExecuteAsync(TableOperation.Retrieve<Url>(partitionKey, rowKey));
            return tableResult.Result as Url;
        }

        public async Task<Url> GetReadOptimizedUrl(string partitionKey, string rowKey)
        {
            var tableResult = await _urlReadTable.ExecuteAsync(TableOperation.Retrieve<Url>(partitionKey, rowKey));
            return tableResult.Result as Url;
        }

        public async Task<UrlListResult> GetUrls(string userId, TableContinuationToken token = null)
        {
            var query = new TableQuery<Url>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, userId)
                );
            var result = await _urlReadTable.ExecuteQuerySegmentedAsync(query, token);
            return new UrlListResult
            {
                Token = result.ContinuationToken,
                Urls = result.Results
            };
        }

        public async Task<bool> DeleteUrl(string shortUrlCode, string userId)
        {
            var redirOptEnt = await GetRedirectOptimizedUrl(shortUrlCode.Substring(0, 3), shortUrlCode);
            var readOptEnt = await GetReadOptimizedUrl(userId, shortUrlCode);
            await _urlRedirectTable.ExecuteAsync(TableOperation.Delete(redirOptEnt));
            await _urlReadTable.ExecuteAsync(TableOperation.Delete(readOptEnt));
            return true;
        }
    }
}