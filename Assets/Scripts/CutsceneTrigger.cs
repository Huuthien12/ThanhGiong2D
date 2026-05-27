using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneTrigger : MonoBehaviour
{
    [Header("=== CẤU HÌNH ===")]
    public string triggerName = "Cổng kết thúc";
    public bool onlyTriggerOnce = true;

    [Header("=== HÀNH ĐỘNG SAU CUTSCENE ===")]
    public bool quayVeMenuChinh = true;
    public bool tatGameSauCutscene = false;

    [Header("=== HIỆU ỨNG ===")]
    public GameObject effectOnTrigger;
    public AudioClip soundOnTrigger;

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered && onlyTriggerOnce) return;

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;
            GameObject player = other.gameObject;

            Debug.Log($"🎮 Player chạm vào {triggerName}! Phát cutscene kết thúc...");

            // Dừng player
            PlayerControl playerControl = player.GetComponent<PlayerControl>();
            if (playerControl != null) playerControl.enabled = false;

            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;

            // Ẩn player
            player.SetActive(false);
            Debug.Log("🗑️ Player đã bị ẩn!");

            // Phát hiệu ứng
            if (effectOnTrigger != null)
            {
                Instantiate(effectOnTrigger, transform.position, Quaternion.identity);
            }

            // Phát âm thanh
            if (soundOnTrigger != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySound(soundOnTrigger);
            }

            // Phát cutscene kết thúc
            if (CutsceneManager.Instance != null)
            {
                if (tatGameSauCutscene)
                {
                    CutsceneManager.Instance.PlayEndingCutscene(() => {
#if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
#else
                        Application.Quit();
#endif
                    });
                }
                else if (quayVeMenuChinh)
                {
                    CutsceneManager.Instance.PlayEndingCutscene();
                }
                else
                {
                    CutsceneManager.Instance.PlayEndingCutscene();
                }
            }
            else
            {
                Debug.LogError("❌ Không tìm thấy CutsceneManager!");
                if (quayVeMenuChinh)
                {
                    Time.timeScale = 1f;
                    SceneManager.LoadScene("MenuScene");
                }
            }

            // Vô hiệu hóa trigger
            Collider2D col = GetComponent<Collider2D>();
            if (col != null && onlyTriggerOnce)
                col.enabled = false;
        }
    }
}