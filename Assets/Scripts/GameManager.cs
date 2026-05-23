using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject gameOverPanel;

    [Header("Player")]
    public HealthManager playerHealth;

    // =====================================================
    // MAP 1 - ĂN COM
    // =====================================================

    [Header("MAP 1 - Thu thập Com")]
    public bool suDungNhiemVuCom = true;

    public int soComCanThu = 10;
    private int soComDaThu = 0;

    // =====================================================
    // MAP 2 / MAP 3 - DIỆT QUÁI
    // =====================================================

    [Header("MAP 2/3 - Tiêu diệt quái")]
    public bool suDungNhiemVuDietQuai = false;

    public int soQuaiCanDiet = 5;
    private int soQuaiDaDiet = 0;

    // =====================================================
    // CỬA / SQUARE
    // =====================================================

    [Header("Square chặn đường")]
    public List<GameObject> blockingSquares;
    public List<Collider2D> squareColliders;

    private bool daMoKhoa = false;

    [Header("UI")]
    public Text textNhiemVu;
    public GameObject thongBaoMoKhoa;

    [Header("Hiệu ứng")]
    public Color mauSquareKhiKhoa = new Color(0.8f, 0.2f, 0.2f, 1f);
    public Color mauSquareKhiMo = new Color(0.2f, 0.8f, 0.2f, 0.5f);

    private List<SpriteRenderer> squareSpriteRenderers = new List<SpriteRenderer>();

    void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (playerHealth == null)
        {
            playerHealth = FindFirstObjectByType<HealthManager>();
        }

        KhoiTaoTatCaSquare();

        if (thongBaoMoKhoa != null)
            thongBaoMoKhoa.SetActive(false);

        CapNhatUI();
    }

    // =====================================================
    // KHỞI TẠO SQUARE
    // =====================================================

    void KhoiTaoTatCaSquare()
    {
        for (int i = 0; i < blockingSquares.Count; i++)
        {
            if (blockingSquares[i] == null) continue;

            Collider2D col = blockingSquares[i].GetComponent<Collider2D>();

            if (col != null)
            {
                col.isTrigger = false;
            }

            squareColliders.Add(col);

            SpriteRenderer sr = blockingSquares[i].GetComponent<SpriteRenderer>();

            squareSpriteRenderers.Add(sr);

            if (sr != null)
            {
                sr.color = mauSquareKhiKhoa;
            }
        }
    }

    // =====================================================
    // UPDATE
    // =====================================================

    void Update()
    {
        if (playerHealth != null && !playerHealth.IsAlive())
        {
            if (!gameOverPanel.activeSelf)
            {
                GameOver();
            }
        }
    }

    // =====================================================
    // MAP 1 - ĂN COM
    // =====================================================

    public void ThuThapCom()
    {
        if (!suDungNhiemVuCom) return;

        if (daMoKhoa) return;

        soComDaThu++;

        Debug.Log("🍙 Đã ăn com: " + soComDaThu);

        CapNhatUI();

        if (soComDaThu >= soComCanThu)
        {
            MoKhoaTatCaSquare();
        }
    }

    // =====================================================
    // MAP 2/3 - DIỆT QUÁI
    // =====================================================

    public void QuaiBiTieuDiet()
    {
        if (!suDungNhiemVuDietQuai) return;

        if (daMoKhoa) return;

        soQuaiDaDiet++;

        Debug.Log("💀 Đã diệt quái: " + soQuaiDaDiet);

        CapNhatUI();

        if (soQuaiDaDiet >= soQuaiCanDiet)
        {
            MoKhoaTatCaSquare();
        }
    }

    // =====================================================
    // MỞ KHÓA
    // =====================================================

    private void MoKhoaTatCaSquare()
    {
        Debug.Log("=== BẮT ĐẦU MỞ KHÓA SQUARE ===");
        Debug.Log($"daMoKhoa trước khi mở: {daMoKhoa}");

        daMoKhoa = true;

        Debug.Log($"Số lượng blockingSquares trong danh sách: {blockingSquares.Count}");

        for (int i = 0; i < blockingSquares.Count; i++)
        {
            GameObject square = blockingSquares[i];

            if (square == null)
            {
                Debug.LogError($"Square {i} là NULL! Hãy kiểm tra lại trong Inspector");
                continue;
            }

            Debug.Log($"\n--- Đang xử lý Square {i}: {square.name} ---");
            Debug.Log($"Square có đang active không? {square.activeSelf}");
            Debug.Log($"Vị trí Square: {square.transform.position}");

            // Kiểm tra SquareBlocker
            SquareBlocker blocker = square.GetComponent<SquareBlocker>();
            if (blocker != null)
            {
                Debug.Log($"Tìm thấy SquareBlocker trên {square.name}");
                blocker.MoKhoa();
            }
            else
            {
                Debug.LogWarning($"KHÔNG tìm thấy SquareBlocker trên {square.name}, tự xử lý...");

                // Cách 1: TẮT HẲN GAMEOBJECT - chắc chắn nhất
                square.SetActive(false);
                Debug.Log($"Đã tắt GameObject {square.name}");

                // Cách 2: Tắt Collider
                Collider2D col = square.GetComponent<Collider2D>();
                if (col != null)
                {
                    col.enabled = false;
                    Debug.Log($"Đã tắt Collider của {square.name}");
                }

                // Đổi màu
                SpriteRenderer sr = square.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.color = mauSquareKhiMo;
                }
            }
        }

        // Hiện thông báo
        if (thongBaoMoKhoa != null)
        {
            thongBaoMoKhoa.SetActive(true);
            Invoke("AnThongBao", 10f);
        }

        Debug.Log("=== KẾT THÚC MỞ KHÓA ===");
    }
    void AnThongBao()
    {
        if (thongBaoMoKhoa != null)
        {
            thongBaoMoKhoa.SetActive(false);
        }
    }

    // =====================================================
    // UI
    // =====================================================

    void CapNhatUI()
    {
        if (textNhiemVu == null) return;

        if (suDungNhiemVuCom)
        {
            textNhiemVu.text =
                "🍙 Com: " + soComDaThu + " / " + soComCanThu;
        }

        if (suDungNhiemVuDietQuai)
        {
            textNhiemVu.text =
                "💀 Quái: " + soQuaiDaDiet + " / " + soQuaiCanDiet;
        }
    }

    // =====================================================
    // GAME OVER
    // =====================================================

    public void GameOver()
    {
        if (gameOverPanel.activeSelf) return;

        Time.timeScale = 0f;

        gameOverPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void PlayerDied()
    {
        if (!gameOverPanel.activeSelf)
        {
            GameOver();
        }
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