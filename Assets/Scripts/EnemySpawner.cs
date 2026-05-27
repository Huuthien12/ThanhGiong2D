using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("=== CẤU HÌNH SPAWN ===")]
    public GameObject enemyPrefab;           // Prefab quái vật
    public Transform spawnPoint;             // Điểm spawn cố định (kéo object vào)
    public float spawnInterval = 2f;         // Thời gian giữa các lần spawn
    public int maxEnemies = 5;               // Số lượng quái tối đa cùng lúc

    [Header("=== TỐC ĐỘ SPAWN THEO THỜI GIAN ===")]
    public bool tangTocDoSpawnTheoThoiGian = false;
    public float spawnIntervalMin = 0.5f;    // Tốc độ spawn nhanh nhất
    public float thoiGianDeTangToc = 60f;    // Sau bao lâu thì đạt tốc độ max

    private List<GameObject> enemies = new List<GameObject>();
    private Transform player;
    private float currentSpawnInterval;
    private float nextSpawnTime;
    private float startTime;

    void Start()
    {
        // Tìm player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // Kiểm tra spawn point
        if (spawnPoint == null)
        {
            spawnPoint = transform;
            Debug.LogWarning("Chưa gán Spawn Point, dùng vị trí của Spawner làm điểm spawn");
        }

        // Khởi tạo
        currentSpawnInterval = spawnInterval;
        nextSpawnTime = Time.time + 1f; // Spawn sau 1 giây
        startTime = Time.time;

        Debug.Log($"✅ Enemy Spawner khởi tạo tại {spawnPoint.position}");
    }

    void Update()
    {
        if (player == null) return;

        // Xóa quái đã chết khỏi danh sách
        RemoveDeadEnemies();

        // Spawn quái theo thời gian
        if (Time.time >= nextSpawnTime)
        {
            if (enemies.Count < maxEnemies)
            {
                SpawnEnemy();
            }

            // Cập nhật thời gian spawn tiếp theo
            CapNhatTocDoSpawn();
            nextSpawnTime = Time.time + currentSpawnInterval;
        }
    }

    void CapNhatTocDoSpawn()
    {
        if (!tangTocDoSpawnTheoThoiGian) return;

        // Tính thời gian đã trôi qua
        float thoiGianDaTroi = Time.time - startTime;

        // Tính tỷ lệ (0 = bắt đầu, 1 = đạt tốc độ max)
        float tyLe = Mathf.Clamp01(thoiGianDaTroi / thoiGianDeTangToc);

        // Giảm dần spawn interval
        currentSpawnInterval = Mathf.Lerp(spawnInterval, spawnIntervalMin, tyLe);
    }

    void SpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("❌ Enemy Prefab chưa được gán!");
            return;
        }

        if (spawnPoint == null) return;

        // Spawn quái mới
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

        // Đặt parent để dễ quản lý
        newEnemy.transform.parent = transform;

        // Cấu hình quái để nó tự động đuổi theo player
        EnemyMovement enemyMovement = newEnemy.GetComponent<EnemyMovement>();
        if (enemyMovement == null)
        {
            // Nếu chưa có script EnemyMovement, tự động thêm
            enemyMovement = newEnemy.AddComponent<EnemyMovement>();
        }

        // Thiết lập player target
        enemyMovement.SetTarget(player);

        // Thêm vào danh sách
        enemies.Add(newEnemy);

        Debug.Log($"🐉 Spawn quái mới! Tổng số: {enemies.Count}/{maxEnemies}");
    }

    void RemoveDeadEnemies()
    {
        enemies.RemoveAll(enemy => enemy == null);
    }

    // Hàm xóa tất cả quái
    public void ClearAllEnemies()
    {
        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
                Destroy(enemy);
        }
        enemies.Clear();
    }

    // Hàm dừng spawn
    public void StopSpawning()
    {
        enabled = false;
    }

    // Hàm bắt đầu spawn lại
    public void StartSpawning()
    {
        enabled = true;
        nextSpawnTime = Time.time + spawnInterval;
    }

    // Hàm lấy số lượng quái hiện tại
    public int GetEnemyCount()
    {
        RemoveDeadEnemies();
        return enemies.Count;
    }

    void OnDrawGizmos()
    {
        if (spawnPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(spawnPoint.position, 1f);

            // Vẽ mũi tên chỉ hướng
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(spawnPoint.position, spawnPoint.position + Vector3.up * 2f);
        }
    }
}