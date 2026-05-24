using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class TeleportByLocation : MonoBehaviour
{
    [Header("=== TELEPORT SETTINGS ===")]
    public Transform targetTeleportPoint;  // Điểm đến nội bộ trong cùng map (kéo Transform vào đây)
    public GameObject player;               // Nhân vật (Sẽ tự động tìm tag "Player" nếu bỏ trống)
    public KeyCode interactKey = KeyCode.W; // Phím để kích hoạt dịch chuyển

    [Header("=== SCENE TELEPORT ===")]
    public bool teleportToDifferentScene = false;  // Tích vào nếu muốn chuyển sang map khác
    public string targetSceneName;                  // Tên scene đích xác (VD: "Map2")
    public string targetTeleportPointID = "FromMap1"; // ID của điểm đón ở map mới (VD: "FromMap1", "FromMap2")

    [Header("=== UI SETTINGS ===")]
    public string locationName = "Cổng Dịch Chuyển"; // Tên khu vực hiển thị lên giao diện
    public GameObject interactUI;           // Panel chữ hướng dẫn "Nhấn W để..."
    public Color textColor = Color.white;

    [Header("=== EFFECTS (tùy chọn) ===")]
    public float teleportDelay = 2f;      // Thời gian hiển thị màn hình chờ (Nên để tầm 2s cho mượt)
    public AudioClip teleportSound;         // Âm thanh hiệu ứng dịch chuyển

    private bool isNearPortal = false;
    private bool isTeleporting = false;
    private TextMeshProUGUI interactText;

    void Start()
    {
        // Tự động tìm kiếm Player gốc đi xuyên map nếu biến này bị trống
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        // Ẩn UI thông báo tương tác lúc mới vào màn chơi
        if (interactUI != null)
            interactUI.SetActive(false);

        // Khởi tạo nội dung văn bản hướng dẫn trên UI
        if (interactUI != null)
        {
            interactText = interactUI.GetComponentInChildren<TextMeshProUGUI>();
            CapNhatVanBanHuongDan();
        }

        Debug.Log($"✅ Đã khởi tạo portal: {locationName} (Nhấn {interactKey} để teleport)");
    }

    void Update()
    {
        // Kiểm tra điều kiện: Đứng cạnh cổng + Chưa trong trạng thái dịch chuyển + Nhấn nút tương tác
        if (isNearPortal && !isTeleporting && Input.GetKeyDown(interactKey))
        {
            KichHoatDichChuyen();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isNearPortal = true;

            // Cập nhật lại tham chiếu để bắt trúng ông Player gốc vừa chuyển map sang
            player = other.gameObject;

            if (interactUI != null)
            {
                CapNhatVanBanHuongDan();
                interactUI.SetActive(true);
            }
            Debug.Log($"🟢 Đến gần {locationName}, nhấn {interactKey} để tiến vào.");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isNearPortal = false;
            if (interactUI != null)
                interactUI.SetActive(false);
            Debug.Log($"🔴 Đã rời xa vùng tương tác của {locationName}");
        }
    }

    void CapNhatVanBanHuongDan()
    {
        if (interactText != null)
        {
            interactText.color = textColor;
            interactText.text = $"Nhấn [{interactKey}] để đến {locationName}";
        }
    }

    void KichHoatDichChuyen()
    {
        isTeleporting = true;

        // Ẩn ngay lập tức khung chữ "Nhấn W" để giao diện sạch sẽ
        if (interactUI != null)
            interactUI.SetActive(false);

        // Phát âm thanh dịch chuyển tại tọa độ cổng nếu có cài đặt
        if (teleportSound != null)
            AudioSource.PlayClipAtPoint(teleportSound, transform.position);

        // KIỂM TRA: Nếu chuyển map, gọi màn hình chờ thông minh xử lý xuyên scene
        if (teleportToDifferentScene)
        {
            if (LoadingPanelHandler.Instance != null)
            {
                // Truyền lệnh: Bật màn hình Thánh Gióng lên -> Chờ đếm giây -> Thực hiện hàm ThucHienChuyenScene
                LoadingPanelHandler.Instance.StartLoadingCoroutine(teleportDelay, () =>
                {
                    ThucHienChuyenScene();
                });
            }
            else
            {
                // Phương án dự phòng nếu chưa có LoadingPanelHandler trong scene, chuyển thẳng luôn
                ThucHienChuyenScene();
            }
        }
        else
        {
            // Nếu chỉ dịch chuyển nội bộ trong cùng một bản đồ, chạy Coroutine đếm giây ngắn tại chỗ
            StartCoroutine(TeleportNoiBoCoroutine());
        }
    }

    void ThucHienChuyenScene()
    {
        // Ghi lại ID điểm đến vào bộ nhớ để Map tiếp theo nạp lên biết lối xếp vị trí nhân vật
        PlayerPrefs.SetString("TargetTeleportID", targetTeleportPointID);
        PlayerPrefs.Save();

        // Tiến hành nạp map mới
        SceneManager.LoadScene(targetSceneName);
    }

    // Coroutine xử lý dịch chuyển nội bộ trong cùng Scene
    IEnumerator TeleportNoiBoCoroutine()
    {
        // Nếu bạn muốn dịch chuyển nội bộ cũng hiện ảnh Thánh Gióng, bật nó lên ở đây
        if (LoadingPanelHandler.Instance != null)
            LoadingPanelHandler.Instance.gameObject.SetActive(true);

        yield return new WaitForSeconds(teleportDelay);

        if (targetTeleportPoint != null && player != null)
        {
            player.transform.position = targetTeleportPoint.position;
            Debug.Log($"✨ Dịch chuyển nội bộ thành công đến {locationName} tại {targetTeleportPoint.position}");
        }
        else
        {
            Debug.LogError($"❌ Thiếu dữ liệu điểm đến hoặc Player tại cổng {locationName}!");
        }

        // Tắt ảnh Thánh Gióng sau khi dịch chuyển nội bộ xong
        if (LoadingPanelHandler.Instance != null)
            LoadingPanelHandler.Instance.gameObject.SetActive(false);

        isTeleporting = false;
    }

    // Vẽ đường liên kết trực quan trong giao diện Scene Editor của Unity
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Gizmos.DrawWireCube(transform.position, col.bounds.size);
        }

        if (targetTeleportPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, targetTeleportPoint.position);
            Gizmos.DrawWireSphere(targetTeleportPoint.position, 0.4f);
        }
    }
}