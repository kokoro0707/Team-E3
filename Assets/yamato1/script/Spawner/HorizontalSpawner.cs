using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HorizontalSpawner : MonoBehaviour
{
    public GameObject horizontalPrefab;
    public float interval = 2f;
    public float spawnScaleTime = 0.3f; // 拡大時間
    public float spawnDelay = 0.2f;     // 一体ずつ出す間隔
    public float spacing = 0.8f;        // ← 敵の縦方向の間隔（ここを調整！）

    public static HorizontalSpawner Instance;

    private List<GameObject> activeEnemies = new List<GameObject>();
    private float timer = 0f;

    void Awake() => Instance = this;

    void Start()
    {
        SpawnRandomSide(); // ゲーム開始時にスポーン
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            SpawnRandomSide();
            timer = 0f;
        }
    }

    void SpawnRandomSide()
    {
        if (horizontalPrefab == null) return;

        float minCenterY = -5f + spacing;      // spacingを考慮して最低Y座標を調整
        float centerY = Random.Range(minCenterY, 0f);  // 地面から画面中央までの範囲
        bool spawnLeft = Random.value < 0.3f;
        float x = spawnLeft ? -9f : 9f;

        StartCoroutine(SpawnSequentially(new Vector3(x, centerY, 0f), spawnLeft));
    }

    IEnumerator SpawnSequentially(Vector3 centerPos, bool spawnLeft)
    {
        Vector3[] spawnPositions = new Vector3[3];
        for (int i = 0; i < 3; i++)
        {
            float yOffset = (i - 1) * spacing; // ← spacingを使用
            spawnPositions[i] = centerPos + new Vector3(0f, yOffset, 0f);
        }

        List<StraightEnemy> enemies = new List<StraightEnemy>();

        // 一体ずつ拡大アニメーションで出現
        foreach (Vector3 pos in spawnPositions)
        {
            GameObject enemy = Instantiate(horizontalPrefab, pos, Quaternion.identity);

            // 向きを設定（拡大アニメ中もこの向きで）
            float angle = spawnLeft ? -90f : 90f;
            enemy.transform.rotation = Quaternion.Euler(0, 0, angle);

            StraightEnemy move = enemy.GetComponent<StraightEnemy>();
            if (move != null)
            {
                move.direction = spawnLeft ? Vector2.right : Vector2.left;
                move.spawnScaleTime = spawnScaleTime;
                move.canMove = false; // 拡大中は動かない
           
                enemies.Add(move);
            }

            yield return new WaitForSeconds(spawnDelay); // 順番に出現
        }

        // 全員が出現してから動き開始
        yield return new WaitForSeconds(spawnScaleTime);
        foreach (var e in enemies)
        {
            if (e != null) e.canMove = true;
        }
    }
}
