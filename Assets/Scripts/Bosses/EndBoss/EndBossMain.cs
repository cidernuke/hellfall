using System.Collections;
using UnityEngine;

public class EndBossMain : MonoBehaviour
{
    private Transform player;
    private Collider2D playerCollider;

    // Moving Attack
    [SerializeField] private int attackDamage;
    [SerializeField] private Vector3 attackOffset;
    public float attackRange; // radius for attack range

    // Charge Attack
    [SerializeField] private int areaAttackDamage; // Damage dealt by the attack
    [SerializeField] private float chargeSpeed;
    [SerializeField] private float chargeDuration;
    private float chargeTimer = 0f;
    public float chargeAttackCooldown;
    [HideInInspector] public float chargeAttackcooldownTimer = Mathf.Infinity;

    // Vanishing Area Attack
    [SerializeField] private int chargeAttackDamage; // Damage dealt by the attack
    [SerializeField] private Vector2 areaAttackboxSize; // Width and height of the box
    [SerializeField] private Vector2 areaAttackboxOffset; // Offset from the boss's position
    public float vanishAreaAttackCooldown;
    [HideInInspector] public float vanishAttackcooldownTimer = Mathf.Infinity;

    // Flags
    [HideInInspector] public bool isFlipped = false;
    [HideInInspector] public bool isInSecondPhase = false;
    [HideInInspector] public bool isCharging = false;
    [HideInInspector] public bool isAreaAttack = false;
    
    // References
    [SerializeField] private LayerMask attackMask;
    private Rigidbody2D rb;
    private Animator animator;
    private Collider2D bossCollider; // Reference to the boss's collider

    public BossHealthBar healthBar;
    public HealthSystem healthSystem;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        bossCollider = GetComponent<BoxCollider2D>();
        playerCollider = player.GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        healthSystem = gameObject.GetComponent<HealthSystem>();

