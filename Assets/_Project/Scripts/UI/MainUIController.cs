using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MainUIController : MonoBehaviour
{
    [Header("UI Buttons")]
    [SerializeField] private Button placeModeButton;
    [SerializeField] private Button deleteModeButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;

    [Header("Building Selection Panel")]
    [SerializeField] private GameObject buildingPanel;
    [SerializeField] private Button building1Button;
    [SerializeField] private Button building2Button;
    [SerializeField] private Button building3Button;

    [Header("Visuals")]
    [SerializeField] private Image modeIndicator;

    private BuildingPlacer _placer;
    private DataService _data;
    private BuildingSelector _selector;

    private readonly List<SavedBuilding> _placedBuildings = new();

    private bool _isPlaceMode = false;
    private bool _isDeleteMode = false;

    [Inject]
    public void Construct(BuildingPlacer placer, DataService data, BuildingSelector selector)
    {
        _placer = placer;
        _data = data;
        _selector = selector;
    }

    private void Start()
    {
        _data.Load();

        // Mode buttons
        placeModeButton.onClick.AddListener(TogglePlaceMode);
        deleteModeButton.onClick.AddListener(ToggleDeleteMode);

        // Building buttons
        building1Button.onClick.AddListener(() => StartPlacement(0));
        building2Button.onClick.AddListener(() => StartPlacement(1));
        building3Button.onClick.AddListener(() => StartPlacement(2));

        // Save/Load
        saveButton.onClick.AddListener(Save);
        loadButton.onClick.AddListener(Load);

        // UI state
        if (buildingPanel != null) buildingPanel.SetActive(false);
        UpdateModeIndicators();
    }

    private void TogglePlaceMode()
    {
        _isPlaceMode = !_isPlaceMode;
        _isDeleteMode = false;

        if (buildingPanel != null) buildingPanel.SetActive(_isPlaceMode);

        if (!_isPlaceMode)
            _placer.StopPlacement();

        _selector.EnableDeleteMode(false);

        UpdateModeIndicators();
    }

    private void ToggleDeleteMode()
    {
        _isDeleteMode = !_isDeleteMode;
        _isPlaceMode = false;

        if (buildingPanel != null) buildingPanel.SetActive(false);

        _selector.EnableDeleteMode(_isDeleteMode);

        UpdateModeIndicators();
    }

    private void UpdateModeIndicators()
    {
        if (modeIndicator != null)
        {
            if (_isPlaceMode)
                modeIndicator.color = Color.green;
            else if (_isDeleteMode)
                modeIndicator.color = Color.red;
            else
                modeIndicator.color = new Color(1, 1, 1, 0); // полностью прозрачный (невидимый)
        }
    }

    private void StartPlacement(int index)
    {
        if (_data.Buildings == null || index < 0 || index >= _data.Buildings.Count)
        {
            Debug.LogError("Invalid building index");
            return;
        }

        var config = _data.Buildings[index];
        _placer.StartPlacement(config);
    }

    private void Save()
    {
        _placedBuildings.Clear();
        var allBuildings = FindObjectsOfType<Building>();

        foreach (var b in allBuildings)
        {
            var config = _data.Buildings.FirstOrDefault(c => c.name == b.BuildingName);
            if (config != null)
            {
                _placedBuildings.Add(new SavedBuilding
                {
                    id = config.id,
                    position = b.transform.position
                });
            }
        }

        _data.SavePlacedBuildings(_placedBuildings);
    }

    private void Load()
    {
        var loaded = _data.LoadPlacedBuildings();
        if (loaded == null || loaded.Count == 0)
        {
            Debug.Log("Nothing to load.");
            return;
        }

        // Remove old buildings
        foreach (var b in FindObjectsOfType<Building>())
            Destroy(b.gameObject);

        // Instantiate loaded buildings
        foreach (var sb in loaded)
        {
            var cfg = _data.Buildings.FirstOrDefault(c => c.id == sb.id);
            if (cfg == null) continue;

            var prefab = Resources.Load<GameObject>(cfg.prefab);
            if (prefab == null)
            {
                Debug.LogError($"Prefab not found: {cfg.prefab}");
                continue;
            }

            Object.Instantiate(prefab, sb.position, Quaternion.identity);
        }

        Debug.Log($"Loaded {loaded.Count} buildings.");
    }
}
