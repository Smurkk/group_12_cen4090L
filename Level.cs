using UnityEngine;

// Level class for Room management during level generation. Has basic functions like getting/setting the Room array, their indecies, and Features. 
public class Level
{
    public Room[] Rooms { get; private set; }
    public int CurrentRoomIndex { get; set; }
    public Level(int roomCount)
    {
        Rooms = new Room[roomCount];
        CurrentRoomIndex = 0;
    }

    public void AddRoom(int index, Room room)
    {
        if (index >= 0 && index < Rooms.Length)
        {
            Rooms[index] = room;
        }
    }

    public Room GetCurrentRoom()
    {
        return Rooms[CurrentRoomIndex];
    }

    public Tile[] GetAllTilesWithFeatures()
    {
        System.Collections.Generic.List<Tile> allTiles = new System.Collections.Generic.List<Tile>();
        // I decided on having it iterate over each value in Room instead of breaking whenever it saw Null because we might come to a point where we want levels that aren't strictly square,
        // such as a horseshoe-esque level or one shaped like an H with a central bridge and nothing else surrounding it.
        foreach (Room room in Rooms)
        {
            if (room == null) continue;
            Tile[] tilesWithFeatures = room.GetTilesWithFeatures();
            allTiles.AddRange(tilesWithFeatures);
        }
        return allTiles.ToArray();
    }
}