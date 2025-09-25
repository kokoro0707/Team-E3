using UnityEngine;
using System.Collections.Generic;

public class SplitSpawner : MonoBehaviour
{
    public GameObject splitEnemyPrefab;   // 分裂する敵のプレハブ
    public float interval = 3f;           // スポーン間隔
    private float timer;

    private List<GameObject> activeEnemies = new List<GameObject>();
    public static SplitSpawner Instance;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            SpawnFromAbove();
            timer = 0f;
        }

        activeEnemies.RemoveAll(e => e == null);
    }

    void SpawnFromAbove()
    {
        if (splitEnemyPrefab == null) return;

        // カメラの左右端と上端を取得
        Vector2 left = Camera.main.ViewportToWorldPoint(new Vector2(0f, 1f));
        Vector2 right = Camera.main.ViewportToWorldPoint(new Vector2(1f, 1f));
        Vector2 top = Camera.main.ViewportToWorldPoint(new Vector2(0.5f, 1f));

        float spawnX = Random.Range(left.x, right.x);
        float spawnY = top.y + 1f; // 画面外から出現

        Vector3 pos = new Vector3(spawnX, spawnY, 0f);
        GameObject enemy = Instantiate(splitEnemyPrefab, pos, Quaternion.identity);

        SplittingEnemy split = enemy.GetComponent<SplittingEnemy>();
        if (split != null)
        {
            split.splitCount = 0; // 初期状態
        }

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