using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomReward : Room
{
    [SerializeField] private GameObject swordPartPrefab;

    public override void InitializeRoom()
    {
        List<SwordPartScripteable> poolObjectsRewardRoom = DungeonCrawlerController.Instance.actualDifficultyData.poolObjectsRewardRoom;

        SwordPartScripteable swordPartToSpawn = poolObjectsRewardRoom[GameManager.Instance.rng.Next(poolObjectsRewardRoom.Count)];

        GameObject newGameObject = Instantiate(swordPartPrefab, this.transform.position, this.transform.rotation);
        newGameObject.GetComponent<SwordPartPickup>().SetSwordPart(swordPartToSpawn);
        newGameObject.transform.parent = this.transform;
    }
}
