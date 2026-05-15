using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public static HealthManager Instance; // Singleton để dễ gọi từ script khác

    public float maxHealth = 1000f;
    public float currentHealth;

    public HealthBarUI healthBarUI; // Kéo thanh máu vào đây

    // THÊM: Tham chiếu đến GameManager
    private GameManager gameManager;

    // THÊM: Biến để tránh gọi Die nhiều lần
    private bool isDead = false;

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
        // SỬA: currentHealth phải = maxHealth, không phải 1
        currentHealth = maxHealth;
        UpdateUI();

        // Tìm GameManager trong scene
        gameManager = FindObjectOfType<GameManager>();

        isDead = false;
    }
    void Update()
    {
        // ✅ ĐỂ CODE TEST Ở ĐÂY - trong hàm Update()

        // TEST: Nhấn H để mất 100 máu (xóa sau khi test)
        /*if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(100f);
        }

        // TEST: Nhấn R để hồi 100 máu
        if (Input.GetKeyDown(KeyCode.R))
        {
            Heal(100f);
        }*/
    }
    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateUI();
        Debug.Log("Hồi máu: " + currentHealth + "/" + maxHealth);
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateUI();

        Debug.Log("Nhận sát thương: " + amount + " | Máu còn: " + currentHealth + "/" + maxHealth);

        if (currentHealth <= 0 && !isDead)
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
        if (isDead) return;

        isDead = true;
        Debug.Log("Thánh Gióng đã hy sinh!");

        // GỌI GAMEOVER TỪ GAMEMANAGER
        if (gameManager != null)
        {
            gameManager.GameOver();
        }
        else
        {
            Debug.LogError("Không tìm thấy GameManager trong scene!");
            // Fallback: tự dừng game
            Time.timeScale = 0f;
        }

        // Vô hiệu hóa PlayerController
        PlayerControl playerController = GetComponent<PlayerControl>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }
    }

    // Gọi hàm này khi qua map mới
    public void ResetHealthForNewMap()
    {
        currentHealth = maxHealth;
        isDead = false;
        UpdateUI();

        // Bật lại PlayerController
        PlayerControl playerController = GetComponent<PlayerControl>();
        if (playerController != null)
        {
            playerController.enabled = true;
        }

        Debug.Log("Reset máu cho map mới: " + currentHealth + "/" + maxHealth);
    }

    // Hàm kiểm tra còn sống không
    public bool IsAlive()
    {
        return !isDead && currentHealth > 0;
    }
}