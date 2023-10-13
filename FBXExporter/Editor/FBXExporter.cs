using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Autodesk.Fbx;
using UnityEditor.Formats.Fbx.Exporter;
using ANYTY.FBXExporter.Data;
using ANYTY.FBXExporter.Creators;

namespace ANYTY.FBXExporter
{
    public static class FBXExporter
    {
        public sealed class ExportData
        {
            public byte[] Bytes;
            public string HexCode;
            public string HashCode;
            public ObjectData ObjectData;
            public string JsonData;

            public ExportData() { }

            public ExportData(byte[] bytes, string hexCode, string hashCode, ObjectData objectData, string jsonData)
            {
                Bytes = bytes;
                HexCode = hexCode;
                HashCode = hashCode;
                ObjectData = objectData;
                JsonData = jsonData;
            }
        }

        private static Object[] _selectedObjects;
        private static string _exportPath;

        private static bool _isExportTextures = false;
        private static bool _isExportDataFiles = true;

        private static FbxManager _scene;
        private static FbxExporter _exporter;

        private static List<Mesh> _exportedMeshes = new();

        public static ExportData GenerateObjectData(this Object obj, string exportPath)
        {
            _exportPath = exportPath;

            ExportData exportData;

            Initialize();

            if (!IsInitialized())
            {
                exportData = Export(obj, false);
            }
            else
            {
                //TODO: add logic show pop-up error
                return null;
            }

            Clear();
            return exportData;
        }

        public static void GenerateAndSaveObjectData(Object[] selectedObjects, string exportPath, bool isExportTexture, bool isIncludeDataFiles)
        {
            _selectedObjects = selectedObjects;
            _exportPath = exportPath;
            _isExportTextures = isExportTexture;
            _isExportDataFiles = isIncludeDataFiles;

            Initialize();

            if (!IsInitialized())
            {
                _exportedMeshes.Clear();

                foreach (var obj in _selectedObjects)
                {
                    if ((obj as GameObject).TryGetComponent(out Mesh _))
                    {
                        if (!(obj as GameObject).IsMeshDuplicate(_exportedMeshes, out var mesh))
                        {
                            Export(obj, true);
                            _exportedMeshes.Add(mesh);
                        }
                    }
                    else
                    {
                        Export(obj, true);
                    }
                }

                Debug.Log($"Export complete: {_exportPath}/FBXExporter/");
            }
            else
            {
                Debug.LogError("Failed to initialize FBX exporter.");
            }

            Clear();
        }

        private static void Initialize()
        {
            _scene = FbxManager.Create();
            _exporter = FbxExporter.Create(_scene, "Exporter");
        }

        private static void Clear()
        {
            _exporter.Destroy();
            _scene.Destroy();
        }

        private static bool IsInitialized()
        {
            return !_exporter.Initialize(_exportPath, -1, _scene.GetIOSettings());
        }

        private static bool IsMeshDuplicate(this GameObject go, List<Mesh> listMeshes, out Mesh mesh)
        {
            mesh = go.GetComponent<MeshFilter>().mesh;

            foreach (var meshFromList in listMeshes)
            {
                // Compare vertex count
                if (mesh.vertexCount != meshFromList.vertexCount)
                    continue;

                // Compare vertices
                bool areVerticesEqual = true;
                for (int i = 0; i < mesh.vertexCount; i++)
                {
                    if (mesh.vertices[i] != meshFromList.vertices[i])
                    {
                        areVerticesEqual = false;
                        break;
                    }
                }
                if (!areVerticesEqual)
                    continue;

                // Compare triangle count
                if (mesh.triangles.Length != meshFromList.triangles.Length)
                    continue;

                // Compare triangles
                bool areTrianglesEqual = true;
                for (int i = 0; i < mesh.triangles.Length; i++)
                {
                    if (mesh.triangles[i] != meshFromList.triangles[i])
                    {
                        areTrianglesEqual = false;
                        break;
                    }
                }
                if (!areTrianglesEqual)
                    continue;

                // The meshes are equal
                return true;
            }

            // No duplicate mesh was found
            return false;
        }


