using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackProjectile : MonoBehaviour
{
    public float shootVelocity = 10f;
    public float damageDealt = 10f;
    private bool inMovement = false;

    public Rigidbody2D rb;
    public BoxCollider2D collider;

    private void Update()
    {
        if(inMovement)
        {
            rb.velocity = this.transform.up * shootVelocity;
        }
    }

    public void StartProjectile(Vector3 position, Quaternion rotation)
    {
        this.gameObject.SetActive(true);
        inMovement = true;

        transform.position = position;
        transform.rotation = rotation;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerController>().TakeDamage(damageDealt);
            this.gameObject.SetActive(false);
            inMovement = false;
        }
        else if(other.CompareTag("RoomColliders"))
        {
            this.gameObject.SetActive(false);
            inMovement = false;
        }
    }
}
