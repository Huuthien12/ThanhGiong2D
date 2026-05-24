using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("=== PANELS ===")]
    public GameObject gameOverPanel;

    [Header("=== PLAYER ===")]
    public HealthManager playerHealth;
    private PlayerControl playerControl;

    [HideInInspector] public bool suDungNhiemVuCom = true;
    [HideInInspector] public bool suDungNhiemVuDietQuai = false;

    [Header("=== CẤU HÌNH MAP 1 ===")]
    public int soComCanThu = 10;
    private int soComDaThu = 0;

    [Header("=== CẤU HÌNH MAP 2/3 ===")]
    public int soQuaiCanDiet = 5;
    private int soQuaiDaDiet = 0;

    [Header("=== SQUARE CHẶN ĐƯỜNG ===")]
    public List<GameObject> blockingSquares = new List<GameObject>();
    public List<Collider2D> squareColliders = new List<Collider2D>();
    private bool daMoKhoa = false;

    [Header("=== UI GIAO DIỆN ===")]
    public Text textNhiemVu;
    public GameObject thongBaoMoKhoa;

    [Header("=== HIỆU ỨNG CỬA ===")]
    public Color mauSquareKhiKhoa = new Color(0.8f, 0.2f, 0.2f, 1f);
    public Color mauSquareKhiMo = new Color(0.2f, 0.8f, 0.2f, 0.5f);

    private List<SpriteRenderer> squareSpriteRenderers = new List<SpriteRenderer>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        KhoiTaoManChoiHienTai(SceneManager.GetActiveScene().name);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        daMoKhoa = false;
        squareSpriteRenderers.Clear();
        squareColliders.Clear();
        blockingSquares.Clear();

        KhoiTaoManChoiHienTai(scene.name);
    }

    void KhoiTaoManChoiHienTai(string currentSceneName)
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // 1. Phân chia nhiệm vụ theo map
        if (currentSceneName == "Map1")
        {
            suDungNhiemVuCom = true;
            suDungNhiemVuDietQuai = false;
        }
        else if (currentSceneName == "Map2" || currentSceneName == "Map3")
        {
            suDungNhiemVuCom = false;
            suDungNhiemVuDietQuai = true;
        }

        // 2. Tìm kiếm nhân vật (Quét lại để bắt trúng ông Player đi xuyên map qua)
#if UNITY_2023_1_OR_NEWER
        playerHealth = FindFirstObjectByType<HealthManager>();
        playerControl = FindFirstObjectByType<PlayerControl>();
#else
        playerHealth = FindObjectOfType<HealthManager>();
        playerControl = FindObjectOfType<PlayerControl>();
