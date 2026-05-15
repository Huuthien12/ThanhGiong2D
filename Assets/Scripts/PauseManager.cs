using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject pausePanel;  // Kéo PausePanel vào

    [Header("Settings")]
    public KeyCode pauseKey = KeyCode.Escape;  // Phím bấm để pause
    public bool isPaused = false;

    void Update()
    {
        // Nhấn ESC để pause/resume
        if (Input.GetKeyDown(pauseKey))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;  // Đóng băng game
        pausePanel.SetActive(true);

        // Hiện con trỏ chuột (quan trọng khi pause)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Dừng âm thanh (nếu muốn)
        // AudioListener.pause = true;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;  // Chạy lại game
        pausePanel.SetActive(false);

        // Ẩn con trỏ (nếu game dùng chuột ngắm)
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;

        // AudioListener.pause = false;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;  // QUAN TRỌNG: reset time trước khi restart
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;  // Reset time
        SceneManager.LoadScene("MenuScene");
    }
}