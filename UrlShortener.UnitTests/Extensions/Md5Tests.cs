using UrlShortener.Extensions.Md5;
using Xunit;

namespace UrlShortener.UnitTests.Extensions
{
    public class Md5Tests
    {
        [Theory]
        [InlineData("", "d41d8cd98f00b204e9800998ecf8427e")]
        [InlineData("md5", "1bc29b36f623ba82aaf6724fd3b16718")]
        [InlineData("The quick brown fox jumps over the lazy dog. 1234567890", "bfb85e401a205cde01d17164bd3de689")]
        [InlineData("Detta är ett prov", "06f21703bc9687dab4ce078e3fd946da")]
        public void Tests(string input, string output)
        {
            Assert.Equal(output, input.GetMd5Hash());
        }
    }
}