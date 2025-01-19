using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndBossMain : MonoBehaviour
{
    // [SerializeField] private Transform firePoint;
    private Transform player;
    public int attackDamage = 1;
    // public int enragedAttackDamage = 40;

    public Vector3 attackOffset;
    public float attackRange = 1f;
    public LayerMask attackMask;

    public float chargeAttackCooldown;
    [HideInInspector] public float cooldownTimer = Mathf.Infinity;
    private readonly float groundCoordinates = 29.7f; // Spawn coordinates for blood_bullets_up
    [HideInInspector] public bool isFlipped = false;
    public bool isInSecondPhase = false;
    [SerializeField] private float chargeSpeed = 5f;
    [SerializeField] private float chargeDuration = 2f;

    public bool isCharging = false;
    public bool isAreaAttack = false;
    private float chargeTimer = 0f;
    public int areaAttackDamage = 30; // Damage dealt by the attack
    public Vector2 areaAttackboxSize = new(5f, 3f); // Width and height of the box
    public Vector2 areaAttackboxOffset = new(2f, 0f); // Offset from the boss's position

    private Rigidbody2D rb;
    private Animator animator;
    public Collider2D bossCollider; // Reference to the boss's collider
    public Collider2D playerCollider;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        bossCollider = GetComponent<BoxCollider2D>();
        playerCollider = player.GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        // if (gameObject.GetComponent<HealthSystem>().currentHealth == 10)
        // {
        //     isInSecondPhase = true;
        // }
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
        Collider2D colInfo = Physics2D.OverlapCircle(pos, attackRange, attackMask);
        // print("attacking player, collider hit: " + colInfo + ", attack damage: " + attackDamage);
        if (colInfo != null)
        {
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
        //! not very fast, slows down when nearing player. Also boss floats upwards
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
                print("hit player");
                // Deal damage to the player
                // PlayerMovement playerMovement = hit.GetComponent<PlayerMovement>();
                hit.GetComponent<HealthSystem>().TakeDamage(attackDamage);
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

    //* needs to be triggered in anim controller
    // public void InitiateVanishingPhaseOne()
    // {
    //     animator.SetTrigger("initiateVanishing_01");
    // }

    // public void InitiateVanishingPhaseTwo()
    // {
    //     animator.SetTrigger("initiateVanishing_02");
    // }

    //! zeichne problem auf. Problem ist dass isAreaAttack entweder immer true ist oder zu spät auf false gesetzt wird
    public void InitiateVanishingPhaseThree()
    {
        if (isAreaAttack)
        {
            print("initializing phase 3");
            animator.SetTrigger("initiateVanishing_03");
            animator.SetBool("isVanishing", isAreaAttack);
        }
    }

    public void InitiateVanishingAndReappearance()
    {
        if (isAreaAttack)
        {
            // print("teleporting to player");
            // animator.ResetTrigger("initiateVanishing_03");
            // gameObject.SetActive(false);
            animator.SetBool("isVanishing", false);
            // animator.SetTrigger("teleportedToPlayer");

            StartCoroutine(TeleportAfterDelay(1f));
            // rb.transform.position = player.position;
        }
    }

    private IEnumerator TeleportAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        print("Teleporting to player...");
        // gameObject.SetActive(true);
        gameObject.transform.position = new Vector2(player.position.x, -1);
        animator.SetTrigger("teleportedToPlayer");
        animator.SetBool("isAreaAttacking", true);
    }

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
                    target.GetComponent<HealthSystem>().TakeDamage(attackDamage, target.GetComponent<PlayerMovement>());
                    Debug.Log($"Damaged {target.tag} for {attackDamage} HP.");
                }
            }

            Physics2D.IgnoreCollision(bossCollider, playerCollider, false);
            animator.SetBool("isAreaAttacking", false);
            isAreaAttack = false;
        }
    }

    // public void EnragedAttack()
    // {
    //     Vector3 pos = transform.position;
    //     pos += transform.right * attackOffset.x;
    //     pos += transform.up * attackOffset.y;

    //     Collider2D colInfo = Physics2D.OverlapCircle(pos, attackRange, attackMask);
    //     if (colInfo != null)
    //     {
    //         colInfo.GetComponent<HealthSystem>().TakeDamage(enragedAttackDamage);
    //     }
    // }

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

