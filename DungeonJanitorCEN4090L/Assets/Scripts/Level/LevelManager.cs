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

    [Header("Cleanable Messes")]
    public GameObject dirtPilePrefab;
    public GameObject trashPrefab;
    public GameObject bloodStainPrefab;
    public GameObject brokenGlassPrefab;
    public GameObject skeletonPrefab;

    [Header("Generation Settings")]
    public float tileSize = 1f;
    public float roomSpacing = 20f;

    [Header("Current Mission")]
    public MissionDifficulty currentDifficulty;
    public Level CurrentLevel { get; private set; }

    // Storing these so we can track when players are allowed to leave the room
    private int totalMesses = 0;
    private int messesCleanedCount = 0;

    // Dictionary that maps IDs to prefab objects
    private Dictionary<int, GameObject> featurePrefabs;

    void Awake()
    {
        InitializeFeaturePrefabs();
    }

    void Start()
    {
        if (currentDifficulty == null)
        {
            currentDifficulty = MissionDifficulty.Easy;
        }
        
        GenerateLevel(currentDifficulty);

        // Commenting out SampleLevel call here as well as implementation down later, if we need to test we can uncomment
        // GenerateSampleLevel();

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
            { FeatureID.CHEST, chestPrefab },
	        { FeatureID.DIRT_PILE, dirtPilePrefab },
            { FeatureID.TRASH, trashPrefab },
            { FeatureID.BLOOD_STAIN, bloodStainPrefab },
            { FeatureID.BROKEN_GLASS, brokenGlassPrefab },
	        { FeatureID.SKELETON, skeletonPrefab }
        };
    }


    // Sample Level Generator with 2 rooms. This hard sets rooms, only used for testing purposes
    // void GenerateSampleLevel()
    // {
    //     CurrentLevel = new Level(2);
    //     Room room1 = new Room(10, 10, new Vector2Int(0, 0), tileSize);
    //     room1.GenerateBasicRoom();
    //     room1.SetFeature(5, 5, FeatureID.TABLE);
    //     room1.SetFeature(3, 3, FeatureID.CHAIR);
    //     room1.SetFeature(7, 7, FeatureID.CHEST);
    //     Room room2 = new Room(8, 8, new Vector2Int(15, 0), tileSize);
    //     room2.GenerateBasicRoom();
    //     room2.SetFeature(4, 4, FeatureID.DECORATION);
    //     CurrentLevel.AddRoom(0, room1);
    //     CurrentLevel.AddRoom(1, room2);
    //     Debug.Log("Level generated with " + CurrentLevel.Rooms.Length + " rooms");
    // }

    void GenerateLevel(MissionDifficulty difficulty)
    {
        int roomCount = 0;
        int minSize = 0;
        int maxSize = 0;

        switch (difficulty)
        {
            case MissionDifficulty.Easy:
                roomCount = 2;
                minSize = 6; maxSize = 8;
                break;

            case MissionDifficulty.Medium:
                roomCount = Random.Range(3, 5);
                minSize = 8; maxSize = 12;
                break;

            case MissionDifficulty.Hard:
                roomCount = Random.Range(5, 7);
                minSize = 12; maxSize = 20;
                break;
        }

        CurrentLevel = new Level(roomCount);

        float xOffset = 0f;

        for (int i = 0; i < roomCount; i++)
        {
            int w = Random.Range(minSize, maxSize + 1);
            int h = Random.Range(minSize, maxSize + 1);

            Room room = new Room(w, h, new Vector2(xOffset, 0), tileSize);

            room.GenerateBasicRoom();        // walls
            GenerateRandomFeatures(room);    // tables, chairs, decorations
            GenerateRandomMesses(room);      // glass, skeletons, etc.

            CurrentLevel.AddRoom(i, room);

            xOffset += (w * tileSize) + roomSpacing;
        }
    }

    void GenerateRandomFeatures(Room room)
    {
        int count = Random.Range(2, 6);

        for (int i = 0; i < count; i++)
        {
            int x = Random.Range(1, room.Width - 1);
            int y = Random.Range(1, room.Height - 1);

            int feature = Random.Range(1, 6); // TABLE-CHEST ONLY! In a perfect world this would be two const variables but I have hard coded it here.
            room.SetFeature(x, y, feature);
        }
    }

    void GenerateRandomMesses(Room room)
    {
        int messCount = Random.Range(3, 7);

        for (int i = 0; i < messCount; i++)
        {
            int x = Random.Range(1, room.Width - 1);
            int y = Random.Range(1, room.Height - 1);

            int messID = Random.Range(FeatureID.DIRT_PILE, FeatureID.SKELETON + 1);

            Tile tile = room.GetTile(x, y);
            tile.FeatureID = messID;

            totalMesses++;
        }
    }

    // Now that the level is properly spawned in, we add features
    void SpawnFeatures()
    {
        Tile[] tiles = CurrentLevel.GetAllTilesWithFeatures();

        foreach (Tile tile in tiles)
        {
            if (!featurePrefabs.TryGetValue(tile.FeatureID, out GameObject prefab))
            {
                continue;
            }

            // Build a new world position based off of tile positions and their size
            Vector2 worldPos =
                new Vector2(tile.Position.x, tile.Position.y) * tileSize +
                CurrentLevel.Rooms[0].Position;

            Room parentRoom = FindRoomFromTile(tile);

            GameObject obj = Instantiate(prefab, worldPos, Quaternion.identity);
            obj.transform.parent = transform;
            obj.name = GetFeatureName(tile.FeatureID);

            if (ShouldHaveCollider(tile.FeatureID))
                EnsureCollider(obj);

            // Attach CleanableObject info if it's a Cleanable Object
            CleanableObject clean = obj.GetComponent<CleanableObject>();
            if (clean != null)
            {
                clean.featureID = tile.FeatureID;
                clean.localTilePosition = tile.Position;
                clean.parentRoom = parentRoom;
            }
        }
    }

    Room FindRoomFromTile(Tile tile)
    {
        foreach (Room r in CurrentLevel.Rooms)
        {
            if (tile.Position.x < r.Width && tile.Position.y < r.Height)
                return r;
        }
        return null;
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
	        case FeatureID.DIRT_PILE: return "Dirt Pile";
            case FeatureID.TRASH: return "Trash";
            case FeatureID.BLOOD_STAIN: return "Blood Stain";
            case FeatureID.BROKEN_GLASS: return "Broken Glass";
	        case FeatureID.SKELETON: return "Skeleton";
            default: return "Feature";
        }
    }
}