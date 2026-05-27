using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System.Collections;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager Instance;

    private VideoPlayer videoPlayer;
    private GameObject videoObject;

    [Header("=== VIDEO CLIPS ===")]
    public VideoClip introVideo;
    public VideoClip endingVideo;

    [Header("=== CÀI ĐẶT ===")]
    public bool autoPlayOnStart = true;
    public KeyCode skipKey = KeyCode.Mouse0;
    public bool allowSkip = true;

    [Header("=== SCENE NAMES ===")]
    public string introNextScene = "Map1";        // Scene sau intro
    public string endingNextScene = "MenuScene";  // Scene sau ending

    private System.Action onCutsceneEnd;
    private bool isPlaying = false;
    private bool isQuitting = false;
    private bool skipRequested = false;
    private string currentCutsceneType = ""; // "intro" hoặc "ending"

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("✅ CutsceneManager khởi tạo");
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        SetupVideoPlayer();
    }

    void SetupVideoPlayer()
    {
        videoObject = new GameObject("VideoPlayer");
        videoObject.transform.SetParent(transform);
        videoPlayer = videoObject.AddComponent<VideoPlayer>();

        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = false;
        videoPlayer.skipOnDrop = true;
        videoPlayer.renderMode = VideoRenderMode.CameraFarPlane;

        if (Camera.main != null)
        {
            videoPlayer.targetCamera = Camera.main;
        }

        videoObject.SetActive(false);
        Debug.Log("✅ VideoPlayer đã được tạo");
    }

    void Start()
    {
        if (autoPlayOnStart && introVideo != null)
        {
            PlayIntroCutscene();
        }
    }

    void Update()
    {
        if (isPlaying && allowSkip && !skipRequested)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("⏭️ Skip cutscene!");
                skipRequested = true;
                SkipCutscene();
            }
        }
    }

    public void PlayIntroCutscene()
    {
        Debug.Log("🎬 PlayIntroCutscene - Sẽ chuyển sang: " + introNextScene);

        if (introVideo != null)
        {
            currentCutsceneType = "intro";
            PlayCutscene(introVideo, () => {
                Debug.Log("🎬 Intro kết thúc! Chuyển sang " + introNextScene);
                SceneManager.LoadScene(introNextScene);
            });
        }
        else
        {
            Debug.LogWarning("⚠️ Chưa gán Intro Video! Chuyển thẳng vào game.");
            SceneManager.LoadScene(introNextScene);
        }
    }

    public void PlayEndingCutscene(System.Action onComplete = null)
    {
        Debug.Log("🎬 PlayEndingCutscene - Sẽ chuyển sang: " + endingNextScene);

        if (endingVideo != null)
        {
            currentCutsceneType = "ending";
            PlayCutscene(endingVideo, () => {
                Debug.Log("🎬 Ending kết thúc! Chuyển sang " + endingNextScene);

                // Gọi callback nếu có
                if (onComplete != null)
                {
                    onComplete.Invoke();
                }
                else
                {
                    // Mặc định chuyển scene
                    Time.timeScale = 1f;
                    SceneManager.LoadScene(endingNextScene);
                }
            });
        }
        else
        {
            Debug.LogError("❌ Chưa gán Ending Video!");
            if (onComplete != null) onComplete.Invoke();
            else SceneManager.LoadScene(endingNextScene);
        }
    }

    public void PlayCutscene(VideoClip clip, System.Action onEnd = null)
    {
        if (isQuitting) return;

        if (clip == null)
        {
            Debug.LogError("❌ Video clip is null!");
            if (onEnd != null) onEnd.Invoke();
            return;
        }

        if (isPlaying)
        {
            Debug.Log("⚠️ Đang phát cutscene khác!");
            if (onEnd != null) onEnd.Invoke();
            return;
        }

        isPlaying = true;
        skipRequested = false;
        onCutsceneEnd = onEnd;

        // Dừng game
        Time.timeScale = 0f;

        // Cấu hình và phát
        videoPlayer.clip = clip;
        videoPlayer.loopPointReached -= OnVideoEnd;
        videoPlayer.loopPointReached += OnVideoEnd;

        videoObject.SetActive(true);
        videoPlayer.Play();

        Debug.Log($"▶️ Đang phát video: {clip.name}");
        Debug.Log("🖱️ Click chuột trái hoặc nhấn Space để bỏ qua");
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        Debug.Log("🏁 Video kết thúc!");
        if (!isQuitting) EndCutscene();
    }

    void SkipCutscene()
    {
        Debug.Log("⏭️ SkipCutscene");

        if (videoPlayer != null && videoPlayer.isPlaying)
        {
            videoPlayer.Stop();
        }

        EndCutscene();
    }

    void EndCutscene()
    {
        if (!isPlaying) return;

        Debug.Log("🔚 EndCutscene");

        // Ẩn video
        if (videoObject != null)
        {
            videoObject.SetActive(false);
        }

        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoEnd;
        }

        // Phục hồi game
        Time.timeScale = 1f;

        isPlaying = false;
        skipRequested = false;

        // Gọi callback
        if (onCutsceneEnd != null)
        {
            var callback = onCutsceneEnd;
            onCutsceneEnd = null;
            callback.Invoke();
        }
    }

    public bool IsPlaying()
    {
        return isPlaying;
    }

    void OnApplicationQuit()
    {
        isQuitting = true;
    }

    void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoEnd;
        }
    }
}