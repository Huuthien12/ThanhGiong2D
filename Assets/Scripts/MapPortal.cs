using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MapPortal : MonoBehaviour
{
    [Header("=== SETTINGS ===")]
    public string sceneName = "Map2";

    [Header("=== LOADING UI ===")]
    public GameObject loadingPanel;
    public float loadingDelay = 2f; // Thời gian hiển thị loading (2 giây)

    private bool isTeleporting = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isTeleporting)
        {
            StartCoroutine(TeleportCoroutine(collision.gameObject));
        }
    }

    IEnumerator TeleportCoroutine(GameObject player)
    {
        isTeleporting = true;

        // Bật loading panel
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
            Debug.Log("🔵 Bật loading panel, chờ 2 giây...");
        }

        // Chờ 2 giây
        yield return new WaitForSeconds(loadingDelay);

        // Reset máu
        HealthManager healthManager = player.GetComponent<HealthManager>();
        if (healthManager != null)
        {
            healthManager.ResetHealthForNewMap();
        }

        // Chuyển scene
        Debug.Log($"📱 Chuyển sang scene: {sceneName}");
        SceneManager.LoadScene(sceneName);

        isTeleporting = false;
    }
}