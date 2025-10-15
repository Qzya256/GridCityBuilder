#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;


public class BuildingConfigEditor : EditorWindow
{
    private List<BuildingConfig> _configs = new();
    private string _configPath = "Assets/_Project/Settings/buildings.json";

    [MenuItem("Tools/Building Config Editor")]
    public static void ShowWindow()
    {
        GetWindow<BuildingConfigEditor>("Building Configs");
    }

    private void OnEnable()
    {
        LoadConfigs();
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("Building Configurations", EditorStyles.boldLabel);

        if (GUILayout.Button("Add New Building"))
        {
            _configs.Add(new BuildingConfig());
        }

        GUILayout.Space(10);

        for (int i = 0; i < _configs.Count; i++)
        {
            var cfg = _configs[i];
            EditorGUILayout.BeginVertical("box");

            cfg.id = EditorGUILayout.IntField("ID", cfg.id);
            cfg.name = EditorGUILayout.TextField("Name", cfg.name);
            cfg.prefab = EditorGUILayout.TextField("Prefab Path", cfg.prefab);
            cfg.sizeX = EditorGUILayout.IntField("Size X", cfg.sizeX);
            cfg.sizeY = EditorGUILayout.IntField("Size Y", cfg.sizeY);

            GUILayout.Space(5);
            if (GUILayout.Button("Remove"))
            {
                _configs.RemoveAt(i);
                break;
            }

            EditorGUILayout.EndVertical();
        }

        GUILayout.Space(10);
        if (GUILayout.Button("💾 Save to JSON"))
        {
            SaveConfigs();
        }
    }

    private void LoadConfigs()
    {
        if (!File.Exists(_configPath))
        {
            _configs = new List<BuildingConfig>();
            return;
        }

        string json = File.ReadAllText(_configPath);
        _configs = JsonUtility.FromJson<Wrapper<BuildingConfig>>(WrapArray(json)).items.ToList();
    }

    private void SaveConfigs()
    {
        string json = JsonUtility.ToJson(new Wrapper<BuildingConfig> { items = _configs.ToArray() }, true);
        File.WriteAllText(_configPath, json);
        AssetDatabase.Refresh();
        Debug.Log("✅ Building configs saved!");
    }

    private static string WrapArray(string json) => "{\"items\":" + json + "}";

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] items;
    }
}
#endif
