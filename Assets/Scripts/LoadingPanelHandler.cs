using System;
using System.Collections;
using UnityEngine;

public class LoadingPanelHandler : MonoBehaviour
{
    // Tạo Singleton để LoadingPanel sống xuyên suốt các Map, không bị hủy giữa chừng
    public static LoadingPanelHandler Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Giữ panel sống sót khi đổi Scene

            // Ban đầu vào game thì tạm ẩn đi, khi nào gọi mới hiện
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject); // Xóa bản sao trùng lặp ở map mới
        }
    }

    public void StartLoadingCoroutine(float loadingTime, Action onComplete)
    {
        // Kích hoạt panel lên trước khi chạy Coroutine
        gameObject.SetActive(true);
        StartCoroutine(LoadingCoroutine(loadingTime, onComplete));
    }

    IEnumerator LoadingCoroutine(float loadingTime, Action onComplete)
    {
        // Chờ đợi thời gian tải map (ví dụ: 2 giây)
        yield return new WaitForSeconds(loadingTime);

        // Thực hiện hành động chuyển cảnh / dịch chuyển vị trí
        onComplete?.Invoke();

        // Tắt panel sau khi mọi thứ đã nạp xong xuôi
        gameObject.SetActive(false);
    }

    // Hàm bổ trợ cho GameManager gọi tắt cưỡng bức nếu có sự cố
    public void ForceClose()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
    }
}