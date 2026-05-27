using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("=== DI CHUYỂN ===")]
    public float moveSpeed = 2f;              // Tốc độ di chuyển
    public float detectionRange = 10f;        // Tầm nhìn (phát hiện player)
    public float attackRange = 1.5f;          // Tầm tấn công

    [Header("=== TẤN CÔNG ===")]
    public int damageToPlayer = 10;            // Sát thương gây ra
    public float attackCooldown = 1f;          // Thời gian giữa các đòn đánh
    private float lastAttackTime = 0f;

    [Header("=== GIỚI HẠN DI CHUYỂN ===")]
    public bool gioiHanDiChuyen = true;
    public float gioiHanTrai = -20f;           // Giới hạn trái
    public float gioiHanPhai = 20f;            // Giới hạn phải

    private Transform player;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Nếu chưa có Rigidbody2D, tự động thêm
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        // Tìm player nếu chưa có
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
    }

    void Update()
    {
        if (isDead) return;
        if (player == null) return;

        // Tính khoảng cách đến player
        float distance = Vector2.Distance(transform.position, player.position);

        // Nếu trong tầm phát hiện -> di chuyển về phía player
        if (distance <= detectionRange)
        {
            DiChuyenVePhiaPlayer();

            // Nếu trong tầm tấn công -> tấn công
            if (distance <= attackRange)
            {
                AttackPlayer();
            }
        }
        else
        {
            // Đứng yên nếu player ở xa
            DungYen();
        }

        // Cập nhật animation
        CapNhatAnimation();
    }

    void DiChuyenVePhiaPlayer()
    {
        // Tính hướng di chuyển
        Vector2 direction = (player.position - transform.position).normalized;

        // Di chuyển (chỉ theo trục X)
        Vector2 newPosition = rb.position;
        newPosition.x += direction.x * moveSpeed * Time.deltaTime;

        // Kiểm tra giới hạn
        if (gioiHanDiChuyen)
        {
            newPosition.x = Mathf.Clamp(newPosition.x, gioiHanTrai, gioiHanPhai);
        }

        rb.MovePosition(newPosition);

        // Flip mặt theo hướng di chuyển
        if (direction.x > 0)
            spriteRenderer.flipX = false;
        else if (direction.x < 0)
            spriteRenderer.flipX = true;
    }

    void DungYen()
    {
        // Đứng yên, giữ nguyên vị trí
        rb.linearVelocity = Vector2.zero;
    }

    void AttackPlayer()
    {
        // Kiểm tra cooldown
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;

            // Kích hoạt animation tấn công
            if (anim != null)
                anim.SetTrigger("Attack");

            // Gây sát thương lên player
            if (player != null)
            {
                PlayerControl playerControl = player.GetComponent<PlayerControl>();
                if (playerControl != null)
                {
                    playerControl.TakeDamage(damageToPlayer);
                    Debug.Log($"⚔️ Quái tấn công player, gây {damageToPlayer} sát thương!");
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

    void CapNhatAnimation()
    {
        if (anim == null) return;

        // Tính tốc độ di chuyển
        float currentSpeed = Mathf.Abs(rb.linearVelocity.x);
        anim.SetFloat("Speed", currentSpeed);
    }

    public void SetTarget(Transform target)
    {
        player = target;
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;

        // Phát hiệu ứng chết
        if (anim != null)
            anim.SetTrigger("Die");

        // Hủy object sau 0.5s
        Destroy(gameObject, 0.5f);
    }

    void OnDrawGizmosSelected()
    {
        // Vẽ tầm nhìn
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Vẽ tầm tấn công
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Vẽ giới hạn di chuyển
        if (gioiHanDiChuyen)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(new Vector3(gioiHanTrai, transform.position.y - 1f, 0),
                           new Vector3(gioiHanTrai, transform.position.y + 1f, 0));
            Gizmos.DrawLine(new Vector3(gioiHanPhai, transform.position.y - 1f, 0),
                           new Vector3(gioiHanPhai, transform.position.y + 1f, 0));
        }
    }
}