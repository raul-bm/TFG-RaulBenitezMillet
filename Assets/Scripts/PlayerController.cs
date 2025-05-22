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
    public PlayerStats playerStats;

    private Rigidbody2D rb;

    private Vector2 moveInput;
    private Vector2 currentVelocity;
    private bool attacking;

    private float timerCooldown = 0f;

    // VALUES PLAYER
    public float currentHealth { get; private set; }
    public float attackBaseCooldown { get; private set; } = 2f;
    public float currentAttackCooldown { get; private set; } = 2f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        playerStats.InitBaseStats();

        currentHealth = 100f;
        moveSpeed = 5f;

        attackRangeHitbox.transform.localScale = new Vector3(attackRangeHitbox.transform.localScale.x, playerStats.GetStat(StatType.AttackRange));
    }

    private void Update()
    {
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        if(attacking && timerCooldown <= currentAttackCooldown)
        {
            timerCooldown += Time.deltaTime;
            UI.Instance.cooldownImageFiller.fillAmount = timerCooldown / currentAttackCooldown;

            if(timerCooldown > currentAttackCooldown)
            {
                UI.Instance.cooldownImageFiller.fillAmount = 1f;
                attacking = false;
            }
        }

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
        UI.Instance.cooldownImageFiller.fillAmount = 0f;
        timerCooldown = 0f;
        attackRangeHitbox.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        attackRangeHitbox.SetActive(false);
    }

    public void ApplySwordParts(List<SwordPartInventory> swordParts)
    {
        playerStats.ApplySwordParts(swordParts);

        currentAttackCooldown = attackBaseCooldown / playerStats.GetStat(StatType.AttackSpeed);

        // Change range
        attackRangeHitbox.transform.localScale = new Vector3(attackRangeHitbox.transform.localScale.x, playerStats.GetStat(StatType.AttackRange));
    }
}
