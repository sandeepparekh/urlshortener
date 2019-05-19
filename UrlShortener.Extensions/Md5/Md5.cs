using System.Security.Cryptography;
using System.Text;

namespace UrlShortener.Extensions.Md5
{
    public static class Md5Extensions
    {
        public static string GetMd5Hash(this string str)
        {
            using (MD5 md5 = MD5.Create())
            {
                var data = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
                var sb = new StringBuilder();
                foreach (var t in data)
                {
                    sb.Append(t.ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}
