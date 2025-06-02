    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRangeHitbox : MonoBehaviour
{
    [SerializeField] private PlayerController playerScript;
    [SerializeField] private Animator animator;

    public void ShowAttackAnimation()
    {
        animator.Play("SwordSlash");
    }

    public void EndAttackAnimation()
    {
        this.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            other.gameObject.GetComponent<EnemyBase>().TakeDamage(playerScript.playerStats.GetStat(StatType.Damage));
        }
        else if(other.gameObject.tag == "Boss")
        {
            other.gameObject.GetComponent<Boss>().TakeDamageBoss(playerScript.playerStats.GetStat(StatType.Damage));
        }
    }
}
