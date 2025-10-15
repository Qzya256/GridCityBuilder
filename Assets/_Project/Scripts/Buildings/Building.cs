using UnityEngine;

public class Building : MonoBehaviour
{
    public string BuildingName;                  // Used for matching config name
    public SpriteRenderer BuildingSprite;        // Main sprite
    public PolygonCollider2D PlacementCollider;  // Collider used for overlap check
}
