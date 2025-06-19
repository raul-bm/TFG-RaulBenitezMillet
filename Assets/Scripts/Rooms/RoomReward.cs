using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomReward : Room
{
    [SerializeField] private GameObject swordPartPrefab;

    private SwordPartScripteable swordPartToSpawn;

    public override void InitializeRoom()
    {
        base.InitializeRoom();

        if (!isCleared)
        {
            GameObject newGameObject = Instantiate(swordPartPrefab, this.transform.position, this.transform.rotation);
            newGameObject.GetComponent<SwordPartPickup>().SetSwordPart(swordPartToSpawn);
            newGameObject.transform.parent = this.transform;

            isCleared = true;
            thisRoomNode.RoomCleared();
        }

        InitialUnlockedDoors();
    }

    public void SetSwordPartToSpawn()
    {
        List<SwordPartScripteable> poolObjectsRewardRoom = DungeonCrawlerController.Instance.actualDifficultyData.poolObjectsRewardRoom;

        swordPartToSpawn = poolObjectsRewardRoom[Random.Range(0, poolObjectsRewardRoom.Count)];
    }
}
