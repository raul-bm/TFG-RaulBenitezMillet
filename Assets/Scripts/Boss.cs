using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : EnemyBase
{
    public void TakeDamageBoss(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            // Make a new level
            Debug.Log("Llamando a generación desde el boss");
            DungeonCrawlerController.Instance.ProceduralGeneration();

            Destroy(gameObject);
        }
    }
}
