using UnityEngine;

public class StairClimber : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool onStairs = false;
    private Vector2 climbDir;
    private Transform stairStart, stairEnd;

    [Header("Stair Settings")]
    public float climbSpeed = 3f; // Tốc độ leo

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (onStairs)
        {
            // Nhận input lên/xuống
            float v = 0;
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                v = 1;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                v = -1;

            // DI CHUYỂN TRÊN CẦU THANG
            if (v != 0)
            {
                Vector2 move = climbDir * v * climbSpeed * Time.deltaTime;
                transform.Translate(move);
            }

            // Giới hạn vị trí không ra khỏi cầu thang
            if (stairStart != null && stairEnd != null)
            {
                Vector2 fromStart = transform.position - stairStart.position;
                float progress = Vector2.Dot(fromStart, climbDir);
                float totalDistance = Vector2.Distance(stairStart.position, stairEnd.position);

                if (progress < 0)
                    transform.position = stairStart.position;
                if (progress > totalDistance)
                    transform.position = stairEnd.position;
            }

            // CHO PHÉP DI CHUYỂN NGANG TRONG KHI LEO (tùy chọn)
            float h = 0;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                h = -1;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                h = 1;

            if (h != 0)
            {
                Vector2 horizontalMove = new Vector2(h * 0.5f * Time.deltaTime, 0);
                transform.Translate(horizontalMove);
            }

            // THOÁT CẦU THANG
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ExitStairs();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Stairs"))
        {
            Debug.Log("Bắt đầu leo cầu thang!");
            onStairs = true;
            rb.gravityScale = 0;
            rb.linearVelocity = Vector2.zero;

            StairsData data = other.GetComponent<StairsData>();
            if (data != null && data.startPoint != null && data.endPoint != null)
            {
                stairStart = data.startPoint;
                stairEnd = data.endPoint;
                climbDir = (stairEnd.position - stairStart.position).normalized;
            }
            else
            {
                Debug.LogError("Thiếu StairsData hoặc startPoint/endPoint!");
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Stairs"))
        {
            ExitStairs();
        }
    }

    void ExitStairs()
    {
        Debug.Log("Thoát cầu thang");
        onStairs = false;
        rb.gravityScale = 1;
    }
}