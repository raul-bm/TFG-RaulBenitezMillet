using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

public class DungeonCrawlerController : MonoBehaviour
{
    public static DungeonCrawlerController Instance { get; private set; }

    [SerializeField] private GameObject cameraPlayer;
    public GameObject player;
    [SerializeField] private GameObject roomNormalPrefab, roomInitialPrefab, roomBossPrefab, roomRewardPrefab;
    [SerializeField] private GameObject roomsClassify;
    [SerializeField] private GameObject swordPartsParent;
    [SerializeField] private Vector2Int distanceBetweenRooms;
    [SerializeField] private DungeonGenerationData earlyLevels, middleLevels, finalLevels, postgameLevels;

    // 0,0 -> Set Hielo Pomo  0,1 -> Set Hielo Grip    etc
    public List<RowContainer> swordPartScripteablesList; 
        
    public string seed;

    public List<RoomNode> roomNodes;
    private Dictionary<Vector2Int, RoomNode> roomPositions;
    public int actualLevel;
    public DungeonGenerationData actualDifficultyData { get; private set; }

    private RoomNode farthestRoomNode;

    private List<Vector2Int> directions = new List<Vector2Int> { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

    public int enemiesToKillOnActualRoom = 0;
    public int enemiesKilled = 0;

    public Room actualRoom;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if(PlayerPrefs.GetInt("gameSaved") == 0)
        {
            actualLevel = 0;

            seed = PlayerPrefs.GetString("gameSeed");

            Random.InitState(seed.GetHashCode());

            ProceduralGeneration();
        }
        else
        {
            GameData saveData = SaveSystem.Load();

            LoadFromSaveData(saveData);
        }
    }

    public void ProceduralGeneration()
    {
        // Last level
        if(actualLevel == 6)
        {

        }
        else
        {
            UI.Instance.LoadingScreenOn();

            EraseLevel();

            actualLevel++;

            GenerateNodeTree();
            ShowNodeTree();
            SaveRoomPositions();
            ChangeRoomType();
            SetEnemiesCountInEachRoom();
            InstanceRooms();

            MinimapManager.Instance.RevealRoom(Vector2Int.zero, roomPositions[Vector2Int.zero]);
            roomPositions[Vector2Int.zero].roomGameObject.GetComponent<Room>().InitializeRoom();
        }
    }

    private void EraseLevel()
    {
        player.transform.position = Vector3.zero;
        cameraPlayer.GetComponent<CameraController>().ResetCameraLevel();

        for (int i = roomsClassify.transform.childCount - 1; i >= 0; i--) Destroy(roomsClassify.transform.GetChild(i).gameObject);
        for(int i = swordPartsParent.transform.childCount - 1; i >= 0; i--) Destroy(swordPartsParent.transform.GetChild(i).gameObject);

        roomNodes = new List<RoomNode>();
        roomPositions = new Dictionary<Vector2Int, RoomNode>();

        MinimapManager.Instance.ResetMinimap();
    }

    private void GenerateNodeTree()
    {
        if (actualLevel <= 2) actualDifficultyData = earlyLevels;
        else if (actualLevel <= 4) actualDifficultyData = middleLevels;
        else if (actualLevel <= 6) actualDifficultyData = finalLevels;
        else actualDifficultyData = postgameLevels;

        roomNodes.Clear();

        int roomId = 0;

        RoomNode initialNode = new RoomNode(roomId++);
        initialNode.SetDistance(0);
        roomNodes.Add(initialNode);

        farthestRoomNode = initialNode;

        int quantityRooms = Random.Range(actualDifficultyData.minRooms, actualDifficultyData.maxRooms);

        for(int i = roomNodes.Count; i <= quantityRooms; i++)
        {
            RoomNode parentNode;

            do
            {
                parentNode = roomNodes[Random.Range(0, roomNodes.Count)];
            } while (!parentNode.CanConnectMoreRooms());

            RoomNode newRoom = new RoomNode(roomId++, parentNode);

            parentNode.Connect(newRoom);
            roomNodes.Add(newRoom);

            if (newRoom.distance > farthestRoomNode.distance)
            {
                farthestRoomNode = newRoom;
            }
        }
    }

