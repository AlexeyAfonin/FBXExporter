using System.IO;
using ANYTY.FBXExporter.Conversions;

namespace ANYTY.FBXExporter.Creators
{
    public static class CreatorHashCode
    {
        public static string Generate(string hexCode)
        {
            return HashConversion.HexStringToHash(hexCode);
        }

        public static string GenerateAndSave(string hexCode, string savePath)
        {
            var hashCode = Generate(hexCode);
            File.WriteAllText(savePath, hashCode);
            return hashCode;
        }
    }
}