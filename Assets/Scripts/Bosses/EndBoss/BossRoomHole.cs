using UnityEngine;
using UnityEngine.Tilemaps;

public class BossRoomHole : MonoBehaviour
{
    public Tilemap groundTilemap; // Reference to the ground Tilemap
    // public TileBase holeTile; // Optional: A tile to represent the hole

    public void OpenHole()
    {
        if (groundTilemap == null)
        {
            Debug.LogError("Ground Tilemap is not assigned!");
            return;
        }
        // Vector3Int holePosition = groundTilemap.WorldToCell(new Vector3(22, 29, 0)); // Position of the hole on the Tilemap
        Vector3Int holePosition = groundTilemap.WorldToCell(new Vector3(25, -2, 0)); // Position of the hole on the Tilemap
        print("converted coordinates: "+holePosition);
        // Clear the tile at the specified position to create a hole
        groundTilemap.SetTile(holePosition, null); // Remove the tile
    }
}
