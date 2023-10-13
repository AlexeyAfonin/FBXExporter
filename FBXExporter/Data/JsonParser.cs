using Newtonsoft.Json;
using System.IO;

namespace ANYTY.FBXExporter.Data
{
    public static class JsonParser
    {
        public static T JsonToClass<T>(string json) =>
            JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

        public static string ClassToJson<T>(T data) =>
            JsonConvert.SerializeObject(data, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

        public static void WriteFile(this string line, string directory, string name, bool isAppend)
        {
            using StreamWriter sw = new($"{directory}/{name}.json", isAppend);
            sw.WriteLine(line);
            sw.Close();
        }

        public static string ReadFile(this string path, bool isReplaceTransfer = false, bool isReplaceTabulation = false, bool isReplaceDoubleSpace = false, bool isReplaceSpace = false)
        {
            using StreamReader sr = new(path);
            var json = sr.ReadToEnd();
            json.EditJsonFile(isReplaceTransfer, isReplaceTabulation, isReplaceDoubleSpace, isReplaceSpace);
            sr.Close();
            return json;
        }

        public static string EditJsonFile(this string json, bool isReplaceTransfer = false, bool isReplaceTabulation = false, bool isReplaceDoubleSpace = false, bool isReplaceSpace = false)
        {
            if (isReplaceTransfer)
            {
                json = json
                    .Replace("\n", string.Empty)
                    .Replace("\r", string.Empty);
            }

            if (isReplaceTabulation)
            {
                json = json
                    .Replace("\t", string.Empty);
            }

            if (isReplaceDoubleSpace)
            {
                json = json
                    .Replace("  ", string.Empty);
            }

            if (isReplaceSpace)
            {
                json = json
                    .Replace(" ", string.Empty);
            }

            return json;
        }
    }
}