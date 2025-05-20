using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private GameObject attackRangeHitbox;
    [SerializeField] private float distanceAttackHitboxFromPlayer;
    public CameraController cameraController;
    [SerializeField] private PlayerStats playerStats;

    private Rigidbody2D rb;

    private Vector2 moveInput;
    private Vector2 currentVelocity;
    private bool attacking;

    public float damage { get; private set; } = 1f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        if (Input.GetKeyDown(KeyCode.Mouse0) && !attacking && Time.timeScale != 0f) Attack();
    }

    private void FixedUpdate()
    {
        Vector2 newPosition = rb.position + moveInput * moveSpeed * Time.fixedDeltaTime;

        rb.MovePosition(newPosition);
    }

    private void Attack()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Vector3 direction = (mousePos - transform.position).normalized;

        attackRangeHitbox.transform.position = transform.position + direction * distanceAttackHitboxFromPlayer;

        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        attackRangeHitbox.transform.rotation = Quaternion.Euler(0f, 0f, -angle);

        StartCoroutine(AttackTimeout());
    }

    IEnumerator AttackTimeout()
    {
        attacking = true;
        attackRangeHitbox.SetActive(true);
        yield return new WaitForSeconds(0.5f); // Change for the attack speed of the player
        attacking = false;
        attackRangeHitbox.SetActive(false);
    }
}
