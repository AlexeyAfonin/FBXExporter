using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace ANYTY.FBXExporter.Editor
{
    public class FBXExporterWindow : EditorWindow
    {
        private List<GameObject> _rootObjects = new();
        private List<GameObject> _allObjects = new();
        private List<GameObject> _selectedObjects = new();
        private Vector2 _scrollPosition;
        private string _exportPath = "";

        private static bool _isExportTextures = false;
        private static bool _isIncludeDataFiles = true;

        [MenuItem("ANYTY/Tools/FBXExporter")]
        private static void ShowWindow()
        {
            FBXExporterWindow window = GetWindow<FBXExporterWindow>();
            window.titleContent = new GUIContent("FBX Exporter");
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Refresh", GUILayout.Width(80)) || IsNewSelect())
            {
                RefreshHierarchy();
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();

            if (Selection.gameObjects == null || Selection.gameObjects.Length == 0)
            {
                EditorGUILayout.LabelField("Please select the object whose hierarchy you want to display before exporting!");
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Export path:", GUILayout.Width(80));
                _exportPath = GUILayout.TextField(_exportPath);
                if (GUILayout.Button("Browse", GUILayout.Width(80)))
                {
                    string newPath = EditorUtility.SaveFolderPanel("Select Export Folder", "", "");
                    if (!string.IsNullOrEmpty(newPath))
                    {
                        _exportPath = newPath;
                    }
                }
                GUILayout.EndHorizontal();

                EditorGUILayout.Space();

                GUILayout.BeginHorizontal();
                _isIncludeDataFiles = EditorGUILayout.Toggle(_isIncludeDataFiles, GUILayout.Width(20));
                EditorGUILayout.LabelField("Create data files");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                _isExportTextures = EditorGUILayout.Toggle(_isExportTextures, GUILayout.Width(20));
                EditorGUILayout.LabelField("Export textures");
                GUILayout.EndHorizontal();

                EditorGUILayout.Space();

                GUILayout.Label($"{_selectedObjects.Count}/{_allObjects.Count}", EditorStyles.boldLabel);

                EditorGUILayout.Space();

                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

                _allObjects.Clear();

                for (int i = _rootObjects.Count - 1; i >= 0; i--)
                {
                    DrawObjectHierarchy(_rootObjects[i], 0);
                }

                EditorGUILayout.EndScrollView();

                GUILayout.Space(10);

                if (GUILayout.Button("Export to FBX"))
                {
                    FBXExporter.GenerateAndSaveObjectData((Object[])_selectedObjects.ToArray(), _exportPath, _isExportTextures, _isIncludeDataFiles);
                }
            }
        }

        private bool IsNewSelect()
        {
            var result = false;

            if (_rootObjects.Count == 0 || _rootObjects == null)
            {
                result = true;
            }
            else
            {
                foreach (var obj in Selection.gameObjects)
                {
                    if (!_rootObjects.Contains(obj))
                    {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }

        private void RefreshHierarchy()
        {
            _allObjects.Clear();
            _selectedObjects.Clear();
            _rootObjects = new List<GameObject>(Selection.gameObjects);
            Repaint();
        }

        private void DrawObjectHierarchy(GameObject obj, int indentLevel)
        {
            _allObjects.Add(obj);

            EditorGUILayout.BeginHorizontal();

            bool isSelected = _selectedObjects.Contains(obj);
            bool newSelection = EditorGUILayout.Toggle(isSelected, GUILayout.Width(20));
            if (newSelection && !isSelected)
            {
                _selectedObjects.Add(obj);
            }
            else if (!newSelection && isSelected)
            {
                _selectedObjects.Remove(obj);
            }

            GUILayout.Space(indentLevel * 20);
            EditorGUILayout.LabelField(obj.name);
            EditorGUILayout.EndHorizontal();

            foreach (Transform child in obj.transform)
            {
                DrawObjectHierarchy(child.gameObject, indentLevel + 1);
            }
        }
    }
}