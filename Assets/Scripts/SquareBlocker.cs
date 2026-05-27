using UnityEngine;

public class SquareBlocker : MonoBehaviour
{
    [Header("Màu trạng thái")]
    public Color mauKhoa = new Color(0.8f, 0.2f, 0.2f, 1f);
    public Color mauMo = new Color(0.2f, 0.8f, 0.2f, 0.5f);

    private SpriteRenderer spriteRenderer;
    private Collider2D col;
    private bool daMo = false;

    void Start()
    {
        // ✅ Dùng trực tiếp daMoKhoa (đã là public)
        if (GameManager.Instance != null && GameManager.Instance.DaMoKhoa)
        {
            Debug.Log($"[SquareBlocker] {gameObject.name} đã được mở khóa từ trước, tự hủy!");
            gameObject.SetActive(false);
            return;
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        if (spriteRenderer != null)
        {
            spriteRenderer.color = mauKhoa;
        }

        if (col != null)
        {
            col.isTrigger = false;
            col.enabled = true;
        }

        Debug.Log($"[SquareBlocker] Khởi tạo {gameObject.name}");
    }

    public void MoKhoa()
    {
        if (daMo) return;

        Debug.Log($"[SquareBlocker] MoKhoa() trên {gameObject.name}");
        daMo = true;
        gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"[SquareBlocker] {gameObject.name}: Player chạm vào. daMo={daMo}");
        }
    }
}