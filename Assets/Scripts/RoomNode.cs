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
    }

    public void Connect(RoomNode descendant)
    {
        descendant.SetDistance(distance + 1);
        descendants.Add(descendant);
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
}
