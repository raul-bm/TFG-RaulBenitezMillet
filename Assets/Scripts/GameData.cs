using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int actualLevel;
    public List<GameDataGraphNode> graphNodes;
    public List<GameDataGraphNodeDescendants> graphNodesDescendants;
    public int actualRoom;
    public float playerHealth;
    public List<GameDataObject> inventoryObjects;
    public List<GameDataObject> equippedObjects;
    public string seed;
    public string randomState;
}

[System.Serializable]
public class GameDataGraphNode
{
    public int nodeId;
    public int positionX;
    public int positionY;
    public int distance;
    public int roomType;
    public int enemiesCount;
    public bool roomCleared;
}

[System.Serializable]
public class GameDataGraphNodeDescendants
{
    public int nodeParentId;
    public List<int> descendants;
}

[System.Serializable]
public class GameDataObject
{
    public int setNumber;
    public int partNumber;
}
