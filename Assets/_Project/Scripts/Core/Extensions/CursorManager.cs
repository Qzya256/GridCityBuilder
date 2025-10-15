using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CursorManager: MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image cursorImage;
    [SerializeField] private Sprite handSprite;
    [SerializeField] private Sprite hammerSprite;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
        HideSystemCursor();
        SetDefualtCursor();
    }

    private void Update()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            mousePos,
            canvas.worldCamera,
            out Vector2 localPoint
        );

        cursorImage.rectTransform.localPosition = localPoint;
    }

    public void SetDefualtCursor()
    {
        HideSystemCursor();
        cursorImage.sprite = handSprite;
        cursorImage.enabled = true;
    }

    public void SetDestroyingCursor()
    {
        HideSystemCursor();
        cursorImage.sprite = hammerSprite;
        cursorImage.enabled = true;
    }

    public void SetBuildCursor()
    {
        HideSystemCursor();
        cursorImage.enabled = false;
    }

    private void HideSystemCursor()
    {
        Cursor.visible = false;
    }
}