    #region DEBUG_SHOW NODE TREE
    private void ShowNodeTree()
    {
        string message = ShowNodeTree(roomNodes[0]);

        Debug.Log(message);
    }

    private string ShowNodeTree(RoomNode actualRoom)
    {
        string message = "";

        message += actualRoom.id.ToString() + " - ";

        for (int i = 0; i < actualRoom.descendants.Count; i++)
        {
            message += ShowNodeTree(actualRoom.descendants[i]);

            message += actualRoom.id.ToString() + " - ";
        }

        return message;
    }
    #endregion

    public void SaveRoomPositions()
    {    
        roomPositions.Clear();
        // Add first room on (0, 0)
        roomNodes[0].SetPosition(Vector2Int.zero);
        roomPositions.Add(Vector2Int.zero, roomNodes[0]);

        SaveRoomPositions(roomNodes[0]);
    }

    public void SaveRoomPositions(RoomNode actualRoomNode)
    {
        List<Vector2Int> directionsAvailable = new List<Vector2Int>(directions);

        foreach (RoomNode descendant in actualRoomNode.descendants)
        {
            List<Vector2Int> directionsForCheck = new List<Vector2Int>(directionsAvailable);
            Vector2Int directionGood;
            int emptyAvailable;
            bool directionFound = false;

            do
            {
                directionGood = directionsForCheck[Random.Range(0, directionsForCheck.Count)];
                directionsForCheck.Remove(directionGood);

                //Debug.Log("Count: " + directionsForCheck.Count + " - ID: " + actualRoomNode.id + " - Descendant: " + descendant.id);

                if(!roomPositions.ContainsKey(actualRoomNode.position + directionGood))
                {
                    emptyAvailable = 0;
                    for (int i = 0; i < directions.Count; i++)
                    {
                        if (!roomPositions.ContainsKey(actualRoomNode.position + directionGood + directions[i])) emptyAvailable++;
                    }

                    if (emptyAvailable >= descendant.descendants.Count) directionFound = true;
                }
            } while (!directionFound && directionsForCheck.Count > 0);

            // Aux room if good direction not found

            directionsAvailable.Remove(directionGood);

            /*Debug.Log(directionGood);
            Debug.Log(actualRoomNode.position + directionGood);
            Debug.Log(descendant.id);*/
            Vector2Int positionNewRoom = actualRoomNode.position + directionGood;
            descendant.SetPosition(positionNewRoom);
            roomPositions.Add(positionNewRoom, descendant);
        }

        foreach(RoomNode descendant in actualRoomNode.descendants)
        {
            SaveRoomPositions(descendant);
        }
    }

    public void ChangeRoomType()
    {
        // Boss room
        farthestRoomNode.SetRoomType(RoomType.Boss);

        // Reward rooms
        for(int i = 0; i < actualDifficultyData.rewardRooms; i++)
        {
            RoomNode actualNodeWhile = roomNodes[0];

            while(actualNodeWhile.roomType != RoomType.Normal) actualNodeWhile = roomNodes[Random.Range(0, roomNodes.Count)];

            actualNodeWhile.SetRoomType(RoomType.Reward);
        }
    }

    private void SetEnemiesCountInEachRoom()
    {
        foreach(var roomNode in roomNodes)
        {
            if(roomNode.roomType == RoomType.Normal)
                roomNode.SetEnemiesCountToRoom(Random.Range(actualDifficultyData.enemiesPerRoomMin, actualDifficultyData.enemiesPerRoomMax + 1));
        }
    }

