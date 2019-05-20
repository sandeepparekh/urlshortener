﻿using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using UrlShortener.Models.Azure;

namespace UrlShortener.Services
{
    public interface IUrlShortService
    {
        Task<Result<string>> CreateUrl(string longUrl, string userId = "anon");
        Task<Result<string>> GetLongUrl(string shortUrlCode);
        Task<UrlListResult> GetUrls(string userId, TableContinuationToken token = null);
        Task<Result<bool>> DeleteUrl(string shortUrlCode, string userId);
    }
}
