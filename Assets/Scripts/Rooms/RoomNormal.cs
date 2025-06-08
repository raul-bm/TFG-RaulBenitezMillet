using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomNormal : Room
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject[] enemySpawnPoints;

    private bool roomVisited = false;

    public override void InitializeRoom()
    {
        base.InitializeRoom();

        if (!roomVisited)
        {
            InitialLockedDoors();

            DungeonCrawlerController.Instance.enemiesToKillOnActualRoom = thisRoomNode.enemiesCount;
            DungeonCrawlerController.Instance.enemiesKilled = 0;

            System.Random random = new System.Random();

            for (int i = 0; i < thisRoomNode.enemiesCount; i++)
            {
                GameObject enemySpawnPoint = enemySpawnPoints[random.Next(enemySpawnPoints.Length)];

                Instantiate(enemyPrefab, enemySpawnPoint.transform.position, Quaternion.identity);
            }

            roomVisited = true;
        }
        else
        {
            InitialUnlockedDoors();
        }
    }
}
