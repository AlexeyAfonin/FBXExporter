using System;
using System.Text;

namespace ANYTY.FBXExporter.Conversions
{
    public static class ByteConversion
    {
        public static string ByteArrayToHexString(byte[] byteArray)
        {
            var hexBuilder = new StringBuilder(byteArray.Length * 2);
            foreach (var b in byteArray)
            {
                hexBuilder.Append(b.ToString("X2"));
            }

            return hexBuilder.ToString();
        }

        public static byte[] HexStringToByteArray(string hexString)
        {
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException("Hex string must have an even number of characters.");
            }

            var byteArray = new byte[hexString.Length / 2];
            for (int i = 0; i < byteArray.Length; i++)
            {
                var byteValue = hexString.Substring(i * 2, 2);
                byteArray[i] = Convert.ToByte(byteValue, 16);
            }

            return byteArray;
        }
    }
}