        private static ExportData Export(Object obj, bool isSaveFiles)
        {
            var directory = $"{_exportPath}/FBXExporter/{obj.name}";
            var nameFbx = $"{obj.name}.fbx";
            var pathFbx = $"{directory}/{nameFbx}";

            var go = obj as GameObject;

            if (go.TryGetComponent(out Renderer renderer))
            {
                var originalMaterial = renderer.sharedMaterial;
                var originalMaterialName = originalMaterial.name;
                var newMaterial = Object.Instantiate(originalMaterial);
                newMaterial.name = originalMaterialName;
                renderer.material = newMaterial;

                ModelExporter.ExportObject(pathFbx, obj);

                Object.DestroyImmediate(newMaterial, true);
                renderer.material = originalMaterial;
            }
            else
            {
                ModelExporter.ExportObject(pathFbx, obj);
            }

            if (_isExportTextures)
            {
                ExportTextures(go.transform, directory);
            }

            var exportData = _isExportDataFiles ?
                ExportDataFiles(directory, obj.name, obj as GameObject, isSaveFiles) :
                null;

            if (!isSaveFiles)
            {
                Directory.Delete($"{_exportPath}/FBXExporter", true);
                AssetDatabase.Refresh();
            }

            return exportData;
        }

        private static void ExportTextures(Transform root, string exportPath)
        {
            var meshRenderer = root.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                foreach (var material in meshRenderer.sharedMaterials)
                {
                    var texture = (Texture2D)material.GetTexture("_BaseMap");
                    if (texture != null)
                    {
                        var newTexture = new Texture2D(texture.width, texture.height, TextureFormat.ARGB32, false);
                        newTexture.SetPixels(0, 0, texture.width, texture.height, texture.GetPixels());
                        newTexture.Apply();
                        var bytes = newTexture.EncodeToPNG();
                        var fileName = "Base_" + root.name + "_" + material.name + ".png";
                        File.WriteAllBytes(Path.Combine(exportPath, fileName), bytes);
                        AssetDatabase.Refresh();
                    }

                    var metallicMap = (Texture2D)material.GetTexture("_MetallicGlossMap");
                    if (metallicMap != null)
                    {
                        var newTexture = new Texture2D(metallicMap.width, metallicMap.height, TextureFormat.ARGB32, false);
                        newTexture.SetPixels(0, 0, metallicMap.width, metallicMap.height, metallicMap.GetPixels());
                        newTexture.Apply();
                        var bytes = newTexture.EncodeToPNG();
                        var fileName = "Metallic_" + root.name + "_" + material.name + ".png";
                        var fullPath = Path.Combine(exportPath, fileName);
                        File.WriteAllBytes(fullPath, bytes);
                        AssetDatabase.Refresh();
                    }

                    var specularMap = (Texture2D)material.GetTexture("_SpecGlossMap");
                    if (specularMap != null)
                    {
                        var newTexture = new Texture2D(specularMap.width, specularMap.height, TextureFormat.ARGB32, false);
                        newTexture.SetPixels(0, 0, specularMap.width, specularMap.height, specularMap.GetPixels());
                        newTexture.Apply();
                        var bytes = newTexture.EncodeToPNG();
                        var fileName = "Specular_" + root.name + "_" + material.name + ".png";
                        var fullPath = Path.Combine(exportPath, fileName);
                        File.WriteAllBytes(fullPath, bytes);
                        AssetDatabase.Refresh();
                    }

                    var normalMap = (Texture2D)material.GetTexture("_BumpMap");
                    if (normalMap != null)
                    {
                        var newTexture = new Texture2D(normalMap.width, normalMap.height, TextureFormat.ARGB32, false);
                        newTexture.SetPixels(0, 0, normalMap.width, normalMap.height, normalMap.GetPixels());
                        newTexture.Apply();
                        var bytes = newTexture.EncodeToPNG();
                        var fileName = "Normal_" + root.name + "_" + material.name + ".png";
                        var fullPath = Path.Combine(exportPath, fileName);
                        File.WriteAllBytes(fullPath, bytes);
                        AssetDatabase.Refresh();
                    }

                    var heightMap = (Texture2D)material.GetTexture("_ParallaxMap");
                    if (heightMap != null)
                    {
                        var newTexture = new Texture2D(heightMap.width, heightMap.height, TextureFormat.ARGB32, false);
                        newTexture.SetPixels(0, 0, heightMap.width, heightMap.height, heightMap.GetPixels());
                        newTexture.Apply();
                        var bytes = newTexture.EncodeToPNG();
                        var fileName = "Height_" + root.name + "_" + material.name + ".png";
                        var fullPath = Path.Combine(exportPath, fileName);
                        File.WriteAllBytes(fullPath, bytes);
                        AssetDatabase.Refresh();
                    }

                    var occlusionMap = (Texture2D)material.GetTexture("_OcclusionMap");
                    if (occlusionMap != null)
                    {
                        var newTexture = new Texture2D(occlusionMap.width, occlusionMap.height, TextureFormat.ARGB32, false);
                        newTexture.SetPixels(0, 0, occlusionMap.width, occlusionMap.height, occlusionMap.GetPixels());
                        newTexture.Apply();
                        var bytes = newTexture.EncodeToPNG();
                        var fileName = "Occlusion_" + root.name + "_" + material.name + ".png";
                        var fullPath = Path.Combine(exportPath, fileName);
                        File.WriteAllBytes(fullPath, bytes);
                        AssetDatabase.Refresh();
                    }

                    var emissionMap = (Texture2D)material.GetTexture("_EmissionMap");
                    if (emissionMap != null)
                    {
                        var newTexture = new Texture2D(emissionMap.width, emissionMap.height, TextureFormat.ARGB32, false);
                        newTexture.SetPixels(0, 0, emissionMap.width, emissionMap.height, emissionMap.GetPixels());
                        newTexture.Apply();
                        var bytes = newTexture.EncodeToPNG();
                        var fileName = "Emission_" + root.name + "_" + material.name + ".png";
                        var fullPath = Path.Combine(exportPath, fileName);
                        File.WriteAllBytes(fullPath, bytes);
                        AssetDatabase.Refresh();
                    }
                }
            }

