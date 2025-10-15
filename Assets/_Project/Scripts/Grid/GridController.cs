using UnityEngine;

public class GridController : MonoBehaviour
{
    [SerializeField] private float cellWidth = 1f;
    [SerializeField] private float cellHeight = 0.5f;

    public Vector3 SnapToGrid(Vector3 worldPos)
    {
        float isoX = (worldPos.x / cellWidth + worldPos.y / cellHeight) / 2f;
        float isoY = (worldPos.y / cellHeight - (worldPos.x / cellWidth)) / 2f;

        isoX = Mathf.Round(isoX);
        isoY = Mathf.Round(isoY);

        float finalX = (isoX - isoY) * cellWidth;
        float finalY = (isoX + isoY) * cellHeight;

        return new Vector3(finalX, finalY, 0);
    }
}