#endif

        // Tìm UI hiển thị
        if (textNhiemVu == null) textNhiemVu = GameObject.Find("TextNhiemVu")?.GetComponent<Text>();
        if (thongBaoMoKhoa == null) thongBaoMoKhoa = GameObject.Find("ThongBaoMoKhoa");
        if (gameOverPanel == null) gameOverPanel = GameObject.Find("GameOverPanel");

        // 3. TỰ ĐỘNG XỬ LÝ DỊCH CHUYỂN VỊ TRÍ CHO PLAYER GỐC
        XulyDichChuyenPlayer();

        // 4. Ép buộc tắt Màn hình chờ nếu nó còn kẹt cứng trên màn hình
        Invoke("TatManHinhChoCuongBuc", 0.2f);

        KhoiTaoTatCaSquare();

        if (thongBaoMoKhoa != null)
            thongBaoMoKhoa.SetActive(false);

        CapNhatUI();
    }

    void XulyDichChuyenPlayer()
    {
        if (playerControl == null) return;

        // BẢO ĐẢM: Bật lại Object nhân vật để chắc chắn không bị vô hình
        playerControl.gameObject.SetActive(true);

        // Tìm kiếm script nhận điểm dịch chuyển trên map mới
        SceneTeleportReceiver receiver = null;
#if UNITY_2023_1_OR_NEWER
        receiver = FindFirstObjectByType<SceneTeleportReceiver>();
#else
        receiver = FindObjectOfType<SceneTeleportReceiver>();
#endif

        if (receiver != null)
        {
            receiver.XulyDichChuyenPlayer(playerControl.gameObject);
        }
        else
        {
            GameObject spawnPoint = GameObject.Find("SpawnPoint");
            if (spawnPoint != null)
            {
                playerControl.transform.position = spawnPoint.transform.position;
            }
        }
    }

    void TatManHinhChoCuongBuc()
    {
        // Gọi thẳng Singleton của LoadingPanelHandler để bắt nó tự đóng lại
        if (LoadingPanelHandler.Instance != null)
        {
            LoadingPanelHandler.Instance.ForceClose();
            Debug.Log("🎯 [GameManager] Đã dập tắt màn hình chờ thành công!");
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void KhoiTaoTatCaSquare()
    {
        if (blockingSquares == null || blockingSquares.Count == 0)
        {
            blockingSquares = new List<GameObject>(GameObject.FindGameObjectsWithTag("BlockingSquare"));
        }

        for (int i = 0; i < blockingSquares.Count; i++)
        {
            if (blockingSquares[i] == null) continue;

            Collider2D col = blockingSquares[i].GetComponent<Collider2D>();
            if (col != null) col.isTrigger = false;

            squareColliders.Add(col);

            SpriteRenderer sr = blockingSquares[i].GetComponent<SpriteRenderer>();
            squareSpriteRenderers.Add(sr);

            if (sr != null) sr.color = mauSquareKhiKhoa;
        }
    }

    void Update()
    {
        if (playerHealth != null && !playerHealth.IsAlive())
        {
            if (gameOverPanel != null && !gameOverPanel.activeSelf)
            {
                GameOver();
            }
        }
    }

    public void ThuThapCom()
    {
        if (!suDungNhiemVuCom || daMoKhoa) return;

        soComDaThu++;
        CapNhatUI();

        if (soComDaThu >= 10)
        {
            if (playerControl != null && playerControl.currentForm == PlayerControl.PlayerForm.Baby)
            {
                playerControl.ChangeForm(PlayerControl.PlayerForm.Adult);
            }
        }

        if (soComDaThu >= soComCanThu)
        {
            MoKhoaTatCaSquare();
        }
    }

    public void QuaiBiTieuDiet()
    {
        if (!suDungNhiemVuDietQuai || daMoKhoa) return;

        soQuaiDaDiet++;
        CapNhatUI();

        if (soQuaiDaDiet >= soQuaiCanDiet)
        {
            MoKhoaTatCaSquare();
        }
    }

    private void MoKhoaTatCaSquare()
    {
        daMoKhoa = true;

        for (int i = 0; i < blockingSquares.Count; i++)
        {
            if (blockingSquares[i] != null)
            {
                SpriteRenderer sr = blockingSquares[i].GetComponent<SpriteRenderer>();
                if (sr != null) sr.color = mauSquareKhiMo;

                Collider2D col = blockingSquares[i].GetComponent<Collider2D>();
                if (col != null) col.isTrigger = true;

                SquareBlocker blocker = blockingSquares[i].GetComponent<SquareBlocker>();
                if (blocker != null) blocker.MoKhoa();
            }
        }

        if (thongBaoMoKhoa != null)
        {
            thongBaoMoKhoa.SetActive(true);
            Invoke("AnThongBao", 2f);
        }
    }

    void AnThongBao()
    {
        if (thongBaoMoKhoa != null) thongBaoMoKhoa.SetActive(false);
    }

    void CapNhatUI()
    {
        if (textNhiemVu == null) return;

        if (suDungNhiemVuCom)
        {
            textNhiemVu.text = "🍙 Com: " + soComDaThu + " / " + soComCanThu;
        }
        else if (suDungNhiemVuDietQuai)
        {
            textNhiemVu.text = "💀 Quái: " + soQuaiDaDiet + " / " + soQuaiCanDiet;
        }
    }

    public void GameOver()
    {
        if (gameOverPanel.activeSelf) return;

        Time.timeScale = 0f;
        gameOverPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RetryGame()
    {
        Time.timeScale = 1f;
        soComDaThu = 0;
        soQuaiDaDiet = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        Destroy(gameObject);
        SceneManager.LoadScene("MenuScene");
    }

    public void ChuyenMap(string tenMapMoi)
    {
        SceneManager.LoadScene(tenMapMoi);
    }
}