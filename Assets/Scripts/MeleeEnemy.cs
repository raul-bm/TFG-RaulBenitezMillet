using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : EnemyBase
{
    protected override void Attack()
    {
        base.Attack();
        Debug.Log("Melee attack!");
    }
}
