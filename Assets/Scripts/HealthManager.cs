using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public static HealthManager Instance;

    public float maxHealth = 1000f;
    public float currentHealth;

    private HealthBarUI healthBarUI;
    private GameManager gameManager;
    private bool isDead = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        currentHealth = maxHealth;
        FindHealthBarUI();
        UpdateUI();
        gameManager = FindFirstObjectByType<GameManager>();
        isDead = false;

        Debug.Log($"❤️ HealthManager khởi tạo: {currentHealth}/{maxHealth} máu");
    }

    void FindHealthBarUI()
    {
        // Tìm theo tên GameObject
        GameObject healthBarObj = GameObject.Find("HealthBar");

        // Nếu không thấy, tìm theo component
        if (healthBarObj == null)
        {
            healthBarUI = FindFirstObjectByType<HealthBarUI>();
            if (healthBarUI != null)
                healthBarObj = healthBarUI.gameObject;
        }

        // Lấy component HealthBarUI
        if (healthBarObj != null)
        {
            healthBarUI = healthBarObj.GetComponent<HealthBarUI>();
            if (healthBarUI != null)
            {
                Debug.Log("✅ Đã tìm thấy HealthBarUI (Filled Image): " + healthBarObj.name);
            }
        }
        else
        {
            Debug.LogWarning("⚠️ Không tìm thấy HealthBarUI trong scene!");
        }
    }

    void Update()
    {
        // TEST: Nhấn H để mất máu
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(100f);
        }

        // TEST: Nhấn R để hồi máu
        if (Input.GetKeyDown(KeyCode.R))
        {
            Heal(100f);
        }
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateUI();
        Debug.Log("💚 Hồi máu: " + currentHealth + "/" + maxHealth);
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateUI();

        Debug.Log("💔 Nhận sát thương: " + amount + " | Máu còn: " + currentHealth + "/" + maxHealth);

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    void UpdateUI()
    {
        if (healthBarUI != null)
        {
            healthBarUI.UpdateBar(currentHealth, maxHealth);
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log("💀 Thánh Gióng đã hy sinh!");

        if (gameManager != null)
        {
            gameManager.GameOver();
        }
        else
        {
            Time.timeScale = 0f;
        }

        PlayerControl playerController = GetComponent<PlayerControl>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }
    }

    public void ResetHealthForNewMap()
    {
        currentHealth = maxHealth;
        isDead = false;
        FindHealthBarUI();
        UpdateUI();

        PlayerControl playerController = GetComponent<PlayerControl>();
        if (playerController != null)
        {
            playerController.enabled = true;
        }

        Debug.Log("🔄 Reset máu cho map mới: " + currentHealth + "/" + maxHealth);
    }

    public bool IsAlive()
    {
        return !isDead && currentHealth > 0;
    }
}