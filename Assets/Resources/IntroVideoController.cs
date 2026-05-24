using UnityEngine;
using UnityEngine.Video;

public class IntroVideoController : MonoBehaviour
{
    private VideoPlayer videoPlayer;

    [Header("Kéo các ??i t??ng ?i?u khi?n Gameplay vào ?ây (n?u c?n)")]
    public GameObject playerObject; // Nhân v?t chính c?a b?n
    public GameObject gameplayUI;   // UI máu, n?ng l??ng... lúc ch?i game

    void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();

        // T?m th?i ?óng b?ng game/?n nhân v?t lúc video ?ang ch?y ?? tránh vi?c nhân v?t b? quái ?ánh lúc ?ang xem video
        if (playerObject != null) playerObject.SetActive(false);
        if (gameplayUI != null) gameplayUI.SetActive(false);
    }

    void Start()
    {
        if (videoPlayer != null)
        {
            // ??ng ký s? ki?n: Video ch?y h?t thì g?i hàm EndVideo
            videoPlayer.loopPointReached += EndVideo;
        }
    }

    void Update()
    {
        // Ng??i ch?i b?m phím b?t k? ?? b? qua ?o?n c?t c?nh
        if (videoPlayer != null && videoPlayer.isPlaying)
        {
            if (Input.anyKeyDown)
            {
                EndVideo(videoPlayer);
            }
        }
    }

    // Hàm x? lý khi video k?t thúc ho?c b? b?m b? qua
    void EndVideo(VideoPlayer vp)
    {
        // B?t nhân v?t và UI ch?i game lên
        if (playerObject != null) playerObject.SetActive(true);
        if (gameplayUI != null) gameplayUI.SetActive(true);

        // H?y ho?c ?n ??i t??ng Video Player này ?i là xong
        gameObject.SetActive(false); 
    }
}