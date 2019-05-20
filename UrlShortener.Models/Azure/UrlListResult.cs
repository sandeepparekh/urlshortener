using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;

namespace UrlShortener.Models.Azure
{
    public class UrlListResult
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public List<Url> Urls { get; set; }
        public TableContinuationToken Token { get; set; }
    }
}