using UnityEngine;

// Tile is a class that is used in level generation. Tiles are effectively chunks used for level generation.
// They may or may not contain a feature. 
public class Tile
{
    public Vector2Int Position { get; set; }

    // Logic mostly for walls
    public bool IsFloor { get; set; }
    public bool IsWall { get; set; }

    // Retrieves ID of the Feature on the tile, returns 0 on no feature.
    public int FeatureID { get; set; }
    public Tile(Vector2 position)
    {
        IsFloor = true;
        IsWall = false;
        Position = new Vector2Int((int)position.x, (int)position.y);
        FeatureID = 0;
    }

}