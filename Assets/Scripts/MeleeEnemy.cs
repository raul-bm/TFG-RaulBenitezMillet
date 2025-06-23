using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : EnemyBase
{
    public enum MeleeEnemyState { Idle, Chase, Attack }
    protected MeleeEnemyState currentState = MeleeEnemyState.Idle;

    protected override void Update()
    {
        base.Update();

        float distance = Vector2.Distance(transform.position, player.transform.position);

        switch (currentState)
        {
            case MeleeEnemyState.Idle:
                if (animator.GetBool("IsWalking")) animator.SetBool("IsWalking", false);

                if (distance < chaseRange)
                    currentState = MeleeEnemyState.Chase;
                break;

            case MeleeEnemyState.Chase:
                if (distance <= attackRange)
                    currentState = MeleeEnemyState.Attack;
                else if (distance >= chaseRange)
                    currentState = MeleeEnemyState.Idle;
                else if (!isDead)
                    MoveTowardsPlayer();
                break;

            case MeleeEnemyState.Attack:
                if (distance > attackRange)
                    currentState = MeleeEnemyState.Chase;
                else if (player.GetComponent<PlayerController>().isDead)
                    currentState = MeleeEnemyState.Idle;
                else if (Time.time - lastAttackTime > attackCooldown && !isDead)
                    Attack();
                break;
        }
    }

    protected void MoveTowardsPlayer()
    {
        if (!animator.GetBool("IsWalking")) animator.SetBool("IsWalking", true);

        Vector2 dir = (player.transform.position - transform.position).normalized;
        transform.position += (Vector3)(dir * moveSpeed * Time.deltaTime);
    }

    protected void Attack()
    {
        if (!player.GetComponent<PlayerController>().isDead)
        {
            animator.SetTrigger("IsAttacking");
            lastAttackTime = Time.time;

            player.GetComponent<PlayerController>().TakeDamage(damage);
        }
    }
}
