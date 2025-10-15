using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using Zenject;

public class BuildingPlacer : ITickable
{
    private readonly GridController _grid;
    private readonly CursorManager _cursor;

    private Camera _camera;
    private GameObject _ghost;
    private SpriteRenderer _ghostRenderer;
    private bool _isPlacing;
    private GameObject _prefabToPlace;

    private readonly Color canPlaceColor = new Color(0.4f, 1f, 0.4f, 0.6f);   // green
    private readonly Color cantPlaceColor = new Color(1f, 0.3f, 0.3f, 0.6f);  // red

    [Inject]
    public BuildingPlacer(GridController grid, CursorManager cursor)
    {
        _grid = grid;
        _cursor = cursor;
    }

    public void StartPlacement(BuildingConfig config)
    {
        StopPlacement();

        _camera = Camera.main;
        _prefabToPlace = Resources.Load<GameObject>(config.prefab);

        var buildingComponent = _prefabToPlace.GetComponent<Building>();
        if (buildingComponent == null || buildingComponent.BuildingSprite == null)
        {
            Debug.LogError("Prefab must have Building component with assigned BuildingSprite");
            return;
        }

        // Create semi-transparent "ghost" of the building
        _ghost = new GameObject("GhostBuilding");
        _ghost.transform.localScale = buildingComponent.BuildingSprite.transform.localScale;
        _ghostRenderer = _ghost.AddComponent<SpriteRenderer>();
        _ghostRenderer.sprite = buildingComponent.BuildingSprite.sprite;
        _ghostRenderer.color = canPlaceColor;
        _ghostRenderer.sortingOrder = 9999; // always on top

        _cursor.SetBuildCursor(); // hide cursor during placement

        _isPlacing = true;
    }

    public void StopPlacement()
    {
        if (_ghost != null)
        {
            Object.Destroy(_ghost);
            _cursor.SetDefualtCursor(); // restore normal cursor
            _ghost = null;
        }
        _isPlacing = false;
    }

    public void Tick()
    {
        if (!_isPlacing || _ghost == null) return;

        if (_camera == null)
            _camera = Camera.main;

        Vector2 mousePos = Mouse.current.position.ReadValue();

        // Ignore cursor outside of screen
        if (mousePos.x < 0 || mousePos.y < 0 || mousePos.x > Screen.width || mousePos.y > Screen.height)
            return;

        // Ignore UI elements
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        Vector3 worldPos = _camera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0));
        worldPos.z = 0;
        _ghost.transform.position = _grid.SnapToGrid(worldPos);

        bool canPlace = CanPlaceHere(_ghost.transform.position);
        _ghostRenderer.color = canPlace ? canPlaceColor : cantPlaceColor;

        // Left click → place building (if valid)
        if (Mouse.current.leftButton.wasPressedThisFrame && canPlace)
        {
            PlaceBuilding();
        }

        // Right click / ESC → cancel placement
        if (Mouse.current.rightButton.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            StopPlacement();
        }
    }

    private bool CanPlaceHere(Vector3 position)
    {
        var tempBuilding = Object.Instantiate(_prefabToPlace, position, Quaternion.identity);
        var buildingComponent = tempBuilding.GetComponent<Building>();

        if (buildingComponent == null || buildingComponent.PlacementCollider == null)
        {
            Object.Destroy(tempBuilding);
            return true;
        }

        var collider = buildingComponent.PlacementCollider;
        collider.enabled = true;

        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = true;

        Collider2D[] results = new Collider2D[10];
        int hitCount = collider.OverlapCollider(filter, results);

        bool overlaps = false;
        for (int i = 0; i < hitCount; i++)
        {
            if (results[i] != null && results[i].transform.root != tempBuilding.transform)
            {
                overlaps = true;
                break;
            }
        }

        Object.Destroy(tempBuilding);
        return !overlaps;
    }

    private void PlaceBuilding()
    {
        if (_prefabToPlace == null) return;

        Vector3 pos = _grid.SnapToGrid(_ghost.transform.position);

        if (!CanPlaceHere(pos))
        {
            Debug.Log("Space is already occupied!");
            return;
        }

        var placed = Object.Instantiate(_prefabToPlace, pos, Quaternion.identity);

        var buildingComponent = placed.GetComponent<Building>();
        if (buildingComponent != null && buildingComponent.BuildingSprite != null)
        {
            Vector3 screenPos = _camera.WorldToScreenPoint(buildingComponent.BuildingSprite.transform.position);
            buildingComponent.BuildingSprite.sortingOrder = Mathf.RoundToInt(-screenPos.y);
        }

        StopPlacement();
    }
}
