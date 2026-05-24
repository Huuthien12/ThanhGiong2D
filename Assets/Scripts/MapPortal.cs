using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MapPortal : MonoBehaviour
{
    [Header("=== SETTINGS ===")]
    public string sceneName = "Map2"; // Tên scene muốn chuyển đến

    [Header("=== LOADING UI ===")]
    public float loadingDelay = 2f;    // Thời gian hiển thị loading

    private bool isTeleporting = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra nếu là Player chạm vào và chưa trong trạng thái dịch chuyển
        if (collision.CompareTag("Player") && !isTeleporting)
        {
            isTeleporting = true;

            // Tìm kiếm LoadingPanelHandler độc lập xuyên màn chơi
            if (LoadingPanelHandler.Instance != null)
            {
                // Gọi màn hình chờ bật lên, sau 2 giây tự động gọi hàm nạp scene mới
                LoadingPanelHandler.Instance.StartLoadingCoroutine(loadingDelay, () =>
                {
                    XulyChuyenScene();
                });
            }
            else
            {
                // Nếu không thấy màn hình chờ (ví dụ test nhanh), chuyển thẳng scene luôn
                XulyChuyenScene();
            }
        }
    }

    void XulyChuyenScene()
    {
        // Ghi nhận cổng đi ra để Map tiếp theo biết chỗ dịch chuyển Player tới
        // Ví dụ: Nếu đang ở Map1 đi qua, lưu lại "FromMap1"
        string currentScene = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString("TargetTeleportID", "From" + currentScene);
        PlayerPrefs.Save();

        // Nạp màn chơi mới
        SceneManager.LoadScene(sceneName);
    }
}