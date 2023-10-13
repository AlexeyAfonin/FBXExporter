using System.Security.Cryptography;
using System.Text;

namespace ANYTY.FBXExporter.Conversions
{
    public static class HashConversion
    {
        public static string HexStringToHash(string hexString)
        {
            var byteArray = ByteConversion.HexStringToByteArray(hexString);

            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(byteArray);
                return ByteConversion.ByteArrayToHexString(hashBytes);
            }
        }

        public static string HashToHexString(byte[] hashBytes)
        {
            var hexBuilder = new StringBuilder(hashBytes.Length * 2);

            foreach (var b in hashBytes)
            {
                hexBuilder.Append(b.ToString("x2"));
            }

            return hexBuilder.ToString();
        }
    }
}