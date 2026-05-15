using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public static HealthManager Instance; // Singleton để dễ gọi từ script khác

    public float maxHealth = 1000f;
    public float currentHealth;

    public HealthBarUI healthBarUI; // Kéo thanh máu vào đây

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        currentHealth = 1f;
        UpdateUI();
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateUI();
        Debug.Log("Máu hiện tại: " + currentHealth + "/" + maxHealth);
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateUI()
    {
        if (healthBarUI != null)
            healthBarUI.UpdateBar(currentHealth, maxHealth);
    }

    void Die()
    {
        Debug.Log("Player đã chết!");
        // Code xử lý khi chết (respawn, game over...)
    }

    // Gọi hàm này khi qua map mới
    public void ResetHealthForNewMap()
    {
        currentHealth = maxHealth;
        UpdateUI();
        Debug.Log("Reset máu cho map mới: " + currentHealth + "/" + maxHealth);
    }
}