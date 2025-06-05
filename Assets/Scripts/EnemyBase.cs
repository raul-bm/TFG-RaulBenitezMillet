using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public enum EnemyState { Idle, Chase, Attack}
    protected EnemyState currentState = EnemyState.Idle;

    [Header("Enemy Settings")]
    public float chaseRange = 5f;
    public float attackRange = 2f;
    public float moveSpeed = 2f;
    public float attackCooldown = 1.5f;
    public float health = 5f;
    public float damage = 20f;

    protected bool isDead = false;

    protected Transform player;
    protected float lastAttackTime = 0f;

    protected Animator animator;

    protected DamageFlash damageFlash;

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        animator = GetComponent<Animator>();

        currentState = EnemyState.Chase;

        if(GetComponent<DamageFlash>() != null)
            damageFlash = GetComponent<DamageFlash>();
    }

    protected virtual void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        switch (currentState)
        {
            case EnemyState.Idle:
                if (animator.GetBool("IsWalking")) animator.SetBool("IsWalking", false);

                if (distance < chaseRange)
                    currentState = EnemyState.Chase;
                break;

            case EnemyState.Chase:
                if (distance <= attackRange)
                    currentState = EnemyState.Attack;
                else if(!isDead)
                    MoveTowardsPlayer();
                break;

            case EnemyState.Attack:
                if (distance > attackRange)
                    currentState = EnemyState.Chase;
                else if (player.GetComponent<PlayerController>().isDead)
                    currentState = EnemyState.Idle;
                else if (Time.time - lastAttackTime > attackCooldown && !isDead)
                    Attack();
                break;
        }
    }

    protected virtual void MoveTowardsPlayer()
    {
        if (!animator.GetBool("IsWalking")) animator.SetBool("IsWalking", true);

        Vector2 dir = (player.position - transform.position).normalized;
        transform.position += (Vector3)(dir * moveSpeed * Time.deltaTime);
    }

    protected virtual void Attack()
    {
        if(!player.GetComponent<PlayerController>().isDead)
        {
            animator.SetTrigger("IsAttacking");
            lastAttackTime = Time.time;

            player.GetComponent<PlayerController>().TakeDamage(damage);
        }
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
        }
    }

    public void DestroyGameObjectAfterAnimation()
    {
        Destroy(this.gameObject);
    }
}
