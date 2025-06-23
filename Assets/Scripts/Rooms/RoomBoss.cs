using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBoss : Room
{
    [SerializeField] private GameObject bossPrefab;

    public override void InitializeRoom()
    {
        base.InitializeRoom();

        if (!isCleared)
        {
            Instantiate(bossPrefab, transform.position, transform.rotation);

            DungeonCrawlerController.Instance.enemiesToKillOnActualRoom = 1;
            DungeonCrawlerController.Instance.enemiesKilled = 0;
        }
    }

    public override void UnlockDoors()
    {
        base.UnlockDoors();

        DungeonCrawlerController.Instance.ProceduralGeneration();
    }
}
