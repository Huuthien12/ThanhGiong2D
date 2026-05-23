using UnityEngine;

public class GiacAnAI : MonoBehaviour
{
    [Header("Chỉ số cơ bản")]
    public float speed = 2f;
    public float detectRange = 2f;
    public float attackRange = 1.2f;

    [Header("Máu và sát thương")]
    public int maxHealth = 3;
    private int currentHealth;
    public int damageToPlayer = 1;

    [Header("Giới hạn phạm vi di chuyển")]
    public float patrolLimit = 5f;
    public float patrolSpeed = 1.5f; // Tốc độ đi tuần riêng
    public float chaseSpeed = 3f;    // Tốc độ đuổi riêng

    [Header("Attack")]
    public float attackCooldown = 1.5f;

    private float lastAttackTime;

    private Vector2 startPos;
    private int patrolDirection = -1; // -1: trái, 1: phải

    private Transform player;
    private Animator anim;
    private SpriteRenderer sprite;
    private Rigidbody2D rb;

    private float currentMoveSpeed = 0f;

    [Header("Hiệu ứng")]
    public GameObject deathEffect;
    public GameObject hitEffect;

    private bool isDead = false;
    //private bool isChasing = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        currentHealth = maxHealth;
        startPos = transform.position;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        // Khởi tạo hướng di chuyển ban đầu
        patrolDirection = Random.Range(0, 2) == 0 ? -1 : 1;
    }

    void Update()
    {
        if (isDead) return;

        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Kiểm tra xem player có trong tầm phát hiện không
        if (distance < detectRange)
        {
            //isChasing = true;
            anim.SetBool("isChasing", true);
            HandleChaseAndAttack(distance);
        }
        else
        {
            //isChasing = false;
            anim.SetBool("isChasing", false);
            Patrol();
        }

        anim.SetFloat("Speed", currentMoveSpeed);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        anim.SetTrigger("Hurt");

        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }

        StartCoroutine(HurtFlash());

        Debug.Log(gameObject.name + " nhận " + damage + " sát thương. Máu: " + currentHealth + "/" + maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    System.Collections.IEnumerator HurtFlash()
    {
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        if (!isDead)
            sprite.color = Color.white;
    }

    void Die()
    {
        isDead = true;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        currentMoveSpeed = 0;
        if (rb != null) rb.linearVelocity = Vector2.zero;

        anim.SetTrigger("Die");
        anim.SetBool("isDead", true);

        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // Báo cho GameManager
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.QuaiBiTieuDiet();
        }

        Destroy(gameObject, 0.5f);
    }

    // =============================================
    // PATROL - Đi qua lại như thật
    // =============================================
    void Patrol()
    {
        // Di chuyển theo hướng patrolDirection
        transform.Translate(Vector2.right * patrolDirection * patrolSpeed * Time.deltaTime);
        currentMoveSpeed = patrolSpeed;

        // Đổi hướng khi đến giới hạn
        if (transform.position.x < startPos.x - patrolLimit)
        {
            patrolDirection = 1;
            // KHÔNG flip mặt ngay, chỉ đổi hướng di chuyển
        }
        else if (transform.position.x > startPos.x + patrolLimit)
        {
            patrolDirection = -1;
            // KHÔNG flip mặt ngay, chỉ đổi hướng di chuyển
        }

        // Quái LUÔN nhìn về hướng đang di chuyển khi patrol
        // Điều này làm quái trông tự nhiên hơn
        if (patrolDirection > 0)
            sprite.flipX = false;  // Nhìn phải
        else if (patrolDirection < 0)
            sprite.flipX = true;   // Nhìn trái
    }

    // =============================================
    // CHASE - Đuổi theo player
    // =============================================
    void HandleChaseAndAttack(float distance)
    {
        if (distance > attackRange)
        {
            // Đuổi theo player
            Vector2 dir = (player.position - transform.position).normalized;
            transform.position += new Vector3(dir.x, 0, 0) * chaseSpeed * Time.deltaTime;
            currentMoveSpeed = chaseSpeed;

            // CHỈ quay mặt về phía player khi đang đuổi
            if (dir.x > 0.01f)
                sprite.flipX = false;  // Nhìn phải
            else if (dir.x < -0.01f)
                sprite.flipX = true;   // Nhìn trái
        }
        else
        {
            currentMoveSpeed = 0;

            // Tấn công player
            if (Time.time - lastAttackTime > attackCooldown)
            {
                lastAttackTime = Time.time;
                anim.SetTrigger("attack");

                if (player != null)
                {
                    PlayerControl playerControl = player.GetComponent<PlayerControl>();
                    if (playerControl != null)
                    {
                        playerControl.TakeDamage(damageToPlayer);
                    }
                }
            }
        }
    }

    // =============================================
    // DEBUG - Vẽ gizmos
    // =============================================
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Vẽ đường patrol
        if (Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(new Vector3(startPos.x - patrolLimit, transform.position.y - 0.5f, 0),
                           new Vector3(startPos.x + patrolLimit, transform.position.y - 0.5f, 0));
        }
    }
}