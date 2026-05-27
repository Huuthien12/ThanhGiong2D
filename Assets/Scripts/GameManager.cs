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

    [Header("=== NHIỆM VỤ ===")]
    public bool suDungNhiemVuCom = true;
    public bool suDungNhiemVuDietQuai = false;

    [Header("=== CẤU HÌNH MAP 1 ===")]
    public int soComCanThu = 10;
    private int soComDaThu = 0;

    [Header("=== CẤU HÌNH MAP 2/3 ===")]
    public int soQuaiCanDiet = 8;
    private int soQuaiDaDiet = 0;

    [Header("=== SQUARE CHẶN ĐƯỜNG ===")]
    public List<GameObject> blockingSquares = new List<GameObject>();
    private bool daMoKhoa = false;

    [Header("=== UI GIAO DIỆN ===")]
    public Text textNhiemVu;
    public GameObject thongBaoMoKhoa;

    [Header("=== MÀU SẮC ===")]
    public Color mauSquareKhiKhoa = new Color(0.8f, 0.2f, 0.2f, 1f);
    public Color mauSquareKhiMo = new Color(0.2f, 0.8f, 0.2f, 0.5f);

    // LƯU TRỮ DỮ LIỆU KHI CHUYỂN MAP
    private static int luuSoComDaThu = 0;
    private static int luuSoQuaiDaDiet = 0;
    private static bool luuDaMoKhoa = false;
    public bool daQuaMapTruoc = false;

    void Awake()
    {
        /*if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }*/
        Instance = this;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"📱 Load scene: {scene.name}");

        // Khôi phục dữ liệu từ biến tĩnh
        soComDaThu = luuSoComDaThu;
        soQuaiDaDiet = luuSoQuaiDaDiet;
        daMoKhoa = luuDaMoKhoa;

        Debug.Log($"📊 Khôi phục: Com={soComDaThu}/{soComCanThu}, Quai={soQuaiDaDiet}/{soQuaiCanDiet}, MoKhoa={daMoKhoa}");

        // Reset danh sách square (sẽ tìm lại trong scene mới)
        blockingSquares.Clear();

        KhoiTaoManChoiHienTai(scene.name);
    }

    void KhoiTaoManChoiHienTai(string currentSceneName)
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // 1. Phân chia nhiệm vụ theo map
        if (currentSceneName == "Map2_1")
        {
            suDungNhiemVuCom = true;
            suDungNhiemVuDietQuai = false;
        }
        else if (currentSceneName == "Map2_2" || currentSceneName == "Map2_3")
        {
            suDungNhiemVuCom = false;
            suDungNhiemVuDietQuai = true;
        }

        // 2. Tìm player
        playerControl = FindFirstObjectByType<PlayerControl>();
        playerHealth = FindFirstObjectByType<HealthManager>();

        // 3. Tìm UI
        if (textNhiemVu == null)
            textNhiemVu = GameObject.Find("TextNhiemVu")?.GetComponent<Text>();
        if (thongBaoMoKhoa == null)
            thongBaoMoKhoa = GameObject.Find("ThongBaoMoKhoa");
        if (gameOverPanel == null)
            gameOverPanel = GameObject.Find("GameOverPanel");

        // 4. Khởi tạo square
        KhoiTaoTatCaSquare();

        if (thongBaoMoKhoa != null)
            thongBaoMoKhoa.SetActive(false);

        CapNhatUI();

        // Nếu đã mở khóa từ trước, mở luôn square trong scene mới
        if (daMoKhoa)
        {
            MoKhoaTatCaSquare();
        }
    }

    void KhoiTaoTatCaSquare()
    {
        // Tìm tất cả square trong scene hiện tại
        if (blockingSquares == null || blockingSquares.Count == 0)
        {
            GameObject[] foundSquares = GameObject.FindGameObjectsWithTag("BlockingSquare");
            blockingSquares = new List<GameObject>(foundSquares);
            Debug.Log($"🔍 Tìm thấy {blockingSquares.Count} BlockingSquare trong scene");
        }

        for (int i = 0; i < blockingSquares.Count; i++)
        {
            if (blockingSquares[i] != null)
            {
                Collider2D col = blockingSquares[i].GetComponent<Collider2D>();
                if (col != null) col.isTrigger = false;

                SpriteRenderer sr = blockingSquares[i].GetComponent<SpriteRenderer>();
                if (sr != null) sr.color = mauSquareKhiKhoa;
            }
        }
    }

    public void ThuThapCom()
    {
        if (!suDungNhiemVuCom || daMoKhoa) return;

        soComDaThu++;
        Debug.Log($"🍙 Thu thập com: {soComDaThu}/{soComCanThu}");

        // LƯU VÀO BIẾN TĨNH
        luuSoComDaThu = soComDaThu;

        CapNhatUI();

        // Biến hình khi đủ 10 com
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
        Debug.Log($"💀 Tiêu diệt quái: {soQuaiDaDiet}/{soQuaiCanDiet}");

        // LƯU VÀO BIẾN TĨNH
        luuSoQuaiDaDiet = soQuaiDaDiet;

        CapNhatUI();

        if (soQuaiDaDiet >= soQuaiCanDiet)
        {
            MoKhoaTatCaSquare();
        }
    }

    private void MoKhoaTatCaSquare()
    {
        if (daMoKhoa) return;

        daMoKhoa = true;
        luuDaMoKhoa = true; // LƯU TRẠNG THÁI ĐÃ MỞ KHÓA

        Debug.Log("🎉 MỞ KHÓA TẤT CẢ SQUARE!");

        for (int i = 0; i < blockingSquares.Count; i++)
        {
            if (blockingSquares[i] != null)
            {
                blockingSquares[i].SetActive(false);
                Debug.Log($"  ✓ Đã tắt {blockingSquares[i].name}");
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
        luuSoComDaThu = 0;
        luuSoQuaiDaDiet = 0;
        luuDaMoKhoa = false;
        daMoKhoa = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;

        // Reset toàn bộ dữ liệu
        soComDaThu = 0;
        soQuaiDaDiet = 0;
        luuSoComDaThu = 0;
        luuSoQuaiDaDiet = 0;
        luuDaMoKhoa = false;
        daMoKhoa = false;
        daQuaMapTruoc = false;

        Destroy(gameObject);
        SceneManager.LoadScene("MenuScene");
    }

    public void ChuyenMap(string tenMapMoi)
    {
        daQuaMapTruoc = true; // Đánh dấu đã qua map
        SceneManager.LoadScene(tenMapMoi);
    }

    public void PlayerDied()
    {
        if (!gameOverPanel.activeSelf)
        {
            GameOver();
        }
    }
}