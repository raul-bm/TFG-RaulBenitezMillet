    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRangeHitbox : MonoBehaviour
{
    [SerializeField] private PlayerController playerScript;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            other.gameObject.GetComponent<Enemy>().TakeDamage(playerScript.damage);
        }
    }
}
