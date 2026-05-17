using UnityEngine;

public class SceneTeleportReceiver : MonoBehaviour
{
    [Header("=== TELEPORT POINTS ===")]
    public Transform startPoint;      // Điểm spawn mặc định
    public Transform fromMap1Point;   // Điểm đến từ Map1
    public Transform fromMap2Point;   // Điểm đến từ Map2
    public Transform fromMap3Point;   // Điểm đến từ Map3

    private GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        // Kiểm tra xem có teleport từ scene khác không
        if (PlayerPrefs.HasKey("TargetTeleportID"))
        {
            string targetID = PlayerPrefs.GetString("TargetTeleportID");
            TeleportToPoint(targetID);

            // Xóa key sau khi dùng
            PlayerPrefs.DeleteKey("TargetTeleportID");
        }
        else if (startPoint != null && player != null)
        {
            // Spawn ở điểm mặc định
            player.transform.position = startPoint.position;
        }
    }

    void TeleportToPoint(string pointID)
    {
        Transform targetPoint = null;

        switch (pointID)
        {
            case "FromMap1":
                targetPoint = fromMap1Point;
                break;
            case "FromMap2":
                targetPoint = fromMap2Point;
                break;
            case "FromMap3":
                targetPoint = fromMap3Point;
                break;
            default:
                targetPoint = startPoint;
                break;
        }

        if (targetPoint != null && player != null)
        {
            player.transform.position = targetPoint.position;
            Debug.Log($"✅ Teleport đến {targetPoint.name} tại {targetPoint.position}");
        }
    }
}