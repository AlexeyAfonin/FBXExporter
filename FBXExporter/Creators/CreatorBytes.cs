using System.IO;

namespace ANYTY.FBXExporter.Creators
{
    public static class CreatorBytes
    {
        public static byte[] Generate(string pathObject)
        {
            return File.ReadAllBytes(pathObject as string);
        }

        public static byte[] GenerateAndSave(string pathObject, string savePath)
        {
            var bytes = Generate(pathObject);
            File.WriteAllBytes(savePath, bytes);
            return bytes;
        }
    }
}