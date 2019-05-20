using UrlShortener.Extensions.BaseX;
using Xunit;

namespace UrlShortener.UnitTests.Extensions
{
    public class BasexTests
    {
        private static readonly char[] AlphabetSet = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '+', '/' };

        [Theory]
        [InlineData("", "")]
        [InlineData("base64", "YmFzZTY0")]
        [InlineData("The quick brown fox jumps over the lazy dog. 1234567890", "VGhlIHF1aWNrIGJyb3duIGZveCBqdW1wcyBvdmVyIHRoZSBsYXp5IGRvZy4gMTIzNDU2Nzg5MA==")]
        [InlineData("dd23d036063c1362ad9563db1834eb94", "ZGQyM2QwMzYwNjNjMTM2MmFkOTU2M2RiMTgzNGViOTQ=")]
        public void Base64Tests(string input, string output)
        {
            Assert.Equal(output, input.ToBaseX(AlphabetSet));
        }
    }
}