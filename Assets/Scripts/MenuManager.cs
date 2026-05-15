using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Scene Names")]
    public string gameSceneName = "GameScene";

    [Header("Popup Confirm")]
    public GameObject confirmQuitPanel;  // Kéo ConfirmQuitPanel vào đây

    void Start()
    {
        // Đảm bảo popup ẩn khi bắt đầu
        if (confirmQuitPanel != null)
            confirmQuitPanel.SetActive(false);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    // Gọi khi bấm nút Quit (thay vì quit ngay)
    public void ShowConfirmQuit()
    {
        if (confirmQuitPanel != null)
            confirmQuitPanel.SetActive(true);

        // Tùy chọn: dừng thời gian nếu muốn (không cần thiết trong menu)
        // Time.timeScale = 0f;
    }

    // Gọi khi bấm "CÓ" trong popup
    public void ConfirmQuit()
    {
        Debug.Log("Thoát game!");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Gọi khi bấm "KHÔNG" trong popup
    public void CancelQuit()
    {   
        if (confirmQuitPanel != null)
            confirmQuitPanel.SetActive(false);

        // Time.timeScale = 1f; // Bỏ comment nếu có dừng thời gian
    }
}