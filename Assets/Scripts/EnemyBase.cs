using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("Enemy Settings")]
    public float chaseRange = 5f;
    public float attackRange = 2f;
    public float moveSpeed = 2f;
    public float attackCooldown = 1.5f;
    public float health = 5f;
    public float damage = 20f;

    protected bool isDead = false;

    protected GameObject player;
    protected float lastAttackTime = 0f;

    protected Animator animator;

    protected DamageFlash damageFlash;

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();

        if(GetComponent<DamageFlash>() != null)
            damageFlash = GetComponent<DamageFlash>();
    }

    protected virtual void Update()
    {
        if (player == null) return;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        health -= damage;

        if (damageFlash != null)
            damageFlash.CallDamageFlash();

        if(health <= 0)
        {
            isDead = true;
            animator.SetTrigger("IsDead");

            DungeonCrawlerController.Instance.CheckRoomCleared();
        }
    }

    public void DestroyGameObjectAfterAnimation()
    {
        Destroy(this.gameObject);
    }
}
