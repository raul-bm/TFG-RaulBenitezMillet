using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType
{
    Initial,
    Normal,
    Reward,
    Boss
}

public class RoomNode
{
    public int id {  get; private set; }
    public Vector2Int position { get; private set; }
    public RoomNode parent { get; private set; }
    public List<RoomNode> descendants { get; private set; }
    public int distance { get; private set; }
    public bool isInitial { get; private set; }
    public GameObject roomGameObject { get; private set; }
    public RoomType roomType { get; private set; }
    public int enemiesCount { get; private set; }

    public bool roomCleared { get; private set; }

    // Constructor for the initial node
    public RoomNode(int id)
    {
        this.id = id;
        position = Vector2Int.zero;
        parent = null;
        descendants = new List<RoomNode>();
        distance = 0;

        roomType = RoomType.Normal;

        if (id == 0)
        {
            isInitial = true;
            roomType = RoomType.Initial;
        }
        else isInitial = false;

        roomCleared = false;
    }

    // Constructor for the rest nodes
    public RoomNode(int id, RoomNode parent)
    {
        this.id = id;
        position = Vector2Int.zero;
        this.parent = parent;
        descendants = new List<RoomNode>();
        distance = 0;

        roomType = RoomType.Normal;

        if (id == 0)
        {
            isInitial = true;
            roomType = RoomType.Initial;
        }
        else isInitial = false;

        roomCleared = false;
    }

    // Constructor with all data
    public RoomNode(int id, int positionX, int positionY, int distance, int roomType, int enemiesCount, bool roomCleared)
    {
        this.id = id;
        position = new Vector2Int(positionX, positionY);
        parent = null;
        descendants = new List<RoomNode>();
        this.distance = distance;
        this.roomType = (RoomType)roomType;

        if (this.roomType == RoomType.Initial) isInitial = true;
        else isInitial = false;

        this.enemiesCount = enemiesCount;

        this.roomCleared = roomCleared;
    }

    public void Connect(RoomNode descendant, bool isLoadingSaveData = false)
    {
        if(!isLoadingSaveData) descendant.SetDistance(distance + 1); 

        descendants.Add(descendant);

        if (isLoadingSaveData) descendant.parent = this;
    }

    public void SetDistance(int distance)
    {
        this.distance = distance;
    }

    public void SetPosition(Vector2Int position)
    {
        this.position = position;
    }

    public void SetRoomType(RoomType roomType)
    {
        this.roomType = roomType;
    }

    public bool CanConnectMoreRooms()
    {
        if (isInitial && descendants.Count >= 4) return false;
        else if (!isInitial && descendants.Count >= 3) return false;
        return true;
    }

    public void SetRoomToThisNode(GameObject room)
    {
        roomGameObject = room;
    }

    public void SetEnemiesCountToRoom(int count)
    {
        enemiesCount = count;
    }

    public void RoomCleared()
    {
        roomCleared = true;
    }

    public string GetSerializedType()
    {
        if (roomType == RoomType.Initial) return "I";
        else if (roomType == RoomType.Reward) return "R";
        else if (roomType == RoomType.Boss) return "B";
        else return "N";
    }
}
