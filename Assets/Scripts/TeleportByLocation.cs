using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class TeleportByLocation : MonoBehaviour
{
    [Header("=== TELEPORT SETTINGS ===")]
    public Transform targetTeleportPoint;  // Điểm đến (kéo TeleportPoint vào đây)
    public GameObject player;               // Nhân vật (kéo Player vào đây)
    public KeyCode interactKey = KeyCode.W; // Phím để teleport

    [Header("=== SCENE TELEPORT ===")]
    public bool teleportToDifferentScene = false;  // Có teleport sang scene khác không?
    public string targetSceneName;                  // Tên scene đích (VD: "Map2")
    public string targetTeleportPointID = "StartPoint"; // ID của điểm đến ở scene mới

    [Header("=== UI SETTINGS ===")]
    public string locationName = "Map 1";   // Tên hiển thị
    public GameObject interactUI;           // UI thông báo (Panel chứa Text)
    public Color textColor = Color.white;

    [Header("=== EFFECTS (tùy chọn) ===")]
    public float teleportDelay = 0.5f;      // Delay trước khi teleport
    public AudioClip teleportSound;         // Âm thanh khi teleport
    public GameObject loadingPanel;         // Panel loading

    private bool isNearPortal = false;
    private bool isTeleporting = false;
    private TextMeshProUGUI interactText;

    void Start()
    {
        // Tìm player nếu chưa gán
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        // Tắt UI tương tác khi chưa đến gần
        if (interactUI != null)
            interactUI.SetActive(false);

        // Lấy text từ UI
        if (interactUI != null)
        {
            interactText = interactUI.GetComponentInChildren<TextMeshProUGUI>();
            if (interactText != null)
                interactText.text = $"Nhấn {interactKey} để đến {locationName}";
        }

        if (loadingPanel != null)
            loadingPanel.SetActive(false);

        Debug.Log($"✅ Đã khởi tạo portal: {locationName} (Nhấn {interactKey} để teleport)");
    }

    void Update()
    {
        // Kiểm tra nếu đang ở gần portal và nhấn phím W
        if (isNearPortal && !isTeleporting && Input.GetKeyDown(interactKey))
        {
            OnTeleport();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isNearPortal = true;
            if (interactUI != null)
            {
                interactUI.SetActive(true);
                if (interactText != null)
                    interactText.text = $"Nhấn {interactKey} để đến {locationName}";
            }
            Debug.Log($"🟢 Đến gần {locationName}, nhấn {interactKey} để teleport");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isNearPortal = false;
            if (interactUI != null)
                interactUI.SetActive(false);
            Debug.Log($"🔴 Rời khỏi {locationName}");
        }
    }

    void OnTeleport()
    {
        StartCoroutine(TeleportCoroutine());
    }

    IEnumerator TeleportCoroutine()
    {
        isTeleporting = true;

        // Tắt UI
        if (interactUI != null)
            interactUI.SetActive(false);

        // Bật loading panel
        if (loadingPanel != null)
            loadingPanel.SetActive(true);

        // Phát âm thanh
        if (teleportSound != null)
            AudioSource.PlayClipAtPoint(teleportSound, transform.position);

        // Chờ delay
        yield return new WaitForSeconds(teleportDelay);

        // Teleport
        if (teleportToDifferentScene)
        {
            // Lưu ID điểm đến để scene mới biết đặt player ở đâu
            PlayerPrefs.SetString("TargetTeleportID", targetTeleportPointID);
            PlayerPrefs.Save();

            // Chuyển scene
            SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            // Teleport trong cùng scene
            if (targetTeleportPoint == null)
            {
                Debug.LogError("❌ Chưa gán Target Teleport Point cho " + locationName);
                isTeleporting = false;
                yield break;
            }

            if (player == null)
            {
                Debug.LogError("❌ Chưa gán Player cho " + locationName);
                isTeleporting = false;
                yield break;
            }

            Vector3 newPosition = targetTeleportPoint.position;
            player.transform.position = newPosition;
            Debug.Log($"✨ Teleport thành công đến {locationName} tại vị trí {newPosition}");
        }

        // Tắt loading panel
        if (loadingPanel != null)
            loadingPanel.SetActive(false);

        isTeleporting = false;
    }

    // Vẽ vùng teleport trong Scene view
    void OnDrawGizmos()
    {
        // Vùng trigger
        Gizmos.color = Color.cyan;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Gizmos.DrawWireCube(transform.position, col.bounds.size);
        }

        // Đường đến điểm đích
        if (targetTeleportPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, targetTeleportPoint.position);
            Gizmos.DrawWireSphere(targetTeleportPoint.position, 0.5f);
        }
    }
}