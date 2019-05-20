using System;
using System.Linq;
using System.Text;

namespace UrlShortener.Extensions.BaseX
{
    public static class BaseX
    {
        public static string ToBaseX(this string str, char[] alphabets)
        {
            var binaryBits = str.ToBinary();

            var baseX = string.Empty;

            var bitsTaken = 0;
            var bitsToTake = 24;
            const byte sixbits = 6;
            while (bitsTaken < binaryBits.Length)
            {
                var bitsOf24 = binaryBits.Skip(bitsTaken).Take(bitsToTake).ToList();

                var sixBitsTaken = 0;
                while (sixBitsTaken < bitsOf24.Count)
                {
                    var chunk = bitsOf24.Skip(sixBitsTaken).Take(sixbits).ToList();
                    var chunkString = string.Join("", chunk);

                    if (chunkString.Length < 6)
                    {
                        chunkString = chunkString.PadRight(6, '0');
                    }

                    var convertedInt = Convert.ToInt32(chunkString, 2);
                    baseX += alphabets[convertedInt];
                    sixBitsTaken += sixbits;
                }

                bitsTaken += bitsToTake;
            }

            for (int i = 0; i < binaryBits.Length % 3; i++)
            {
                baseX += "=";
            }

            return baseX;
        }

        private static string ToBinary(this string str)
        {
            var byteData = Encoding.ASCII.GetBytes(str);
            return string.Join("", byteData.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')));
        }
    }
}