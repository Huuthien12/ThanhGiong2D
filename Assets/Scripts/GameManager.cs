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
    private bool daMoKhoa = false;  // Giữ private

    // Thêm property public
    public bool DaMoKhoa
    {
        get { return daMoKhoa; }
        set { daMoKhoa = value; }
    }

    [Header("=== UI GIAO DIỆN ===")]
    public Text textNhiemVu;
    public GameObject thongBaoMoKhoa;

    [Header("=== MÀU SẮC ===")]
    public Color mauSquareKhiKhoa = new Color(0.8f, 0.2f, 0.2f, 1f);
    public Color mauSquareKhiMo = new Color(0.2f, 0.8f, 0.2f, 0.5f);

    // Dữ liệu lưu trữ xuyên scene
    private static int luuSoComDaThu = 0;
    private static int luuSoQuaiDaDiet = 0;
    private static bool luuDaMoKhoa = false;
    public bool daQuaMapTruoc = false;

    // Flag để tránh biến hình nhiều lần
    private bool daBienHinhTuBaby = false;

    void Awake()
    {
        // ✅ Singleton Pattern CHUẨN
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Giữ object khi chuyển scene
            SceneManager.sceneLoaded += OnSceneLoaded;  // Đăng ký sự kiện load scene
            Debug.Log("🎮 GameManager khởi tạo (Singleton)");
        }
        else
        {
            Debug.Log("🗑️ Destroy GameManager dư thừa");
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        // Hủy đăng ký sự kiện khi object bị destroy
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"📱 Load scene: {scene.name}");
        Debug.Log($"📊 Trước khi khôi phục: soComDaThu={soComDaThu}, luuSoComDaThu={luuSoComDaThu}");

        // Khôi phục dữ liệu từ biến tĩnh
        soComDaThu = luuSoComDaThu;
        soQuaiDaDiet = luuSoQuaiDaDiet;
        daMoKhoa = luuDaMoKhoa;

        Debug.Log($"📊 Sau khi khôi phục: Com={soComDaThu}/{soComCanThu}, Quai={soQuaiDaDiet}/{soQuaiCanDiet}, MoKhoa={daMoKhoa}");

        blockingSquares.Clear();
        KhoiTaoManChoiHienTai(scene.name);
    }

    void KhoiTaoManChoiHienTai(string currentSceneName)
    {
        // Tìm UI trong scene mới
        if (gameOverPanel == null)
            gameOverPanel = GameObject.Find("GameOverPanel");
        if (textNhiemVu == null)
            textNhiemVu = GameObject.Find("TextNhiemVu")?.GetComponent<Text>();
        if (thongBaoMoKhoa == null)
            thongBaoMoKhoa = GameObject.Find("ThongBaoMoKhoa");

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
        if (PlayerControl.Instance != null)
        {
            playerControl = PlayerControl.Instance;
        }
        else
        {
            playerControl = FindFirstObjectByType<PlayerControl>();
        }
        playerHealth = FindFirstObjectByType<HealthManager>();

        // 3. Khởi tạo square (BẮT BUỘC phải gọi TRƯỚC khi mở khóa)
        KhoiTaoTatCaSquare();

        if (thongBaoMoKhoa != null)
            thongBaoMoKhoa.SetActive(false);

        CapNhatUI();

        // 4. Nếu đã mở khóa từ trước, mở luôn square trong scene mới
        if (daMoKhoa)
        {
            Debug.Log($"🔓 Đã mở khóa từ trước, đang mở {blockingSquares.Count} square...");
            MoKhoaTatCaSquare();  // Gọi lại để tắt square
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

        // Đảm bảo playerControl không bị null
        if (playerControl == null && PlayerControl.Instance != null)
        {
            playerControl = PlayerControl.Instance;
        }

        if (playerControl == null)
        {
            Debug.LogError("❌ Không tìm thấy PlayerControl!");
            return;
        }

        soComDaThu++;
        Debug.Log($"🍙 Thu thập com: {soComDaThu}/{soComCanThu}");

        luuSoComDaThu = soComDaThu;
        CapNhatUI();

        // Biến hình khi đủ 10 com
        if (soComDaThu >= 10 && !daBienHinhTuBaby)
        {
            if (playerControl.currentForm == PlayerControl.PlayerForm.Baby)
            {
                daBienHinhTuBaby = true;
                playerControl.ChangeForm(PlayerControl.PlayerForm.Adult);
                Debug.Log("🦄 Đã biến hình Baby → Adult!");
            }
        }

        // ✅ MỞ KHÓA CỬA khi đủ com
        if (soComDaThu >= soComCanThu)
        {
            Debug.Log($"🔑 Đủ {soComCanThu} com, chuẩn bị mở cửa!");
            MoKhoaTatCaSquare();
        }
        else
        {
            Debug.Log($"🚪 Cần thêm {soComCanThu - soComDaThu} com nữa để mở cửa");
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
        if (daMoKhoa && blockingSquares.Count > 0)
        {
            Debug.Log("⚠️ Cửa đã mở, bỏ qua!");
            return;
        }

        daMoKhoa = true;
        luuDaMoKhoa = true;

        Debug.Log($"🎉 MỞ KHÓA TẤT CẢ {blockingSquares.Count} SQUARE!");

        for (int i = 0; i < blockingSquares.Count; i++)
        {
            if (blockingSquares[i] != null)
            {
                // ✅ Gọi hàm MoKhoa() trên SquareBlocker nếu có
                SquareBlocker blocker = blockingSquares[i].GetComponent<SquareBlocker>();
                if (blocker != null)
                {
                    blocker.MoKhoa();
                }
                else
                {
                    // Fallback: tắt trực tiếp
                    blockingSquares[i].SetActive(false);
                }
                Debug.Log($"  ✓ Đã xử lý {blockingSquares[i].name}");
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
        if (gameOverPanel == null || gameOverPanel.activeSelf) return;

        Time.timeScale = 0f;
        gameOverPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RetryGame()
    {
        Time.timeScale = 1f;

        // Reset dữ liệu
        soComDaThu = 0;
        soQuaiDaDiet = 0;
        luuSoComDaThu = 0;
        luuSoQuaiDaDiet = 0;
        luuDaMoKhoa = false;
        daMoKhoa = false;
        daBienHinhTuBaby = false;
        daQuaMapTruoc = false;

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
        daBienHinhTuBaby = false;

        Destroy(gameObject);
        SceneManager.LoadScene("MenuScene");
    }

    public void ChuyenMap(string tenMapMoi)
    {
        daQuaMapTruoc = true;
        SceneManager.LoadScene(tenMapMoi);
    }

    public void PlayerDied()
    {
        if (gameOverPanel == null || !gameOverPanel.activeSelf)
        {
            GameOver();
        }
    }

    // Property để lấy số com hiện tại (cho UI)
    public int LaySoComHienTai() => soComDaThu;
}