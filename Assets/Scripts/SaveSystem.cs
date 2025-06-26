using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SaveSystem
{
    private static string saveFileRoute = Path.Combine(Path.Combine(Application.persistentDataPath, "Saves"), "saveFile.json");

    public static void Save()
    {
        Save(saveFileRoute);

        SceneManager.LoadScene(0);
    }

    public static void Save(string fileName)
    {
        var directoryName = Path.GetDirectoryName(fileName);

        if(!Directory.Exists(directoryName)) Directory.CreateDirectory(directoryName);

        File.WriteAllText(fileName, SerializeSave());

        Debug.Log("Saved on: " + fileName);
    }

    private static string SerializeSave()
    {
        GameData gameData = new GameData();

        // Actual level
        gameData.actualLevel = DungeonCrawlerController.Instance.actualLevel;

        // Nodes Graph
        gameData.graphNodes = new List<GameDataGraphNode>();
        
        foreach(RoomNode node in DungeonCrawlerController.Instance.roomNodes)
        {
            GameDataGraphNode gameDataGraphNode = new GameDataGraphNode();
            gameDataGraphNode.nodeId = node.id;
            gameDataGraphNode.positionX = node.position.x;
            gameDataGraphNode.positionY = node.position.y;
            gameDataGraphNode.distance = node.distance;
            gameDataGraphNode.roomType = (int)node.roomType;
            gameDataGraphNode.enemiesCount = node.enemiesCount;
            gameDataGraphNode.roomCleared = node.roomCleared;

            gameData.graphNodes.Add(gameDataGraphNode);
        }

        // Descendants of each node
        gameData.graphNodesDescendants = new List<GameDataGraphNodeDescendants>();

        foreach(RoomNode node in DungeonCrawlerController.Instance.roomNodes)
        {
            GameDataGraphNodeDescendants gameDataNodeDescendants = new GameDataGraphNodeDescendants();
            gameDataNodeDescendants.nodeParentId = node.id;

            gameDataNodeDescendants.descendants = new List<int>();

            foreach(RoomNode descendant in node.descendants)
            {
                gameDataNodeDescendants.descendants.Add(descendant.id);
            }

            gameData.graphNodesDescendants.Add(gameDataNodeDescendants);
        }

        // Actual room
        gameData.actualRoom = DungeonCrawlerController.Instance.actualRoom.thisRoomNode.id;

        // Current player health
        gameData.playerHealth = DungeonCrawlerController.Instance.player.GetComponent<PlayerController>().currentHealth;

        // Inventory objects
        gameData.inventoryObjects = new List<GameDataObject>();

        foreach(var swordPart in InventoryUI.Instance.inventorySlotsOccupied.Values)
        {
            SwordPartScripteable scripteable = swordPart.GetComponent<SwordPartInventory>().partScriptable;

            GameDataObject gameDataObject = new GameDataObject();
            gameDataObject.setNumber = (int)scripteable.setOfSword;
            gameDataObject.partNumber = (int)scripteable.partType;

            gameData.inventoryObjects.Add(gameDataObject);
        }

        // Equipped objects
        gameData.equippedObjects = new List<GameDataObject>();

        foreach (var swordPart in InventoryUI.Instance.inventorySlotsPartsEquippedOccupied.Values)
        {
            SwordPartScripteable scripteable = swordPart.GetComponent<SwordPartInventory>().partScriptable;

            GameDataObject gameDataObject = new GameDataObject();
            gameDataObject.setNumber = (int)scripteable.setOfSword;
            gameDataObject.partNumber = (int)scripteable.partType;

            gameData.equippedObjects.Add(gameDataObject);
        }

        // Seed
        gameData.seed = DungeonCrawlerController.Instance.seed;

        // Actual random state hash
        gameData.randomState = JsonUtility.ToJson(Random.state);

        return JsonUtility.ToJson(gameData);
    }

    public static GameData Load()
    {
        string json = File.ReadAllText(saveFileRoute);
        GameData gameData = JsonUtility.FromJson<GameData>(json);

        return gameData;
    }

    public static void Delete()
    {
        File.Delete(saveFileRoute);
    }

    public static bool IsThereAnySaveData()
    {
        return File.Exists(saveFileRoute);
    }
}