        healthBar.SetMaxHealth((int)healthSystem.startingHealth);
    }

    private void Update()
    {
        chargeAttackcooldownTimer += Time.deltaTime;
        vanishAttackcooldownTimer += Time.deltaTime;

        // if (gameObject.GetComponent<HealthSystem>().currentHealth == 10)
        // {
        //     isInSecondPhase = true;
        // }
        if (healthSystem.currentHealth < healthSystem.startingHealth)
        {
            print("should be taking damage");
            // healthSystem.TakeDamage(5);
            healthBar.SetHealth((int)healthSystem.currentHealth);
        }
        if (isCharging)
        {
            ChargeTowardsPlayer();
        }
    }

    public void StartCharge()
    {
        isCharging = true;
        chargeTimer = chargeDuration;
        animator.SetTrigger("chargeAttack");
    }

    public void LookAtPlayer(Transform player)
    {
        Vector3 flipped = transform.localScale;

        // boss is left of player
        if (transform.position.x < player.position.x && isFlipped)
        {
            flipped.x *= -1f;
            transform.localScale = flipped;
            isFlipped = false;
        }
        // boss is right of player
        else if (transform.position.x > player.position.x && !isFlipped)
        {
            flipped.x *= -1f;
            transform.localScale = flipped;
            isFlipped = true;
        }
    }

    public void Attack()
    {
        Vector3 pos = SetAttackPosition();
        // print("pos in attack: "+pos);
        Collider2D colInfo = Physics2D.OverlapCircle(pos, attackRange, attackMask);
        print("attacking player, collider hit: " + colInfo + ", attack damage: " + attackDamage);
        if (colInfo != null)
        {
            print("attack hit");
            PlayerMovement playerMovement = colInfo.GetComponent<PlayerMovement>();
            colInfo.GetComponent<HealthSystem>().TakeDamage(attackDamage, playerMovement);
        }
    }

    private void ChargeTowardsPlayer()
    {
        // Calculate direction towards the player
        // Vector2 direction = (player.position - transform.position).normalized;
        Vector2 direction = isFlipped ? new Vector2(-1, 0) : new Vector2(1, 0);

        // Move the boss
        rb.velocity = direction * chargeSpeed;
        // rb.AddForce(direction * chargeSpeed);

        // Decrease the charge timer
        chargeTimer -= Time.deltaTime;

        // Check for collisions
        Vector3 pos = SetAttackPosition();
        Collider2D hit = Physics2D.OverlapCircle(pos, 1f, attackMask);
        if (hit != null)
        {
            if (hit.CompareTag("Player"))
            {
                // Deal damage to the player
                PlayerMovement playerMovement = hit.GetComponent<PlayerMovement>();
                hit.GetComponent<HealthSystem>().TakeDamage(chargeAttackDamage, playerMovement);
                StopCharge();
            }
            else
            {
                // Stop if hitting a wall or other obstacle
                print("hit nothing");
                StopCharge();
            }
        }

        // End the charge after the timer runs out
        if (chargeTimer <= 0)
        {
            print("timer ran out");
            StopCharge();
        }
    }

    private void StopCharge()
    {
        isCharging = false;
        rb.velocity = Vector2.zero; // Stop the boss movement
        animator.ResetTrigger("chargeAttack");
        animator.SetTrigger("returnToMoving"); // Return to idle or other states
    }

    public void InitiateVanishingPhaseThree()
    {
        if (isAreaAttack)
        {
            animator.SetTrigger("initiateVanishing_03");
            animator.SetBool("isVanishing", isAreaAttack);
        }
    }

    public void InitiateVanishingAndReappearance()
    {
        if (isAreaAttack)
        {
            animator.SetBool("isVanishing", false);
            StartCoroutine(TeleportAfterDelay(1f));
        }
    }

    private IEnumerator TeleportAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        gameObject.transform.position = new Vector2(player.position.x, -1);
        animator.SetTrigger("teleportedToPlayer");
        animator.SetBool("isAreaAttacking", true);
    }

    //! Adjust dashframe and possible dashes in PlayerMovement for balancing
    public void AreaAttack()
    {
        if (isAreaAttack)
        {
            Physics2D.IgnoreCollision(bossCollider, playerCollider, true);
            Vector2 boxPosition = (Vector2)transform.position + areaAttackboxOffset;

            // Find all targets in the box area
            Collider2D[] hitTargets = Physics2D.OverlapBoxAll(boxPosition, areaAttackboxSize, 0f, attackMask);

            // Deal damage to each target
            foreach (Collider2D target in hitTargets)
            {
                // if (target.TryGetComponent(out HealthSystem health))
                if (target.CompareTag("Player"))
                {
                    target.GetComponent<HealthSystem>().TakeDamage(areaAttackDamage, target.GetComponent<PlayerMovement>());
                    // Debug.Log($"Damaged {target.tag} for {areaAttackDamage} HP.");
                }
            }

            Physics2D.IgnoreCollision(bossCollider, playerCollider, false);
            animator.SetBool("isAreaAttacking", false);
            isAreaAttack = false;
        }
    }

    private Vector3 SetAttackPosition()
    {
        Vector3 pos = transform.position;
        pos += transform.right * attackOffset.x * (isFlipped ? -1 : 1);
        pos += transform.up * attackOffset.y;
        return pos;
    }

    void OnDrawGizmosSelected()
    {
        Vector3 pos = transform.position;
        pos += transform.right * attackOffset.x * (isFlipped ? -1 : 1);
        pos += transform.up * attackOffset.y;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pos, attackRange);

        Vector2 boxPosition = (Vector2)transform.position + areaAttackboxOffset;
        Gizmos.DrawWireCube(boxPosition, areaAttackboxSize);
    }

    public void PrinterForBossRun(string message)
    {
        print(message);
    }

    public void PrinterMessage()
    {
        print("isVanishing: " + animator.GetBool("isVanishing"));
    }
}

