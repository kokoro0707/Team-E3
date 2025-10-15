using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;

    // 指定された数だけ敵をスポーンするメソッド
    public void SpawnEnemies(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPos = transform.position + new Vector3(i * 1.5f, 0, 0);
            Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        }
    }
}
