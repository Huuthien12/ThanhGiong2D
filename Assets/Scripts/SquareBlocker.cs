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
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        // Trạng thái khóa ban đầu
        if (spriteRenderer != null)
        {
            spriteRenderer.color = mauKhoa;
        }

        if (col != null)
        {
            col.isTrigger = false;
            col.enabled = true;
        }

        Debug.Log($"[SquareBlocker] Khởi tạo {gameObject.name}: Collider={col != null}, Sprite={spriteRenderer != null}");
    }

    // Hàm được GameManager gọi khi mở khóa
    public void MoKhoa()
    {
        Debug.Log($"[SquareBlocker] MoKhoa() được gọi trên {gameObject.name}");

        if (daMo)
        {
            Debug.Log($"[SquareBlocker] {gameObject.name} đã mở rồi!");
            return;
        }

        daMo = true;

        // TẮT HẲN GAMEOBJECT - cách chắc chắn nhất
        gameObject.SetActive(false);
        Debug.Log($"[SquareBlocker] Đã tắt {gameObject.name}");

        // Hoặc chỉ tắt collider
        /*
        if (col != null)
        {
            col.enabled = false;
            Debug.Log($"[SquareBlocker] Đã tắt Collider của {gameObject.name}");
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.color = mauMo;
        }
        */
    }

    // Debug: Kiểm tra trạng thái hiện tại
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"[SquareBlocker] {gameObject.name}: Player chạm vào. daMo={daMo}, isTrigger={col.isTrigger}, enabled={col.enabled}");
        }
    }
}