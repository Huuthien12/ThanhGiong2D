using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Scene Names")]
    public string gameSceneName = "CutsceneScene"; // Tên scene chứa video riêng biệt

    [Header("Popup Confirm")]
    public GameObject confirmQuitPanel;  // Kéo ConfirmQuitPanel của bạn vào đây

    void Start()
    {
        // Đảm bảo popup ẩn khi bắt đầu
        if (confirmQuitPanel != null)
            confirmQuitPanel.SetActive(false);
    }

    // Hàm này gắn vào nút BẮT ĐẦU (btnStart)
    public void StartGame()
    {
        // Gọi thẳng tới AudioManager có sẵn: "Hãy tắt nhạc nền Menu đi!"
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMusic();
        }

        // Chuyển sang Scene chạy video
        LoadGameScene();
    }

    void LoadGameScene()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    // ================= GIỮ NGUYÊN CÁC HÀM CŨ CỦA BẠN =================
    public void ShowConfirmQuit() { if (confirmQuitPanel != null) confirmQuitPanel.SetActive(true); }
    public void ConfirmQuit()
    {
        Debug.Log("Thoát game!");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    public void CancelQuit() { if (confirmQuitPanel != null) confirmQuitPanel.SetActive(false); }
}