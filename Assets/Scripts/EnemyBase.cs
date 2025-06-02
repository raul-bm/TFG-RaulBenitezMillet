using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public enum EnemyState { Idle, Chase, Attack}
    protected EnemyState currentState = EnemyState.Idle;

    [Header("Enemy Settings")]
    public float chaseRange = 5f;
    public float attackRange = 1f;
    public float moveSpeed = 2f;
    public float attackCooldown = 1.5f;
    public float health = 5f;

    protected Transform player;
    protected float lastAttackTime;

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        currentState = EnemyState.Chase;
    }

    protected virtual void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        switch (currentState)
        {
            case EnemyState.Idle:
                if (distance < chaseRange)
                    currentState = EnemyState.Chase;
                break;

            case EnemyState.Chase:
                if (distance < attackRange)
                    currentState = EnemyState.Attack;
                else
                    MoveTowardsPlayer();
                break;

            case EnemyState.Attack:
                if (distance > attackRange)
                    currentState = EnemyState.Chase;
                else if (Time.deltaTime - lastAttackTime > attackCooldown)
                    Attack();
                break;
        }
    }

    protected virtual void MoveTowardsPlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        transform.position += (Vector3)(dir * moveSpeed * Time.deltaTime);
    }

    protected virtual void Attack()
    {
        lastAttackTime = Time.time;
        Debug.Log("Enemy attacks!");
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0) Destroy(gameObject);
    }
}
