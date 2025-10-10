using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HorizontalSpawner : MonoBehaviour
{
    public GameObject horizontalPrefab; // 敵のPrefab
    public GameObject warningPrefab;    // 警告マークのPrefab
    public float interval = 2f;         // 敵を出す間隔
    public float warningDuration = 1.5f; // 警告を出しておく時間

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

        float y = Random.Range(-8f, -1f);
        bool spawnLeft = Random.value < 0.5f;
        float x = spawnLeft ? -16f : 16f;
        Vector3 spawnPos = new Vector3(x, y, 0f);

        // 警告マークの位置は、敵の出現位置と一致させる（少し内側にしてもOK）
        StartCoroutine(SpawnWithWarning(spawnPos, spawnLeft));
    }

    IEnumerator SpawnWithWarning(Vector3 spawnPosition, bool spawnLeft)
    {
        // 警告マークを生成
        GameObject warning = Instantiate(warningPrefab, spawnPosition, Quaternion.identity);

        // 警告マークを一定時間表示
        yield return new WaitForSeconds(warningDuration);

        // 警告マークを削除
        Destroy(warning);

        // 敵を生成
        GameObject enemy = Instantiate(horizontalPrefab, spawnPosition, Quaternion.identity);
        activeEnemies.Add(enemy);

        // 移動方向設定
        StraightEnemy move = enemy.GetComponent<StraightEnemy>();
        if (move != null)
        {
            move.direction = spawnLeft ? Vector2.right : Vector2.left;

            // 向き調整：スプライトの上が前提
            float angle = Mathf.Atan2(move.direction.y, move.direction.x) * Mathf.Rad2Deg - 90f;
            enemy.transform.rotation = Quaternion.Euler(0, 0, angle);
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
