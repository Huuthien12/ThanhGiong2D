using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
public class BackToVelang : MonoBehaviour
{
    public string velangSceneName = "Map2";
    public GameObject loadingPanel;
    public float delayTime = 0.5f;
    public AudioClip clickSound;

    private bool isTeleporting = false;

    void Start()
    {
        // Gán sự kiện tự động nếu chưa có
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnClickBackToVelang);
            Debug.Log("Đã gán sự kiện cho nút!");
        }
    }

    public void OnClickBackToVelang()
    {
        Debug.Log("=== ĐÃ BẤM NÚT VỀ LÀNG ===");

        if (isTeleporting)
        {
            Debug.Log("Đang teleport, bỏ qua...");
            return;
        }

        StartCoroutine(BackToVelangCoroutine());
    }

    IEnumerator BackToVelangCoroutine()
    {
        Debug.Log("Bắt đầu teleport...");
        isTeleporting = true;

        if (clickSound != null)
            AudioSource.PlayClipAtPoint(clickSound, Camera.main.transform.position);

        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
            Debug.Log("Bật loading panel");
        }

        yield return new WaitForSeconds(delayTime);

        string fromMap = gameObject.scene.name;
        PlayerPrefs.SetString("FromMap", fromMap);
        PlayerPrefs.Save();

        Debug.Log("Chuyển sang scene: " + velangSceneName);
        SceneManager.LoadScene(velangSceneName);

        isTeleporting = false;
    }
}