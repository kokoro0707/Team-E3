using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public Transform[] spawnPoints;
    public int spawnCount = 5;
    public GameManager gameManager;

    public void SpawnEnemies()
    {
        gameManager.SetEnemyCount(spawnCount);

        for (int i = 0; i < spawnCount; i++)
        {
            int prefabIndex = Random.Range(0, enemyPrefabs.Length);
            int pointIndex = Random.Range(0, spawnPoints.Length);

            GameObject enemy = Instantiate(enemyPrefabs[prefabIndex], spawnPoints[pointIndex].position, Quaternion.identity);
            enemy.GetComponent<Enemy>().gameManager = gameManager;
        }
    }
}
