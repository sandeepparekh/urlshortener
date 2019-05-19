using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;

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
