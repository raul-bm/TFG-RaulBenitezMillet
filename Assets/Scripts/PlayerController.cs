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

    public SpriteRenderer pommelSpriterRenderer;
    public SpriteRenderer gripSpriteRenderer;
    public SpriteRenderer crossguardSpriteRenderer;
    public SpriteRenderer bladeSpriteRenderer;

    public bool isDead = false;

    private Rigidbody2D rb;
    private Animator animator;
    private DamageFlash damageFlash;

    private Vector2 moveInput;
    private Vector2 currentVelocity;
    private bool attacking;

    private float timerCooldown = 0f;

    private int lastHorizontalDirection = 1;

    // VALUES PLAYER
    public float currentHealth { get; private set; }
    public float maxHealth { get; private set; }
    public float attackBaseCooldown { get; private set; } = 2f;
    public float currentAttackCooldown { get; private set; } = 2f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        damageFlash = GetComponent<DamageFlash>();

        playerStats.InitBaseStats();

        currentHealth = 100f;
        maxHealth = 100f;
        moveSpeed = 5f;

        attackRangeHitbox.transform.localScale = new Vector3(playerStats.GetStat(StatType.AttackRange), playerStats.GetStat(StatType.AttackRange));

        UI.Instance.healthImageFiller.fillAmount = 1f;
    }

    private void Update()
    {
        // Attack cooldown
        if (attacking && timerCooldown <= currentAttackCooldown)
        {
            timerCooldown += Time.deltaTime;
            UI.Instance.attackCooldownImageFiller.fillAmount = timerCooldown / currentAttackCooldown;

            if (timerCooldown > currentAttackCooldown)
            {
                UI.Instance.attackCooldownImageFiller.fillAmount = 1f;
                attacking = false;
            }
        }

        //Movimiento
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        // Animacion movimiento horizontal
        if (moveX != 0)
        {
            lastHorizontalDirection = moveX > 0 ? 1 : -1;
            animator.SetBool("isWalking", true);
            animator.SetInteger("facing", lastHorizontalDirection);
        }
        // Animacion movimiento vertical
        else if(moveY != 0)
        {
            animator.SetBool("isWalking", true);
            animator.SetInteger("facing", lastHorizontalDirection);
        }
        // Animacion IDLE
        else
        {
            animator.SetBool("isWalking", false);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && !attacking && Time.timeScale != 0f && !isDead)
        {
            Attack();
        }
    }

    private void FixedUpdate()
    {
        if(!isDead)
        {
            Vector2 newPosition = rb.position + moveInput * moveSpeed * Time.fixedDeltaTime;

            rb.MovePosition(newPosition);
        }
    }

    private void Attack()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Vector3 direction = (mousePos - transform.position).normalized;

        lastHorizontalDirection = direction.x > 0 ? 1 : -1;
        animator.SetInteger("facing", lastHorizontalDirection);
        animator.SetTrigger("isAttacking");

        attackRangeHitbox.transform.position = transform.position + direction * distanceAttackHitboxFromPlayer;

        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        attackRangeHitbox.transform.rotation = Quaternion.Euler(0f, 0f, -angle);

        attackRangeHitbox.transform.localScale = new Vector3(-lastHorizontalDirection, 1, 1);

        attacking = true;
        UI.Instance.attackCooldownImageFiller.fillAmount = 0f;
        timerCooldown = 0f;
        attackRangeHitbox.SetActive(true);
        attackRangeHitbox.GetComponent<AttackRangeHitbox>().ShowAttackAnimation();
    }

    public void ApplySwordParts(List<SwordPartInventory> swordParts)
    {
        playerStats.ApplySwordParts(swordParts);

        pommelSpriterRenderer.sprite = null;
        gripSpriteRenderer.sprite = null;
        crossguardSpriteRenderer.sprite = null;
        bladeSpriteRenderer.sprite = null;

        foreach(var swordPart in swordParts)
        {
            Sprite spriteImage = swordPart.partScriptable.partImageGameObject;

            if (swordPart.partType == TypeSwordPart.Pommel) pommelSpriterRenderer.sprite = spriteImage;
            else if (swordPart.partType == TypeSwordPart.Grip) gripSpriteRenderer.sprite = spriteImage;
            else if (swordPart.partType == TypeSwordPart.Crossguard) crossguardSpriteRenderer.sprite = spriteImage;
            else if (swordPart.partType == TypeSwordPart.Blade) bladeSpriteRenderer.sprite = spriteImage;
        }

        currentAttackCooldown = attackBaseCooldown / playerStats.GetStat(StatType.AttackSpeed);

        // Change range
        attackRangeHitbox.transform.localScale = new Vector3(playerStats.GetStat(StatType.AttackRange), playerStats.GetStat(StatType.AttackRange));
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        UI.Instance.healthImageFiller.fillAmount = currentHealth / maxHealth;

        damageFlash.CallDamageFlash();

        if(currentHealth <= 0)
        {
            animator.Play("Player_Death");
            isDead = true;
        }
    }
}
