using System.Linq;
using ANYTY.FBXExporter.Data;

namespace ANYTY.FBXExporter.Creators
{
    public static class CreatorDataJson
    {
        public static string Generate<T>(T data)
        {
            return JsonParser.ClassToJson(data);
        }

        public static string GenerateAndSave<T>(T data, string savePath)
        {
            var json = Generate<T>(data);
            var name = savePath.Split('/').Last();
            var directory = savePath.Replace(name, string.Empty);
            json.WriteFile(directory, name, false);
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
            return json;
        }
    }
}
