using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBoss : Room
{
    [SerializeField] private GameObject bossPrefab;

    public override void InitializeRoom()
    {
        if (!isCleared)
        {
            Instantiate(bossPrefab, transform.position, transform.rotation);
        }
    }
}
