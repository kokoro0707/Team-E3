using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HorizontalSpawner : MonoBehaviour
{
    public GameObject horizontalPrefab;
    public GameObject warningPrefab;
    public float interval = 2f;
    public float warningDuration = 1.5f;
    public float spawnScaleTime = 0.5f; // 拡大にかける時間（秒）

    private float timer = 0f;
    private List<GameObject> activeEnemies = new List<GameObject>();
    public static HorizontalSpawner Instance;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            SpawnRandomSide();
            timer = 0f;
        }

        activeEnemies.RemoveAll(e => e == null);
    }

    void SpawnRandomSide()
    {
        if (horizontalPrefab == null || warningPrefab == null) return;

        float centerY = Random.Range(-8f, -1f);
        bool spawnLeft = Random.value < 0.5f;
        float x = spawnLeft ? -16f : 16f;

        StartCoroutine(SpawnWithWarning(new Vector3(x, centerY, 0f), spawnLeft));
    }

    IEnumerator SpawnWithWarning(Vector3 centerPos, bool spawnLeft)
    {
        // 3体の縦位置オフセット（例：上下に1.2f間隔で配置）
        float offset = 1.2f;
        Vector3[] spawnPositions = new Vector3[3];
        for (int i = 0; i < 3; i++)
        {
            float yOffset = (i - 1) * offset; // i=0→-1, i=1→0, i=2→+1
            spawnPositions[i] = centerPos + new Vector3(0f, yOffset, 0f);
        }

        // 警告マークを3体分表示
        List<GameObject> warnings = new List<GameObject>();
        foreach (Vector3 pos in spawnPositions)
        {
            GameObject warning = Instantiate(warningPrefab, pos, Quaternion.identity);
            warnings.Add(warning);
        }

        yield return new WaitForSeconds(warningDuration);

        // 警告削除
        foreach (GameObject warning in warnings)
        {
            Destroy(warning);
        }

        // 敵を3体スポーン
        foreach (Vector3 pos in spawnPositions)
        {
            GameObject enemy = Instantiate(horizontalPrefab, pos, Quaternion.identity);
            activeEnemies.Add(enemy);

            // 向きと進行方向設定
            StraightEnemy move = enemy.GetComponent<StraightEnemy>();
            if (move != null)
            {
                move.direction = spawnLeft ? Vector2.right : Vector2.left;
                move.spawnScaleTime = spawnScaleTime;

                float angle = Mathf.Atan2(move.direction.y, move.direction.x) * Mathf.Rad2Deg - 90f;
                enemy.transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }
    }

    public static void Unregister(GameObject enemy)
    {
        if (Instance != null && Instance.activeEnemies.Contains(enemy))
        {
            Instance.activeEnemies.Remove(enemy);
        }
    }
}