    public void InstanceRooms()
    {
        foreach(var roomNodeDictionary in roomPositions)
        {
            Vector3 roomPosition = new Vector3(roomNodeDictionary.Value.position.x * distanceBetweenRooms.x, roomNodeDictionary.Value.position.y * distanceBetweenRooms.y, 0);

            GameObject prefabToInstantiate;

            if (roomNodeDictionary.Value.roomType == RoomType.Normal) prefabToInstantiate = roomNormalPrefab;
            else if (roomNodeDictionary.Value.roomType == RoomType.Initial) prefabToInstantiate = roomInitialPrefab;
            else if (roomNodeDictionary.Value.roomType == RoomType.Reward) prefabToInstantiate = roomRewardPrefab;
            else prefabToInstantiate = roomBossPrefab;

            GameObject roomGenerated = Instantiate(prefabToInstantiate, roomPosition, Quaternion.identity, roomsClassify.transform);

            if (prefabToInstantiate == roomRewardPrefab)
                roomGenerated.GetComponent<RoomReward>().SetSwordPartToSpawn();

            // Make a double connection --> Room <-> RoomNode
            roomGenerated.GetComponent<Room>().thisRoomNode = roomNodeDictionary.Value;
            roomNodeDictionary.Value.SetRoomToThisNode(roomGenerated);
        }

        foreach(var roomNodeDictionary in roomPositions)
        {
            roomNodeDictionary.Value.roomGameObject.GetComponent<Room>().SetDoors();
            roomNodeDictionary.Value.roomGameObject.GetComponent<Room>().ChangeTextRoom();
        }
    }

    public void CheckRoomCleared()
    {
        enemiesKilled++;

        if(enemiesKilled == enemiesToKillOnActualRoom)
        {
            actualRoom.UnlockDoors();
        }
    }

    private void LoadFromSaveData(GameData saveData)
    {
        UI.Instance.LoadingScreenOn();

        actualLevel = saveData.actualLevel;

        if (actualLevel <= 2) actualDifficultyData = earlyLevels;
        else if (actualLevel <= 4) actualDifficultyData = middleLevels;
        else if (actualLevel <= 6) actualDifficultyData = finalLevels;
        else actualDifficultyData = postgameLevels;

        roomNodes = new List<RoomNode>();
        roomPositions = new Dictionary<Vector2Int, RoomNode>();

        foreach(var graphNode in saveData.graphNodes)
        {
            RoomNode roomNode = new RoomNode(graphNode.nodeId, graphNode.positionX, graphNode.positionY, graphNode.distance, graphNode.roomType, graphNode.enemiesCount, graphNode.roomCleared);

            roomNodes.Add(roomNode);

            roomPositions.Add(roomNode.position, roomNode);
        }

        foreach(var graphNodeDescendants in saveData.graphNodesDescendants)
        {
            int nodeParentId = graphNodeDescendants.nodeParentId;

            foreach(var descendantId in graphNodeDescendants.descendants)
            {
                roomNodes[nodeParentId].Connect(roomNodes[descendantId], true);
            }
        }

        InstanceRooms();

        // Set clear rooms
        foreach(var roomNode in roomNodes)
        {
            roomNode.roomGameObject.GetComponent<Room>().isCleared = roomNode.roomCleared;
        }

        actualRoom = roomNodes[saveData.actualRoom].roomGameObject.GetComponent<Room>();

        actualRoom.InitializeRoom();

        cameraPlayer.GetComponent<CameraController>().ChangeCameraPosition(roomNodes[saveData.actualRoom].roomGameObject);

        player.transform.position = roomNodes[saveData.actualRoom].roomGameObject.transform.position;

        player.GetComponent<PlayerController>().currentHealth = saveData.playerHealth;

        foreach(var inventoryObject in saveData.inventoryObjects)
        {
            SwordPartScripteable partScripteable = swordPartScripteablesList[inventoryObject.setNumber].columns[inventoryObject.partNumber];

            InventoryUI.Instance.AddObject(partScripteable);
        }

        // Partes equipadas
        foreach(var equippedObject in saveData.equippedObjects)
        {
            SwordPartScripteable partScripteable = swordPartScripteablesList[equippedObject.setNumber].columns[equippedObject.partNumber];

            InventoryUI.Instance.EquipPartSwordLoadGame(partScripteable);
        }

        seed = saveData.seed;
        Random.state = JsonUtility.FromJson<Random.State>(saveData.randomState);

        // Minimap reveal
        foreach(var roomNode in roomNodes)
        {
            if(roomNode.roomCleared)
                MinimapManager.Instance.RevealRoom(roomNode.position, roomNode);
        }

        MinimapManager.Instance.RevealRoom(actualRoom.thisRoomNode.position, actualRoom.thisRoomNode);

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SaveSystem.Save();
    }
}