            foreach (Transform child in root)
            {
                // Recursively save the textures of the child's children
                ExportTextures(child, exportPath);
            }
        }

        private static ExportData ExportDataFiles(string directory, string nameObject, GameObject objectOnScene, bool isSaveFiles)
        {
            var pathFbx = $"{directory}/{nameObject}.fbx";
            var saveBytesPath = $"{directory}/bytes_{nameObject}.bytes";
            var saveHexCodePath = $"{directory}/hex_{nameObject}.txt";
            var saveHashCodePath = $"{directory}/hash_{nameObject}.txt";
            var saveDataJsonPath = $"{directory}/json_{nameObject}";

            var bytes = isSaveFiles
                ? CreatorBytes.GenerateAndSave(pathFbx, saveBytesPath)
                : CreatorBytes.Generate(pathFbx);

            var hexCode = isSaveFiles
                ? CreatorHexCode.GenerateAndSave(bytes, saveHexCodePath)
                : CreatorHexCode.Generate(bytes);

            var hashCode = isSaveFiles
                ? CreatorHashCode.GenerateAndSave(hexCode, saveHashCodePath)
                : CreatorHashCode.Generate(hexCode);

            var objectData = new ObjectData()
            {
                Name = objectOnScene.name,
                Type = "None",
                Position = $"{objectOnScene.transform.position}",
                Rotation = $"{objectOnScene.transform.rotation}",
                Scale = $"{objectOnScene.transform.localScale}",
                ModelHashCode = hashCode ?? "",
            };

            var dataJson = isSaveFiles
                ? CreatorDataJson.GenerateAndSave(objectData, saveDataJsonPath)
                : CreatorDataJson.Generate(objectData);

            var exportData = new ExportData()
            {
                Bytes = bytes,
                HexCode = hexCode,
                HashCode = hashCode,
                ObjectData = objectData,
                JsonData = dataJson,
            };

            AssetDatabase.Refresh();

            return exportData;
        }
    }
}