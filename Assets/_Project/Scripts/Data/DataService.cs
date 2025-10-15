using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[Serializable]
public class BuildingConfig
{
    public int id;
    public string name;
    public string prefab;
    public int sizeX;
    public int sizeY;
}

[Serializable]
public class SavedBuilding
{
    public int id;
    public Vector3 position;
}

public class DataService
{
    private const string CONFIG_PATH = "Assets/_Project/Settings/buildings.json";
    private readonly string SAVE_PATH = Path.Combine(Application.persistentDataPath, "placed_buildings.json");

    public IReadOnlyList<BuildingConfig> Buildings { get; private set; }
    private List<SavedBuilding> _placedBuildings = new();

    public void Load()
    {
        LoadConfigs();
    }

    private void LoadConfigs()
    {
        if (!File.Exists(CONFIG_PATH))
        {
            Debug.LogError($"Config file not found: {CONFIG_PATH}");
            Buildings = new List<BuildingConfig>();
            return;
        }

        string json = File.ReadAllText(CONFIG_PATH);
        Buildings = JsonUtility.FromJson<Wrapper<BuildingConfig>>(WrapArray(json)).items.ToList();
        Debug.Log($"Loaded {Buildings.Count} building configs.");
    }

    public void SavePlacedBuildings(List<SavedBuilding> buildings)
    {
        _placedBuildings = buildings;
        string json = JsonUtility.ToJson(new Wrapper<SavedBuilding> { items = _placedBuildings.ToArray() }, true);
        File.WriteAllText(SAVE_PATH, json);
        Debug.Log($"Saved {_placedBuildings.Count} placed buildings → {SAVE_PATH}");
    }

    public List<SavedBuilding> LoadPlacedBuildings()
    {
        if (!File.Exists(SAVE_PATH))
        {
            Debug.Log("No save file found.");
            return new List<SavedBuilding>();
        }

        string json = File.ReadAllText(SAVE_PATH);
        var result = JsonUtility.FromJson<Wrapper<SavedBuilding>>(json).items.ToList();
        Debug.Log($"Loaded {result.Count} placed buildings from save file.");
        return result;
    }

    private static string WrapArray(string json) => "{\"items\":" + json + "}";

    [Serializable]
    private class Wrapper<T>
    {
        public T[] items;
    }
}
