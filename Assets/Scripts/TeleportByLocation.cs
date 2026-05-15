using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TeleportByLocation : MonoBehaviour
{
    [Header("=== TELEPORT SETTINGS ===")]
    public Transform targetTeleportPoint;  // Điểm đến (kéo TeleportPoint vào đây)
    public GameObject player;               // Nhân vật (kéo Player vào đây)

    [Header("=== UI SETTINGS ===")]
    public string locationName = "Map 1";   // Tên hiển thị
    public Color textColor = Color.white;

    [Header("=== EFFECTS (tùy chọn) ===")]
    public float teleportDelay = 0f;        // Delay trước khi teleport
    public bool useFadeEffect = false;

    private Button button;
    private TextMeshProUGUI buttonText;

    void Start()
    {
        // Lấy component Button
        button = GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError("❌ Không tìm thấy Button trên " + gameObject.name);
            return;
        }

        // Lấy Text bên trong Button
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            buttonText.text = locationName;
            buttonText.color = textColor;
        }

        // Gán sự kiện click
        button.onClick.AddListener(OnClickTeleport);

        Debug.Log($"✅ Đã khởi tạo portal: {locationName}");
    }

    void OnClickTeleport()
    {
        // Kiểm tra đã gán đủ dữ liệu chưa
        if (targetTeleportPoint == null)
        {
            Debug.LogError("❌ Chưa gán Target Teleport Point cho " + locationName);
            return;
        }

        if (player == null)
        {
            Debug.LogError("❌ Chưa gán Player cho " + locationName);
            return;
        }

        // Teleport ngay hoặc có delay
        if (teleportDelay > 0)
        {
            Invoke("DoTeleport", teleportDelay);
        }
        else
        {
            DoTeleport();
        }
    }

    void DoTeleport()
    {
        Vector3 newPosition = targetTeleportPoint.position;
        player.transform.position = newPosition;

        Debug.Log($"✨ Teleport thành công đến {locationName} tại vị trí {newPosition}");

        // (Tùy chọn) Phát âm thanh nếu có
        // AudioSource.PlayClipAtPoint(teleportSound, Camera.main.transform.position);
    }
}