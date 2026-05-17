using UnityEngine;

public class GiacAnAI : MonoBehaviour
{
    [Header("Chỉ số cơ bản")]
    public float speed = 2f;
    public float detectRange = 2f;
    public float attackRange = 1.2f;

    [Header("Giới hạn phạm vi")]
    public float patrolLimit = 5f;

    [Header("Attack")]
    public float attackCooldown = 1.5f;

    private float lastAttackTime;

    private Vector2 startPos;
    private int patrolDirection = -1;

    private Transform player;
    private Animator anim;
    private SpriteRenderer sprite;

    private float moveSpeed = 0f;
    private Vector3 lastPos;

    void Start()
    {
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();

        startPos = transform.position;
        lastPos = transform.position;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {

        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance < detectRange)
        {
            anim.SetBool("isChasing", true); // Bật trạng thái đuổi
            FacePlayer();
            HandleChaseAndAttack(distance);
        }
        else
        {
            anim.SetBool("isChasing", false); // Tắt trạng thái đuổi
            Patrol();
        }

        // Luôn cập nhật Speed để Animator biết đang di chuyển hay đứng yên
        anim.SetFloat("Speed", moveSpeed);
    }

    void Patrol()
    {
        transform.Translate(Vector2.right * patrolDirection * speed * Time.deltaTime);
        moveSpeed = speed;

        if (transform.position.x < startPos.x - patrolLimit)
            patrolDirection = 1;
        else if (transform.position.x > startPos.x + patrolLimit)
            patrolDirection = -1;
    }

    void HandleChaseAndAttack(float distance)
    {
        if (distance > attackRange)
        {
            Vector2 dir = (player.position - transform.position).normalized;

            transform.position += new Vector3(dir.x, 0, 0) * speed * 2f * Time.deltaTime;

            moveSpeed = 2f;

            // FLIP chuẩn theo hướng di chuyển
            if (dir.x > 0.01f)
                sprite.flipX = false;
            else if (dir.x < -0.01f)
                sprite.flipX = true;
        }
        else
        {
            moveSpeed = 0;

            if (Time.time - lastAttackTime > attackCooldown)
            {
                lastAttackTime = Time.time;
                anim.SetTrigger("attack");
            }
        }
    }
    void FacePlayer()
    {
        if (player == null) return;

        sprite.flipX = player.position.x < transform.position.x;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}