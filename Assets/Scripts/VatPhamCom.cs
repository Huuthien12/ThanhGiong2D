using UnityEngine;

public class VatPhamCom : MonoBehaviour
{
    [Header("=== HEAL SETTINGS ===")]
    [Tooltip("Phần trăm lượng máu hồi lại dựa trên MaxHealth của Player (0.1 = 10%)")]
    public float phanTramHoi = 0.1f;

    [Header("=== VISUAL EFFECTS ===")]
    public GameObject floatingTextPrefab;
    public Vector3 offset = new Vector3(0, 0.5f, 0);

    [Header("=== AUDIO EFFECTS ===")]
    [Tooltip("Tên chính xác của Sound Effect ăn cơm trong AudioManager")]
    public string eatSoundName = "EatRice";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra nếu đối tượng chạm vào hạt cơm có tag là Player
        if (collision.CompareTag("Player"))
        {
            HealthManager healthManager = collision.GetComponent<HealthManager>();

            if (healthManager != null)
            {
                // 1. XỬ LÝ HỒI MÁU CHO NHÂN VẬT
                float luongHoi = healthManager.maxHealth * phanTramHoi;
                healthManager.Heal(luongHoi);

                // 2. HIỂN THỊ CHỮ NỔI (+MÁU) TRÊN ĐẦU PLAYER
                ShowFloatingText(collision.transform.position, luongHoi);

                // 3. PHÁT ÂM THANH ĂN CƠM
                PlayEatSound();

                // 4. CẬP NHẬT NHIỆM VỤ SANG GAMEMANAGER (Giữ nguyên logic cũ của bạn)
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.ThuThapCom();
                }
                else
                {
#if UNITY_2023_1_OR_NEWER
                    GameManager gameManager = FindFirstObjectByType<GameManager>();
#else
                    GameManager gameManager = FindObjectOfType<GameManager>();
#endif
                    if (gameManager != null) gameManager.ThuThapCom();
                }

                // ========================================================
                // ĐÃ SỬA: GỌI ĐIỆN BÁO ĐIỂM SANG PLAYERCONTROL ĐỂ ĐẾM CƠM VÀ MỞ KHÓA
                // ========================================================
                if (PlayerControl.Instance != null)
                {
                    PlayerControl.Instance.AddRice(); // Báo điểm cho Singleton
                }
                else
                {
                    PlayerControl pc = collision.GetComponent<PlayerControl>();
                    if (pc != null) pc.AddRice(); // Phương án dự phòng nếu Singleton chưa sẵn sàng
                }
                // ========================================================

                // 5. BIẾN MẤT HẠT CƠM KHỎI BẢN ĐỒ
                Destroy(gameObject);
            }
        }
    }

    void ShowFloatingText(Vector3 playerPosition, float healAmount)
    {
        if (floatingTextPrefab == null) return; // Đã sửa điều kiện chống lỗi không xuất hiện chữ

        Vector3 spawnPos = playerPosition + offset;
        GameObject floatingText = Instantiate(floatingTextPrefab, spawnPos, Quaternion.identity);

        Floating3DText ft = floatingText.GetComponent<Floating3DText>();
        if (ft != null)
        {
            ft.SetText("+" + Mathf.RoundToInt(healAmount));
        }

        Destroy(floatingText, 1.5f);
    }

    void PlayEatSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayEatSound();
            // Nếu AudioManager dùng chuỗi string, bỏ comment dòng dưới:
            // AudioManager.Instance.PlaySFX(eatSoundName);
        }
        else
        {
            Debug.LogWarning("⚠️ [VatPhamCom] Không tìm thấy AudioManager trong scene!");
        }
    }
}