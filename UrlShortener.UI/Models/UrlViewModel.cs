using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace UrlShortener.UI.Models
{
    public class UrlViewModel
    {
        [Required]
        [DisplayName("Url To Shorten")]
        public string LongUrl { get; set; }

        [DisplayName("Short Url")]
        public string ShortUrl { get; set; }

        public string UserId { get; set; }
        public string Error { get; set; }
    }
}