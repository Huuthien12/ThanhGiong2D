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
        daMoKhoa = true;

        for (int i = 0; i < blockingSquares.Count; i++)
        {
            if (blockingSquares[i] != null)
            {
                SquareBlocker blocker =
                    blockingSquares[i].GetComponent<SquareBlocker>();

                if (blocker != null)
                {
                    blocker.MoKhoa();
                }
            }
        }

        if (thongBaoMoKhoa != null)
        {
            thongBaoMoKhoa.SetActive(true);
            Invoke("AnThongBao", 2f);
        }

        Debug.Log("🎉 ĐÃ MỞ KHÓA TẤT CẢ!");
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