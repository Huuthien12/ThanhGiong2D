using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("=== MÁU ===")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("=== DI CHUYỂN ===")]
    public float patrolSpeed = 1.5f;
    public float chaseSpeed = 3f;
    public float detectRange = 3f;
    public float attackRange = 1.2f;
    public float patrolLimit = 5f;

    [Header("=== TẤN CÔNG ===")]
    public float attackCooldown = 1.5f;
    public int damageToPlayer = 1;
    private float lastAttackTime;

    [Header("=== KNOCKBACK ===")]
    public float knockbackForce = 6f;
    public float knockbackDuration = 0.2f;
    private bool isKnockback = false;

    [Header("=== HIỆU ỨNG ===")]
    public GameObject deathEffect;
    public GameObject hitEffect;

    // Components
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;
    private Transform player;

    // Patrol
    private Vector2 startPos;
    private int patrolDirection = -1;
    private bool isDead = false;
    private bool isChasing = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();

        currentHealth = maxHealth;
        startPos = transform.position;

        // Tìm player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        // Hướng patrol ngẫu nhiên
        patrolDirection = Random.Range(0, 2) == 0 ? -1 : 1;
    }

    void Update()
    {
        if (isDead || isKnockback || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Kiểm tra player trong tầm phát hiện
        if (distance < detectRange)
        {
            isChasing = true;
            if (anim != null) anim.SetBool("isChasing", true);
            HandleChaseAndAttack(distance);
        }
        else
        {
            isChasing = false;
            if (anim != null) anim.SetBool("isChasing", false);
            Patrol();
        }

        // Cập nhật speed cho animation
        if (anim != null)
        {
            float currentSpeed = isChasing ? chaseSpeed : (isKnockback ? 0 : patrolSpeed);
            anim.SetFloat("Speed", currentSpeed);
        }
    }

    void HandleChaseAndAttack(float distance)
    {
        if (distance > attackRange)
        {
            // Đuổi theo player
            Vector2 dir = (player.position - transform.position).normalized;
            transform.position += new Vector3(dir.x, 0, 0) * chaseSpeed * Time.deltaTime;

            // Flip mặt theo hướng đuổi
            if (dir.x > 0.01f) sprite.flipX = false;
            else if (dir.x < -0.01f) sprite.flipX = true;
        }
        else
        {
            // Trong tầm đánh -> tấn công
            if (Time.time - lastAttackTime > attackCooldown)
            {
                lastAttackTime = Time.time;

                // Kích hoạt animation tấn công
                if (anim != null) anim.SetTrigger("attack");

                // Gây sát thương lên player
                if (player != null)
                {
                    PlayerControl playerControl = player.GetComponent<PlayerControl>();
                    if (playerControl != null)
                    {
                        playerControl.TakeDamage(damageToPlayer);
                        Debug.Log($"{gameObject.name} đã tấn công player gây {damageToPlayer} sát thương!");
                    }

                    // Nếu có HealthManager
                    HealthManager healthManager = player.GetComponent<HealthManager>();
                    if (healthManager != null)
                    {
                        healthManager.TakeDamage(damageToPlayer);
                    }
                }
            }
        }
    }

    void Patrol()
    {
        // Di chuyển tuần tra
        transform.Translate(Vector2.right * patrolDirection * patrolSpeed * Time.deltaTime);

        // Đổi hướng khi đến giới hạn
        if (transform.position.x < startPos.x - patrolLimit)
        {
            patrolDirection = 1;
        }
        else if (transform.position.x > startPos.x + patrolLimit)
        {
            patrolDirection = -1;
        }

        // Flip mặt theo hướng patrol
        if (patrolDirection > 0) sprite.flipX = false;
        else if (patrolDirection < 0) sprite.flipX = true;
    }

    public void TakeDamage(int damage, Vector2 attackPosition)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"{gameObject.name} nhận {damage} sát thương. Máu: {currentHealth}/{maxHealth}");

        // Hiệu ứng hit
        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }

        // Animation bị thương
        if (anim != null) anim.SetTrigger("Hurt");

        // Hiệu ứng nhấp nháy đỏ
        StartCoroutine(HurtFlash());

        // Knockback (đẩy lùi)
        if (rb != null)
        {
            Vector2 knockbackDirection = ((Vector2)transform.position - attackPosition).normalized;
            knockbackDirection = new Vector2(knockbackDirection.x, 0.4f).normalized;
            StartCoroutine(ApplyKnockback(knockbackDirection));
        }

        // Kiểm tra chết
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator ApplyKnockback(Vector2 direction)
    {
        isKnockback = true;

        // Dừng AI tạm thời
        if (anim != null) anim.SetFloat("Speed", 0);

        // Áp dụng lực đẩy
#if UNITY_2023_1_OR_NEWER
        rb.linearVelocity = direction * knockbackForce;
#else
        rb.velocity = direction * knockbackForce;
#endif

        yield return new WaitForSeconds(knockbackDuration);

        // Kết thúc knockback
#if UNITY_2023_1_OR_NEWER
        rb.linearVelocity = Vector2.zero;
#else
        rb.velocity = Vector2.zero;
#endif
        isKnockback = false;
    }

    IEnumerator HurtFlash()
    {
        if (sprite != null)
        {
            Color originalColor = sprite.color;
            sprite.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            if (!isDead) sprite.color = originalColor;
        }
    }

    void Die()
    {
        isDead = true;

        Debug.Log($"💀 {gameObject.name} đã chết!");

        // Hiệu ứng chết
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // Tắt collider
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // Dừng di chuyển
        if (rb != null) rb.linearVelocity = Vector2.zero;

        // Animation chết
        if (anim != null)
        {
            anim.SetTrigger("Die");
            anim.SetBool("isDead", true);
        }

        // Báo cho GameManager để cập nhật nhiệm vụ
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.QuaiBiTieuDiet();
        }
        EnemyMovement movement = GetComponent<EnemyMovement>();
        if (movement != null)
        {
            movement.Die();
        }
        // Hủy object sau một thời gian
        Destroy(gameObject, 0.5f);
    }

    // Vẽ gizmos để debug
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(new Vector3(startPos.x - patrolLimit, transform.position.y - 0.5f, 0),
                           new Vector3(startPos.x + patrolLimit, transform.position.y - 0.5f, 0));
        }
    }
}