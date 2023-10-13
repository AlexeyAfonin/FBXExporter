using System.IO;
using System.IO.Compression;
using ANYTY.FBXExporter.Conversions;

namespace ANYTY.FBXExporter
{
    public static class HexCompression
    {
        public static byte[] CompressHex(string hexString)
        {
            var byteArray = ByteConversion.HexStringToByteArray(hexString);

            using (var memoryStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
                {
                    gzipStream.Write(byteArray, 0, byteArray.Length);
                }

                return memoryStream.ToArray();
            }
        }

        public static string DecompressHex(byte[] compressedBytes)
        {
            using (var memoryStream = new MemoryStream(compressedBytes))
            {
                using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    using (var decompressedStream = new MemoryStream())
                    {
                        gzipStream.CopyTo(decompressedStream);
                        var decompressedBytes = decompressedStream.ToArray();
                        return ByteConversion.ByteArrayToHexString(decompressedBytes);
                    }
                }
            }
        }
    }
}