using UnityEngine;

public class VatPhamCom : MonoBehaviour
{
    public float phanTramHoi = 0.1f;
    public GameObject floatingTextPrefab;
    public Vector3 offset = new Vector3(0, 0.5f, 0);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            HealthManager healthManager = collision.GetComponent<HealthManager>();

            if (healthManager != null)
            {
                float luongHoi = healthManager.maxHealth * phanTramHoi;
                healthManager.Heal(luongHoi);

                ShowFloatingText(collision.transform.position, luongHoi);
                PlayEatSound();

                // ⭐⭐⭐ THÊM DÒNG NÀY - Báo cho GameManager đã ăn com ⭐⭐⭐
                GameManager gameManager = FindFirstObjectByType<GameManager>();
                if (gameManager != null)
                {
                    gameManager.ThuThapCom();
                }
                else
                {
                    Debug.LogWarning("Không tìm thấy GameManager trong scene!");
                }

                Destroy(gameObject);
            }
        }
    }

    void ShowFloatingText(Vector3 playerPosition, float healAmount)
    {
        if (floatingTextPrefab != null)
        {
            Vector3 spawnPos = playerPosition + offset;
            GameObject floatingText = Instantiate(floatingTextPrefab, spawnPos, Quaternion.identity);

            Floating3DText ft = floatingText.GetComponent<Floating3DText>();
            if (ft != null)
            {
                ft.SetText("+" + Mathf.RoundToInt(healAmount));
            }

            Destroy(floatingText, 1.5f);
        }
    }

    void PlayEatSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayEatSound();
        }
        else
        {
            Debug.LogWarning("Không tìm thấy AudioManager trong scene!");
        }
    }
}