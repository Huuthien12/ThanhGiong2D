using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;

    // Hệ thống tấn công COMBO
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayer;
    public int attackDamage = 1;

    // Combo settings
    public float comboWindow = 0.5f;
    public float attackCooldown = 0.8f;
    private int currentComboStep = 0;
    private float lastAttackTime = 0f;
    private bool isAttacking = false;
    private bool canComboNext = false;

    // Animation Timing
    [Header("Animation Timing")]
    public float attack1Duration = 0.35f;
    public float attack2Duration = 0.5f;
    public float attack3Duration = 0.7f;

    public float attack1HitTiming = 0.15f;  
    public float attack2HitTiming = 0.2f;
    public float attack3HitTiming = 0.25f;

    // Hệ thống máu
    public int maxHealth = 5;
    private int currentHealth;
    public float invincibilityDuration = 1f;
    private bool isInvincible = false;
    private float invincibilityTimer = 0f;

    // UI và hiệu ứng
    public HealthBarUI healthBarUI;
    public GameObject hitEffect;
    public GameObject deathEffect;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private bool isGrounded;
    private float moveInput;
    private bool facingRight = true;
    private bool isClimbing = false;
    private bool hasJumped = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        currentHealth = maxHealth;

        if (healthBarUI != null)
        {
            healthBarUI.UpdateBar(currentHealth, maxHealth);
        }
    }

    void Update()
    {
        if (Time.timeScale == 0f)
            return;

        // Cập nhật trạng thái grounded
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Xử lý invincibility frames
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0f)
            {
                isInvincible = false;
                if (spriteRenderer != null)
                    spriteRenderer.color = Color.white;
            }
        }

        // KHÔNG return ở đây nữa - vẫn cho phép combo khi đang tấn công

        // Input di chuyển
        moveInput = Input.GetAxisRaw("Horizontal");

        // XỬ LÝ NHẢY
        if (!isClimbing && !isAttacking)  // Chỉ cho nhảy khi không tấn công
        {
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !hasJumped)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                hasJumped = true;
                anim.SetBool("IsGrounded", false);
            }

            if (isGrounded && rb.linearVelocity.y <= 0)
            {
                hasJumped = false;
                anim.SetBool("IsGrounded", true);
            }
        }

        // Xử lý tấn công COMBO - Dùng phím F
        if (Input.GetKeyDown(KeyCode.F))
        {
            HandleAttack();
        }

        // Flip nhân vật - chỉ khi không tấn công
        if (!isAttacking)
        {
            if (moveInput > 0 && !facingRight) Flip();
            else if (moveInput < 0 && facingRight) Flip();
        }

        // Animation
        anim.SetFloat("Speed", !isAttacking ? Mathf.Abs(moveInput) : 0f);
    }

    void HandleAttack()
    {
        Debug.Log($"=== HandleAttack ===");
        Debug.Log($"Time: {Time.time}, lastAttackTime: {lastAttackTime}, Cooldown check: {Time.time >= lastAttackTime + attackCooldown}");

        if (Time.time < lastAttackTime + attackCooldown)
        {
            Debug.Log("Đang trong cooldown, không thể attack");
            return;
        }

        Debug.Log($"isAttacking: {isAttacking}, canComboNext: {canComboNext}, currentComboStep: {currentComboStep}");

        // QUAN TRỌNG: Kiểm tra combo trước
        if (canComboNext && currentComboStep < 3)
        {
            // Đang trong combo window -> chuyển sang đòn tiếp theo
            currentComboStep++;
            Debug.Log($"✓ Combo tiếp! Chuyển sang step: {currentComboStep}");
            PerformAttack();
        }
        else if (!isAttacking)
        {
            // Bắt đầu combo mới
            currentComboStep = 1;
            Debug.Log($"✓ Bắt đầu combo mới, step: {currentComboStep}");
            PerformAttack();
        }
        else
        {
            Debug.Log($"✗ Không thể attack! isAttacking={isAttacking}, canComboNext={canComboNext}");
        }
    }

    void PerformAttack()
    {
        isAttacking = true;
        canComboNext = false;

        Debug.Log($"PerformAttack - Step {currentComboStep}");

        // Reset combo bools (CHỈ reset IsCombo2 và IsCombo3, không có IsCombo4)
        anim.SetBool("IsCombo2", false);
        anim.SetBool("IsCombo3", false);

        switch (currentComboStep)
        {
            case 1:
                anim.SetTrigger("Attack1");
                Debug.Log("Trigger Attack1 animation");
                StartCoroutine(HandleAttackSequence(attack1Duration, attack1HitTiming, 1));
                break;
            case 2:
                anim.SetBool("IsCombo2", true);
                anim.SetTrigger("Attack2");
                Debug.Log("Trigger Attack2 animation with IsCombo2=true");
                StartCoroutine(HandleAttackSequence(attack2Duration, attack2HitTiming, 2));
                break;
            case 3:
                anim.SetBool("IsCombo3", true);
                anim.SetTrigger("Attack3");
                Debug.Log("Trigger Attack3 animation with IsCombo3=true");
                StartCoroutine(HandleAttackSequence(attack3Duration, attack3HitTiming, 3));
                break;
        }
    }

    IEnumerator HandleAttackSequence(float animDuration, float hitTiming, int comboStep)
    {
        yield return new WaitForSeconds(hitTiming);
        DealDamage(comboStep);

        float remainingTime = animDuration - hitTiming;
        yield return new WaitForSeconds(remainingTime);

        Debug.Log($"Kết thúc animation step {comboStep}");

        // QUAN TRỌNG: Chỉ tắt isAttacking nếu không còn combo window nữa
        if (comboStep < 3)
        {
            // Mở combo window nhưng vẫn giữ isAttacking = true để chờ combo tiếp
            Debug.Log($"Mở combo window cho step {comboStep + 1}");
            StartCoroutine(ComboWindowCoroutine());
        }
        else
        {
            // Đòn cuối, kết thúc hoàn toàn
            isAttacking = false;
            ResetCombo();
            lastAttackTime = Time.time;
            Debug.Log("Hoàn thành full combo, reset");
        }
    }

    IEnumerator ComboWindowCoroutine()
    {
        canComboNext = true;
        float timer = 0f;

        Debug.Log($"Combo window bắt đầu, sẽ kéo dài {comboWindow} giây");

        while (timer < comboWindow)
        {
            if (!canComboNext)
            {
                Debug.Log("Combo window kết thúc sớm vì đã nhận đòn tiếp theo");
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // Hết thời gian, không có combo tiếp theo
        if (canComboNext)
        {
            Debug.Log("Hết thời gian combo window, reset combo");
            isAttacking = false;
            ResetCombo();
            lastAttackTime = Time.time;
        }
    }

    void DealDamage(int comboStep)
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            GiacAnAI enemyHealth = enemy.GetComponent<GiacAnAI>();
            if (enemyHealth != null)
            {
                int damage = attackDamage * comboStep;
                enemyHealth.TakeDamage(damage);

                if (hitEffect != null)
                    Instantiate(hitEffect, enemy.transform.position, Quaternion.identity);

                Debug.Log($"Hit {enemy.name} - Đòn {comboStep}, sát thương: {damage}");
            }
        }
    }

    void ResetCombo()
    {
        Debug.Log("Reset combo");
        currentComboStep = 0;
        canComboNext = false;

        // Reset all combo bools (CHỈ reset IsCombo2 và IsCombo3)
        anim.SetBool("IsCombo2", false);
        anim.SetBool("IsCombo3", false);
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        currentHealth -= damage;

        if (healthBarUI != null)
        {
            healthBarUI.UpdateBar(currentHealth, maxHealth);
        }

        if (hitEffect != null)
            Instantiate(hitEffect, transform.position, Quaternion.identity);

        isInvincible = true;
        invincibilityTimer = invincibilityDuration;

        StartCoroutine(FlashEffect());
        anim.SetTrigger("Hurt");

        ResetCombo();
        isAttacking = false;  // Reset trạng thái tấn công khi bị thương

        Debug.Log("Player nhận " + damage + " sát thương. Máu: " + currentHealth + "/" + maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    System.Collections.IEnumerator FlashEffect()
    {
        if (spriteRenderer == null) yield break;

        float elapsed = 0f;
        while (elapsed < invincibilityDuration)
        {
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.2f;
        }
        spriteRenderer.color = Color.white;
    }

    void Die()
    {
        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        enabled = false;
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        anim.SetTrigger("Die");
        // TÌM GAMEMANAGER VÀ GỌI GAME OVER
        GameManager gm = FindFirstObjectByType<GameManager>();
        if (gm != null)
        {
            gm.PlayerDied();
        }
        Debug.Log("Player đã chết!");
    }

    public void SetClimbing(bool climbing)
    {
        isClimbing = climbing;
    }

    void FixedUpdate()
    {
        if (isAttacking) return;

        if (!isClimbing)
        {
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);

        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}