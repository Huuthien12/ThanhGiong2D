using UnityEngine;

public class FireTrap : MonoBehaviour
{
    [Header("=== DAMAGE SETTINGS ===")]
    public int damageAmount = 100;           // Lượng sát thương mỗi lần
    public float damageCooldown = 1f;       // Thời gian giữa các lần gây sát thương

    [Header("=== EFFECTS ===")]
    public ParticleSystem fireParticles;    // Hiệu ứng lửa
    public AudioClip fireSound;             // Âm thanh lửa

    private float lastDamageTime;
    private bool isPlayerInFire = false;
    private GameObject currentPlayer;

    void Start()
    {
        // Tự động tìm ParticleSystem nếu chưa gán
        if (fireParticles == null)
            fireParticles = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (isPlayerInFire && currentPlayer != null)
        {
            // Kiểm tra thời gian để gây sát thương
            if (Time.time >= lastDamageTime + damageCooldown)
            {
                DealDamage(currentPlayer);
                lastDamageTime = Time.time;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInFire = true;
            currentPlayer = other.gameObject;
            lastDamageTime = Time.time - damageCooldown; // Gây sát thương ngay lập tức
            Debug.Log("🔥 Player bước vào lửa!");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInFire = false;
            currentPlayer = null;
            Debug.Log("🔥 Player ra khỏi lửa!");
        }
    }

    void DealDamage(GameObject player)
    {
        HealthManager health = player.GetComponent<HealthManager>();
        if (health != null)
        {
            health.TakeDamage(damageAmount);
            Debug.Log($"🔥 Lửa gây {damageAmount} sát thương!");

            // Hiệu ứng
            if (fireSound != null)
                AudioSource.PlayClipAtPoint(fireSound, transform.position);
        }
    }
}