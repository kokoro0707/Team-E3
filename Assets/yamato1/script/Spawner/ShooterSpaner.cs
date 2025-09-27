using UnityEngine;
using System.Collections.Generic;

public class ShooterSpawner : MonoBehaviour
{
    public GameObject shooterPrefab;       // 突進型の敵プレハブ
    public float interval = 4f;            // スポーン間隔
    private float timer = 0f;

    private List<GameObject> activeEnemies = new List<GameObject>();
    public static ShooterSpawner Instance;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            SpawnShooter();
            timer = 0f;
        }

        activeEnemies.RemoveAll(e => e == null); // 消えた敵をリストから除去
    }

    void SpawnShooter()
    {
        if (shooterPrefab == null) return;

        float x = Random.Range(-10f, 10f);       // 横方向ランダム
        float y = Random.Range(8f, 12f);         // 上から出現
        Vector3 pos = new Vector3(x, y, 0f);

        GameObject enemy = Instantiate(shooterPrefab, pos, Quaternion.identity);
        activeEnemies.Add(enemy);
    }

    public static void Unregister(GameObject enemy)
    {
        if (Instance != null && Instance.activeEnemies.Contains(enemy))
        {
            Instance.activeEnemies.Remove(enemy);
        }
    } 
}
    