using System.Collections;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.UI;
using TMPro;

public class TeleportWithCinemachine3 : MonoBehaviour
{
    [Header("=== TELEPORT SETTINGS ===")]
    public Transform targetTeleportPoint;
    public GameObject player;

    [Header("=== CAMERA SETTINGS ===")]
    public CinemachineCamera sourceCamera;
    public CinemachineCamera targetCamera;

    [Header("=== BOUNDS SETTINGS ===")]
    public Collider2D bounds_MapGoc;
    public Collider2D bounds_MapPhu1;
    public Collider2D bounds_MapPhu2;
    public Collider2D bounds_MapPhu3;

    [Header("=== MAP SETTINGS ===")]
    public int currentMapID = 1;
    public int targetMapID = 2;

    [Header("=== UI ===")]
    public string locationName = "Map";
    public GameObject loadingPanel;
    public float loadingTime = 3f;

    [Header("=== CANVAS/GLOBAL BUTTONS ===")]
    public GameObject canvasGlobal;      // Canvas chứa Btn_Back (luôn bật)
    public GameObject btnBack;           // Nút Back (nếu muốn điều khiển riêng)

    [Header("=== MAP BUTTONS (TRÊN MAP GỐC) ===")]
    public GameObject mapButtonsParent;  // Parent chứa các nút Btn_Map1, Btn_Map2, Btn_Map3
    // Hoặc gán từng nút:
    public GameObject btnMap1;
    public GameObject btnMap2;
    public GameObject btnMap3;

    private Button button;
    private GameObject buttonObject;
    private CinemachineConfiner2D confiner;
    private bool isTeleporting = false;

    void Start()
    {
        button = GetComponent<Button>();
        buttonObject = gameObject;

        if (button != null)
            button.onClick.AddListener(OnClickTeleport);

        TextMeshProUGUI text = GetComponentInChildren<TextMeshProUGUI>();
        if (text != null) text.text = locationName;

        if (currentMapID == targetMapID)
            buttonObject.SetActive(false);

        if (player == null)
        {
            Debug.LogError($"❌ Player chưa được gán cho {gameObject.name}!");
            return;
        }

        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            Debug.LogError("❌ Không tìm thấy Main Camera!");
            return;
        }

        confiner = mainCam.GetComponent<CinemachineConfiner2D>();
        if (confiner == null)
            confiner = mainCam.gameObject.AddComponent<CinemachineConfiner2D>();

        if (loadingPanel != null)
            loadingPanel.SetActive(false);

        // Khởi tạo UI theo map hiện tại
        UpdateUIByMap(currentMapID);

        Debug.Log($"✅ Khởi tạo {gameObject.name} thành công");
    }

    void OnClickTeleport()
    {
        if (isTeleporting) return;

        isTeleporting = true;

        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
            Debug.Log("🔵 Bật loading panel");
        }

        Invoke(nameof(ExecuteTeleport), loadingTime);
    }

    void ExecuteTeleport()
    {
        PerformTeleport();

        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
            Debug.Log("🟢 Tắt loading panel");
        }

        isTeleporting = false;
    }

    void PerformTeleport()
    {
        if (targetTeleportPoint == null)
        {
            Debug.LogError($"❌ Target Teleport Point chưa được gán cho {gameObject.name}!");
            return;
        }

        Vector3 oldPosition = player.transform.position;
        Vector3 newPosition = targetTeleportPoint.position;
        Vector3 delta = newPosition - oldPosition;

        Debug.Log($"📍 Teleport từ {oldPosition} đến {newPosition}");

        if (sourceCamera != null)
            sourceCamera.OnTargetObjectWarped(player.transform, delta);
        if (targetCamera != null)
            targetCamera.OnTargetObjectWarped(player.transform, delta);

        player.transform.position = newPosition;

        if (sourceCamera != null)
            sourceCamera.Priority = 0;
        if (targetCamera != null)
            targetCamera.Priority = 10;

        ChangeBounds(targetMapID);
        UpdateAllButtonsAfterTeleport(targetMapID);

        // 🔥 CẬP NHẬT UI SAU KHI TELEPORT 🔥
        UpdateUIByMap(targetMapID);

        Debug.Log($"✨ Teleported to {locationName}");
    }

    // 🔥 HÀM MỚI: Cập nhật UI theo map hiện tại
    void UpdateUIByMap(int mapID)
    {
        Debug.Log($"🎨 Cập nhật UI cho map {mapID}");

        // Xử lý Btn_Back (luôn hiển thị ở mọi map)
        if (btnBack != null)
            btnBack.SetActive(true);

        if (canvasGlobal != null)
            canvasGlobal.SetActive(true);

        // Xử lý các nút map (Btn_Map1, Btn_Map2, Btn_Map3)
        // Chỉ hiển thị khi đang ở MAP GỐC (mapID == 1)
        bool isInMapGoc = (mapID == 1);

        // Cách 1: Dùng parent
        if (mapButtonsParent != null)
            mapButtonsParent.SetActive(isInMapGoc);

        // Cách 2: Từng nút riêng
        if (btnMap1 != null)
            btnMap1.SetActive(isInMapGoc);

        if (btnMap2 != null)
            btnMap2.SetActive(isInMapGoc);

        if (btnMap3 != null)
            btnMap3.SetActive(isInMapGoc);

        // Log để kiểm tra
        if (isInMapGoc)
            Debug.Log("🏠 Đang ở MAP GỐC → Hiển thị tất cả nút map");
        else
            Debug.Log("🗺️ Đang ở MAP PHỤ → Ẩn các nút map, chỉ hiển thị Btn_Back");
    }

    void ChangeBounds(int mapID)
    {
        if (confiner == null) return;

        switch (mapID)
        {
            case 1:
                if (bounds_MapGoc != null) confiner.BoundingShape2D = bounds_MapGoc;
                break;
            case 2:
                if (bounds_MapPhu1 != null) confiner.BoundingShape2D = bounds_MapPhu1;
                break;
            case 3:
                if (bounds_MapPhu2 != null) confiner.BoundingShape2D = bounds_MapPhu2;
                break;
            case 4:
                if (bounds_MapPhu3 != null) confiner.BoundingShape2D = bounds_MapPhu3;
                break;
        }
    }

    void UpdateAllButtonsAfterTeleport(int newMapID)
    {
        TeleportWithCinemachine3[] allButtons = FindObjectsByType<TeleportWithCinemachine3>(FindObjectsSortMode.None);
        foreach (var btn in allButtons)
        {
            btn.currentMapID = newMapID;
            btn.UpdateButtonVisibility();
        }
    }

    void UpdateButtonVisibility()
    {
        buttonObject.SetActive(currentMapID != targetMapID);
    }
}