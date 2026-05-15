using UnityEngine;

public class SimpleTeleportOnTouch : MonoBehaviour
{
    [Header("Settings")]
    public Transform teleportDestination;
    public GameObject player;

    [Header("Effects")]
    public AudioClip teleportSound;

    private bool canTeleport = true;

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && canTeleport)
        {
            StartCoroutine(TeleportCoroutine());
        }
    }

    System.Collections.IEnumerator TeleportCoroutine()
    {
        canTeleport = false;

        // Phát âm thanh
        if (teleportSound != null)
            AudioSource.PlayClipAtPoint(teleportSound, transform.position);

        // Hiệu ứng nhấp nháy (tùy chọn)
        SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color originalColor = sr.color;
            sr.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            sr.color = originalColor;
        }

        // Teleport
        player.transform.position = teleportDestination.position;

        Debug.Log($"Player teleported from {gameObject.name} to {teleportDestination.name}");

        yield return new WaitForSeconds(0.5f);
        canTeleport = true;
    }

    // Vẽ vùng teleport trong Scene view
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.DrawWireCube(transform.position, col.bounds.size);
        }

        if (teleportDestination != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, teleportDestination.position);
            Gizmos.DrawWireSphere(teleportDestination.position, 0.5f);
        }
    }
}