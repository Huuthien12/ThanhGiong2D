using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("=== CẤU HÌNH SPAWN ===")]
    public GameObject enemyPrefab;           // Prefab quái vật
    public Transform spawnPoint;             // Điểm spawn cố định (kéo object vào)
    public float spawnInterval = 2f;         // Thời gian giữa các lần spawn
    public int maxEnemies = 5;               // Số lượng quái tối đa cùng lúc

    [Header("=== GIỚI HẠN SỐ LẦN SPAWN ===")]
    public bool gioiHanSoLanSpawn = true;    // Bật/tắt giới hạn số lần spawn
    public int soLanSpawnToiDa = 5;          // Số lần spawn tối đa (mặc định 5)
    private int soLanDaSpawn = 0;            // Đếm số lần đã spawn

    [Header("=== TỐC ĐỘ SPAWN THEO THỜI GIAN ===")]
    public bool tangTocDoSpawnTheoThoiGian = false;
    public float spawnIntervalMin = 0.5f;    // Tốc độ spawn nhanh nhất
    public float thoiGianDeTangToc = 60f;    // Sau bao lâu thì đạt tốc độ max

    private List<GameObject> enemies = new List<GameObject>();
    private Transform player;
    private float currentSpawnInterval;
    private float nextSpawnTime;
    private float startTime;
    private bool daSpawnHet = false;          // Đánh dấu đã spawn hết số lần

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
        nextSpawnTime = Time.time + 1f;
        startTime = Time.time;
        soLanDaSpawn = 0;
        daSpawnHet = false;

        Debug.Log($"✅ Enemy Spawner khởi tạo tại {spawnPoint.position}");
        Debug.Log($"📊 Sẽ spawn tối đa {soLanSpawnToiDa} lần");
    }

    void Update()
    {
        if (player == null) return;

        // ✅ Nếu đã spawn đủ số lần thì không spawn nữa
        if (gioiHanSoLanSpawn && daSpawnHet) return;

        // Xóa quái đã chết khỏi danh sách
        RemoveDeadEnemies();

        // Spawn quái theo thời gian
        if (Time.time >= nextSpawnTime)
        {
            // ✅ Kiểm tra còn được spawn không
            if (CanSpawn())
            {
                SpawnEnemy();
                soLanDaSpawn++;
                Debug.Log($"🐉 Lần spawn thứ {soLanDaSpawn}/{soLanSpawnToiDa}");

                // ✅ Kiểm tra nếu đã spawn đủ số lần
                if (gioiHanSoLanSpawn && soLanDaSpawn >= soLanSpawnToiDa)
                {
                    daSpawnHet = true;
                    Debug.Log($"🏁 Đã spawn đủ {soLanSpawnToiDa} lần! Ngừng spawn.");
                }
            }

            // Cập nhật thời gian spawn tiếp theo
            CapNhatTocDoSpawn();
            nextSpawnTime = Time.time + currentSpawnInterval;
        }
    }

    // ✅ Kiểm tra có thể spawn không
    bool CanSpawn()
    {
        // Nếu bật giới hạn và đã spawn đủ số lần
        if (gioiHanSoLanSpawn && soLanDaSpawn >= soLanSpawnToiDa)
            return false;

        // Nếu đã đạt số lượng quái tối đa trên màn hình
        if (enemies.Count >= maxEnemies)
            return false;

        return true;
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
        newEnemy.transform.parent = transform;

        // Cấu hình quái
        EnemyMovement enemyMovement = newEnemy.GetComponent<EnemyMovement>();
        if (enemyMovement == null)
        {
            enemyMovement = newEnemy.AddComponent<EnemyMovement>();
        }
        enemyMovement.SetTarget(player);

        enemies.Add(newEnemy);
        Debug.Log($"🐉 Spawn quái mới! Tổng số: {enemies.Count}/{maxEnemies}");
    }

    void RemoveDeadEnemies()
    {
        enemies.RemoveAll(enemy => enemy == null);
    }

    // ✅ Hàm reset spawner (dùng khi restart map)
    public void ResetSpawner()
    {
        soLanDaSpawn = 0;
        daSpawnHet = false;
        nextSpawnTime = Time.time + spawnInterval;
        ClearAllEnemies();
        Debug.Log("🔄 Đã reset Enemy Spawner");
    }

    // ✅ Hàm ép buộc dừng spawn
    public void StopSpawning()
    {
        daSpawnHet = true;
        enabled = false;
        Debug.Log("⏸️ Đã dừng spawn");
    }

    // ✅ Hàm bắt đầu spawn lại (có reset số lần)
    public void StartSpawning(bool resetCount = true)
    {
        if (resetCount)
        {
            soLanDaSpawn = 0;
            daSpawnHet = false;
        }
        enabled = true;
        nextSpawnTime = Time.time + spawnInterval;
        Debug.Log("▶️ Tiếp tục spawn");
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

    // Hàm lấy số lượng quái hiện tại
    public int GetEnemyCount()
    {
        RemoveDeadEnemies();
        return enemies.Count;
    }

    // ✅ Hàm lấy số lần đã spawn
    public int GetSoLanDaSpawn()
    {
        return soLanDaSpawn;
    }

    // ✅ Hàm kiểm tra đã spawn hết chưa
    public bool IsSpawnHet()
    {
        return daSpawnHet;
    }

    void OnDrawGizmos()
    {
        if (spawnPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(spawnPoint.position, 1f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(spawnPoint.position, spawnPoint.position + Vector3.up * 2f);
        }
    }
}