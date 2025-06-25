using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomNormal : Room
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private List<GameObject> enemySpawnPoints;

    public override void InitializeRoom()
    {
        //Debug.Log("Ejecutado");

        base.InitializeRoom();

        if(!isInitialized)
        {
            isInitialized = true;

            if (!isCleared)
            {
                InitialLockedDoors();

                DungeonCrawlerController.Instance.enemiesToKillOnActualRoom = thisRoomNode.enemiesCount;
                DungeonCrawlerController.Instance.enemiesKilled = 0;

                System.Random random = new System.Random();

                //Debug.Log("AAAA: " + thisRoomNode.enemiesCount);

                for (int i = 0; i < thisRoomNode.enemiesCount; i++)
                {
                    //Debug.Log("Contador " + i);
                    //Debug.Log(enemySpawnPoints.Count);
                    int numEnemySpawnPoint = random.Next(enemySpawnPoints.Count);

                    GameObject enemySpawnPoint = enemySpawnPoints[numEnemySpawnPoint];

                    Instantiate(enemyPrefab, enemySpawnPoint.transform.position, Quaternion.identity);

                    enemySpawnPoints.RemoveAt(numEnemySpawnPoint);
                }
            }
            else
            {
                InitialUnlockedDoors();
            }
        }
    }
}
