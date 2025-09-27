using UnityEngine;
using System.Collections.Generic;

public class SplitSpawner : MonoBehaviour
{
    public GameObject splitEnemyPrefab;   // 敵プレハブ
    public float interval = 3f;            // スポーン間隔
    private float timer = 0f;

    private List<GameObject> activeEnemies = new List<GameObject>();

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            SpawnFromAbove();
            timer = 0f;
        }

        // 消えた敵をリストから除去
        activeEnemies.RemoveAll(e => e == null);
    }

    void SpawnFromAbove()
    {
        if (splitEnemyPrefab == null) return;

        float camX = Camera.main.transform.position.x;
        float camY = Camera.main.transform.position.y;

        float orthoSize = Camera.main.orthographicSize;
        float aspect = Camera.main.aspect;

        float camHalfHeight = orthoSize; // 9
        float camHalfWidth = orthoSize * aspect; // 9 * 16/9 = 16

        float leftX = camX - camHalfWidth;
        float rightX = camX + camHalfWidth;
        float topY = camY + camHalfHeight;

        float spawnX = Random.Range(leftX, rightX);
        float spawnY = topY + 1f;  // 1ユニット上

        Vector3 spawnPos = new Vector3(spawnX, spawnY, 0f);

        Instantiate(splitEnemyPrefab, spawnPos, Quaternion.identity);
    }

}
