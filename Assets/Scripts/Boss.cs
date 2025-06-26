using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : EnemyBase
{
    public enum BossState { Phase1, Phase2, Dead }
    
    protected BossState currentState = BossState.Phase1;

    public GameObject pointerToPlayer;
    public GameObject[] attackGameobjects;

    private float healthToPhase2;

    private float timer = 0f;
    private bool cooldown = true;
    private float offsetDistance = 0.7f;

    private System.Random random = new System.Random();

    const float TIME_CHARGE_ATTACK_PHASE1 = 2.5f;
    const float TIME_BETWEEN_ATTACKS_PHASE1 = 2f;
    const float TIME_CHARGE_ATTACK_PHASE2 = 1f;
    const float TIME_BETWEEN_ATTACKS_PHASE2 = 1f;

    protected override void Start()
    {
        base.Start();

        healthToPhase2 = health / 2f;
    }

    protected override void Update()
    {
        base.Update();

        if(currentState != BossState.Dead)
        {
            switch (currentState)
            {
                case BossState.Phase1:
                    if (health <= healthToPhase2)
                    {
                        currentState = BossState.Phase2;
                        pointerToPlayer.SetActive(false);
                    }
                    else AttackPhase1();
                    
                    break;

                case BossState.Phase2:
                    if (health <= 0f) currentState = BossState.Dead;
                    else AttackPhase2();
                    break;
            }
        }
    }

    private void AttackPhase1()
    {
        timer += Time.deltaTime;

        if (cooldown && timer >= TIME_BETWEEN_ATTACKS_PHASE1)
        {
            cooldown = false;
            timer = 0f;
        }
        else if(!cooldown && timer <= TIME_CHARGE_ATTACK_PHASE1)
        {
            // Charging the attack animation and change direction
            if (!pointerToPlayer.activeSelf) pointerToPlayer.SetActive(true);
            Vector2 direction = (player.transform.position - pointerToPlayer.transform.position).normalized;
            float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            pointerToPlayer.transform.rotation = Quaternion.Euler(0, 0, -angle);

            Vector2 offset = pointerToPlayer.transform.up * offsetDistance;
            pointerToPlayer.transform.position = (Vector2)transform.position + offset;
        }
        else if(!cooldown && timer >= TIME_CHARGE_ATTACK_PHASE1)
        {
            pointerToPlayer.SetActive(false);
            // Launch attack
            attackGameobjects[0].GetComponent<BossAttackProjectile>().StartProjectile(pointerToPlayer.transform.position, pointerToPlayer.transform.rotation);

            cooldown = true;
            timer = 0f;
        }
    }

    private void AttackPhase2()
    {
        timer += Time.deltaTime;

        if (cooldown && timer >= TIME_BETWEEN_ATTACKS_PHASE2)
        {
            cooldown = false;
            timer = 0f;
        }
        else if (!cooldown && timer >= TIME_CHARGE_ATTACK_PHASE2)
        {
            Vector2[] crossDirections = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
            float[] otherCrossAngles = { 45f, 135f, 225f, 315f };

            bool isCrossAttack = random.Next(0, 2) == 0 ? true : false;

            // Cross attack launches
            if(!isCrossAttack)
            {
                for(int i = 0; i < crossDirections.Length; i++)
                {
                    float angle = Mathf.Atan2(crossDirections[i].x, crossDirections[i].y) * Mathf.Rad2Deg;
                    attackGameobjects[i].transform.rotation = Quaternion.Euler(0, 0, -angle);

                    Vector2 offset = crossDirections[i] * offsetDistance;
                    attackGameobjects[i].transform.position = (Vector2)transform.position + offset;

                    attackGameobjects[i].GetComponent<BossAttackProjectile>().StartProjectile(attackGameobjects[i].transform.position, attackGameobjects[i].transform.rotation);
                }
            }
            // X atack
            else
            {
                for (int i = 0; i < otherCrossAngles.Length; i++)
                {
                    float rad = otherCrossAngles[i] * Mathf.Deg2Rad;

                    Vector2 direction = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;

                    float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
                    attackGameobjects[i].transform.rotation = Quaternion.Euler(0, 0, -angle);

                    Vector2 offset = direction * offsetDistance;
                    attackGameobjects[i].transform.position = (Vector2)transform.position + offset;

                    attackGameobjects[i].GetComponent<BossAttackProjectile>().StartProjectile(attackGameobjects[i].transform.position, attackGameobjects[i].transform.rotation);
                }
            }

            cooldown = true;
            timer = 0f;
        }
    }
}
