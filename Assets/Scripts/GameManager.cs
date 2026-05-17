using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject gameOverPanel;

    [Header("Player")]
    public HealthManager playerHealth;  // Đổi từ PlayerHealth sang HealthManager

    void Start()
    {
        // Ẩn GameOverPanel khi bắt đầu game
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // Tìm HealthManager nếu chưa gán
        if (playerHealth == null)
        {
            playerHealth = FindFirstObjectByType<HealthManager>();

            if (playerHealth == null)
            {
                Debug.LogError("KHÔNG TÌM THẤY HealthManager! Hãy tạo GameObject và gắn script HealthManager vào.");
            }
            else
            {
                Debug.Log("Đã tìm thấy HealthManager: " + playerHealth.gameObject.name);
            }
        }

    }

    void Update()
    {
        // Không cần cập nhật UI máu ở đây nữa (HealthManager tự lo)

        // Kiểm tra player chết thông qua HealthManager
        if (playerHealth != null && !playerHealth.IsAlive())
        {
            // Tránh gọi GameOver nhiều lần
            if (!gameOverPanel.activeSelf)
            {
                GameOver();
            }
        }
    }

    public void GameOver()
    {
        if (gameOverPanel.activeSelf) return;

        Time.timeScale = 0f;  // Dừng game
        gameOverPanel.SetActive(true);

        // Hiện con trỏ chuột
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Game Over Panel hiển thị!");
    }

    public void RetryGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuScene");
    }
}