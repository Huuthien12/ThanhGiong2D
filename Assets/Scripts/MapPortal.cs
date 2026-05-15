using UnityEngine;
using UnityEngine.SceneManagement;

public class MapPortal : MonoBehaviour
{
    public string sceneName = "Map2"; // Tên scene map 2 (phải viết đúng)

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Reset máu trước khi chuyển map
            HealthManager healthManager = collision.GetComponent<HealthManager>();
            if (healthManager != null)
            {
                healthManager.ResetHealthForNewMap();
            }

            // Chuyển scene
            SceneManager.LoadScene(sceneName);
        }
    }
}