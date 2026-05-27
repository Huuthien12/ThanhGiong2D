using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class BackToVelang : MonoBehaviour
{
    [Header("=== CẤU HÌNH ===")]
    public string velangSceneName = "Map2";
    public float delayTime = 0.5f;
    public AudioClip clickSound;

    [Header("=== PHÍM TẮT ===")]
    public KeyCode backKey = KeyCode.O;  // Phím O để về làng

    private bool isTeleporting = false;
    private Button btn;

    void Start()
    {
        // ✅ Bỏ điều kiện map, luôn hiển thị nút
        gameObject.SetActive(true);

        btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnClickBackToVelang);
            Debug.Log("✅ Nút về làng đã sẵn sàng");
        }
    }

    void Update()
    {
        // ✅ Phím O để về làng - không cần điều kiện map
        if (Input.GetKeyDown(backKey) && !isTeleporting)
        {
            Debug.Log("⌨️ Đã bấm phím O - Về làng!");
            OnClickBackToVelang();
        }
    }

    public void OnClickBackToVelang()
    {
        if (isTeleporting) return;
        StartCoroutine(BackToVelangCoroutine());
    }

    IEnumerator BackToVelangCoroutine()
    {
        isTeleporting = true;

        if (clickSound != null)
            AudioSource.PlayClipAtPoint(clickSound, Camera.main.transform.position);

        if (LoadingManager.Instance != null)
            LoadingManager.Instance.ShowLoading();

        yield return new WaitForSeconds(delayTime);

        PlayerPrefs.SetString("FromMap", gameObject.scene.name);
        PlayerPrefs.Save();

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(velangSceneName);

        isTeleporting = false;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (LoadingManager.Instance != null)
            LoadingManager.Instance.HideLoading();
    }
}