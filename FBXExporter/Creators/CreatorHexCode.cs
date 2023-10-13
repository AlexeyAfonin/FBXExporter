using System.IO;
using ANYTY.FBXExporter.Conversions;

namespace ANYTY.FBXExporter.Creators
{
    public static class CreatorHexCode
    {
        public static string Generate(byte[] bytes)
        {
            var compressedBytes = HexCompression.CompressHex(ByteConversion.ByteArrayToHexString(bytes));
            return ByteConversion.ByteArrayToHexString(compressedBytes);
        }

        public static string GenerateAndSave(byte[] bytes, string savePath)
        {
            var hexCode = Generate(bytes);
            File.WriteAllText(savePath, hexCode);
            return hexCode;
        }
    }
}