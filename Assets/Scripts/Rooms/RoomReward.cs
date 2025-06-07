using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomReward : Room
{
    [SerializeField] private GameObject swordPartPrefab;

    private SwordPartScripteable swordPartToSpawn;
    private bool swordPartSpawned = false;

    public override void InitializeRoom()
    {
        if(!swordPartSpawned)
        {
            GameObject newGameObject = Instantiate(swordPartPrefab, this.transform.position, this.transform.rotation);
            newGameObject.GetComponent<SwordPartPickup>().SetSwordPart(swordPartToSpawn);
            newGameObject.transform.parent = this.transform;

            swordPartSpawned = true;
        }

        UnlockDoors();
    }

    public void SetSwordPartToSpawn()
    {
        List<SwordPartScripteable> poolObjectsRewardRoom = DungeonCrawlerController.Instance.actualDifficultyData.poolObjectsRewardRoom;

        swordPartToSpawn = poolObjectsRewardRoom[GameManager.Instance.rng.Next(poolObjectsRewardRoom.Count)];
    }
}
