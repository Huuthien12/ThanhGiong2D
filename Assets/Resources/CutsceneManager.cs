using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class CutsceneManager : MonoBehaviour
{
    private VideoPlayer videoPlayer;

    [Header("Tên Màn Ch?i Ti?p Theo")]
    public string nextSceneName = "Map1"; // Tên scene s? chuy?n t?i sau khi xem xong

    void Start()
    {
        // L?y thành ph?n VideoPlayer g?n trên cùng ??i t??ng
        videoPlayer = GetComponent<VideoPlayer>();

        if (videoPlayer != null)
        {
            // ??ng ký s? ki?n: Khi video ch?y h?t t? ??ng thì g?i hàm ChangeScene
            videoPlayer.loopPointReached += ChangeScene;
        }
    }

    void Update()
    {
        // Ki?m tra n?u ng??i ch?i b?m phím SPACE (D?u cách) thì m?i b? qua video
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangeScene(videoPlayer);
        }
    }

    // Hàm th?c hi?n chuy?n c?nh sang Map1
    void ChangeScene(VideoPlayer vp)
    {
        // H?y ??ng ký s? ki?n tr??c khi chuy?n scene ?? tránh l?i b? nh? xung ??t
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= ChangeScene;
        }

        SceneManager.LoadScene(nextSceneName);
    }
}