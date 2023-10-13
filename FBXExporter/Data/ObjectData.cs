using UnityEngine;

namespace ANYTY.FBXExporter.Data
{
    [System.Serializable]
    public sealed class ObjectData
    {
        [SerializeField] public string Name = "";
        [SerializeField] public string Type = "";
        [SerializeField] public string Position = "0,0,0";
        [SerializeField] public string Rotation = "0,0,0";
        [SerializeField] public string Scale = "1,1,1";
        [SerializeField] public string ModelHashCode = "";
    }
}