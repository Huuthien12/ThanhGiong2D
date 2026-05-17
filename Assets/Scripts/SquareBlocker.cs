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
        }
    }

    // Hàm được GameManager gọi khi mở khóa
    public void MoKhoa()
    {
        if (daMo) return;

        daMo = true;

        // Cho đi xuyên qua
        if (col != null)
        {
            col.isTrigger = true;
        }

        // Đổi màu
        if (spriteRenderer != null)
        {
            spriteRenderer.color = mauMo;
        }

        Debug.Log(gameObject.name + " đã mở khóa!");
    }
}