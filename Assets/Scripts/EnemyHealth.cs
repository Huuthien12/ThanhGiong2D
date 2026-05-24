using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("=== THÔNG SỐ MÁU ===")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("=== LỰC VẬT LÝ ĐẨY LÙI (KNOCKBACK) ===")]
    public float knockbackForce = 6f;      // Lực hất văng ra sau
    public float knockbackDuration = 0.2f; // Thời gian bị khựng/văng

    private Rigidbody2D rb;
    private bool isKnockback = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }


    public void TakeDamage(int damage, Vector2 attackPosition)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} bị Thánh Gióng đấm! Máu còn: {currentHealth}");

        // Tạo lực đẩy vật lý (Knockback) hất quái ra sau
        if (rb != null)
        {
            // Hướng đẩy = Vị trí quái trừ đi vị trí nắm đấm (để quái bay ra xa)
            Vector2 knockbackDirection = ((Vector2)transform.position - attackPosition).normalized;

            // Ép quái bay theo phương ngang X, hơi nẩy lên phương Y một chút cho đẹp mắt
            knockbackDirection = new Vector2(knockbackDirection.x, 0.4f).normalized;

            StartCoroutine(ApplyKnockback(knockbackDirection));
        }

        // Nếu hết máu thì tiêu diệt quái
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator ApplyKnockback(Vector2 direction)
    {
        isKnockback = true;

        // Truyền lực vào Rigidbody2D của quái
#if UNITY_2023_1_OR_NEWER
        rb.linearVelocity = direction * knockbackForce;
#else
        rb.velocity = direction * knockbackForce;
#endif

        yield return new WaitForSeconds(knockbackDuration);

        // Hết thời gian văng, cho quái đứng im lại để nó tiếp tục AI/di chuyển
#if UNITY_2023_1_OR_NEWER
        rb.linearVelocity = Vector2.zero;
#else
        rb.velocity = Vector2.zero;
#endif
        isKnockback = false;
    }

    private void Die()
    {
        Debug.Log($"💥 Đã tiêu diệt thành công: {gameObject.name}!");
        Destroy(gameObject); // Xóa con quái khỏi Map
    }
}