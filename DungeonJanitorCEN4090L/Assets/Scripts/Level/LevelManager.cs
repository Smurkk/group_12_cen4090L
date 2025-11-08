using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    [Header("Features")]
    public GameObject wallPrefab;
    public GameObject tablePrefab;
    public GameObject decorationPrefab;
    public GameObject doorPrefab;
    public GameObject chairPrefab;
    public GameObject chestPrefab;

    [Header("Generation Settings")]
    public float tileSize = 1f;

    public Level CurrentLevel { get; private set; }

    // Dictionary that maps IDs to prefab objects
    private Dictionary<int, GameObject> featurePrefabs;

    void Awake()
    {
        InitializeFeaturePrefabs();
    }

    void Start()
    {
        GenerateSampleLevel();
        SpawnFeatures();
    }

    void InitializeFeaturePrefabs()
    {
        featurePrefabs = new Dictionary<int, GameObject>
        {
            { FeatureID.TABLE, tablePrefab },
            { FeatureID.WALL, wallPrefab },
            { FeatureID.DECORATION, decorationPrefab },
            { FeatureID.DOOR, doorPrefab },
            { FeatureID.CHAIR, chairPrefab },
            { FeatureID.CHEST, chestPrefab }
        };
    }
    // Sample Level Generator with 2 rooms. This hard sets rooms, only used for testing purposes
    void GenerateSampleLevel()
    {
        CurrentLevel = new Level(2);
        Room room1 = new Room(10, 10, new Vector2Int(0, 0), tileSize);
        room1.GenerateBasicRoom();
        room1.SetFeature(5, 5, FeatureID.TABLE);
        room1.SetFeature(3, 3, FeatureID.CHAIR);
        room1.SetFeature(7, 7, FeatureID.CHEST);
        Room room2 = new Room(8, 8, new Vector2Int(15, 0), tileSize);
        room2.GenerateBasicRoom();
        room2.SetFeature(4, 4, FeatureID.DECORATION);
        CurrentLevel.AddRoom(0, room1);
        CurrentLevel.AddRoom(1, room2);
        Debug.Log("Level generated with " + CurrentLevel.Rooms.Length + " rooms");
    }
    // Now that the level is properly spawned in, we add features
    void SpawnFeatures()
    {
        Tile[] tiles = CurrentLevel.GetAllTilesWithFeatures();
        foreach (Tile tile in tiles)
        {
            if (tile.FeatureID != FeatureID.NONE && featurePrefabs.ContainsKey(tile.FeatureID))
            {
                GameObject prefab = featurePrefabs[tile.FeatureID];
                if (prefab != null)
                {
                    // Converts tile grid position to world position if raw coords are necessary
                    Vector2 worldPos = new Vector2(tile.Position.x * tileSize, tile.Position.y * tileSize);
                    GameObject spawned = Instantiate(prefab, worldPos, Quaternion.identity);
                    spawned.name = GetFeatureName(tile.FeatureID);
                    spawned.transform.parent = this.transform;
                    // Checks if a feature should block movement
                    if (ShouldHaveCollider(tile.FeatureID))
                    {
                        EnsureCollider(spawned);
                    }
                }
            }
        }
        Debug.Log("Spawned " + tiles.Length + " Features");
    }
    // This function should be an exhaustive list of which features block movement
    bool ShouldHaveCollider(int featureID)
    {
        return featureID == FeatureID.TABLE || featureID == FeatureID.WALL || featureID == FeatureID.CHEST;
    }
    // This function forces colliders to spawn (for Rigidbody purposes)
    void EnsureCollider(GameObject obj)
    {
        if (obj.GetComponent<Collider2D>() == null)
        {
            BoxCollider2D collider = obj.AddComponent<BoxCollider2D>();
            SpriteRenderer sprite = obj.GetComponent<SpriteRenderer>();
            if (sprite != null)
            {
                collider.size = sprite.bounds.size;
            }
        }
    }
    // Returns Feature's name
    string GetFeatureName(int featureID)
    {
        switch (featureID)
        {
            case FeatureID.TABLE: return "Table";
            case FeatureID.WALL: return "Wall";
            case FeatureID.DECORATION: return "Decoration";
            case FeatureID.DOOR: return "Door";
            case FeatureID.CHAIR: return "Chair";
            case FeatureID.CHEST: return "Chest";
            default: return "Feature";
        }
    }
}