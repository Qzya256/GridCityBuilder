using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class BuildingSelector : ITickable
{
    private readonly Camera _camera;
    private readonly CursorManager _cursor;
    private bool _deleteMode = false;

    public System.Action<Building> OnBuildingDeleted;

    [Inject]
    public BuildingSelector(Camera camera, CursorManager cursor)
    {
        _camera = camera;
        _cursor = cursor;
    }

    public void EnableDeleteMode(bool value)
    {
        _deleteMode = value;
        if (_deleteMode)
            _cursor.SetDestroyingCursor();
        else
            _cursor.SetDefualtCursor();
    }

    public void DisableDeleteMode()
    {
        _deleteMode = false;
        _cursor.SetDefualtCursor();
    }

    public void Tick()
    {
        if (!_deleteMode)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector3 worldPos = _camera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, -_camera.transform.position.z));
            Vector2 point = new Vector2(worldPos.x, worldPos.y);

            Collider2D hit = Physics2D.OverlapPoint(point, Physics2D.DefaultRaycastLayers);
            if (hit != null)
            {
                Building building = hit.GetComponent<Building>();
                if (building == null)
                    building = hit.GetComponentInParent<Building>();

                if (building != null)
                {
                    Object.Destroy(building.gameObject);
                    OnBuildingDeleted?.Invoke(building);
                }
            }
        }

        if (Mouse.current.rightButton.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            DisableDeleteMode();
        }
    }
}
