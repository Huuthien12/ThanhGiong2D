using UnityEngine;

public class SceneTeleportReceiver : MonoBehaviour
{
    [Header("=== TELEPORT POINTS ===")]
    public Transform startPoint;      // Điểm spawn mặc định nếu chơi trực tiếp từ map này
    public Transform fromMap1Point;   // Điểm đón khi đi từ Map1 sang
    public Transform fromMap2Point;   // Điểm đón khi đi từ Map2 sang
    public Transform fromMap3Point;   // Điểm đón khi đi từ Map3 sang

    // Hàm này sẽ được GameManager chủ động gọi ngay khi map mới tải xong xuôi
    public void XulyDichChuyenPlayer(GameObject player)
    {
        if (player == null) return;

        Transform targetPoint = null;

        // Kiểm tra xem có dữ liệu cổng dịch chuyển được lưu từ map trước không
        if (PlayerPrefs.HasKey("TargetTeleportID"))
        {
            string targetID = PlayerPrefs.GetString("TargetTeleportID");

            switch (targetID)
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

            // Xóa key ngay sau khi dùng xong để tránh map sau bị nhảy nhầm vị trí
            PlayerPrefs.DeleteKey("TargetTeleportID");
        }

        // Nếu không tìm thấy ID cổng phù hợp, mặc định lấy điểm Start Point
        if (targetPoint == null)
        {
            targetPoint = startPoint;
        }

        // Thực hiện dịch chuyển ông Player gốc sang vị trí mới
        if (targetPoint != null)
        {
            player.transform.position = targetPoint.position;
            Debug.Log($"✅ [Receiver] Đã dịch chuyển Player thành công đến điểm: {targetPoint.name}");
        }
    }
}