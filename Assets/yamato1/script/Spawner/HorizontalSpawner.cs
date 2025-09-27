using UnityEngine;
using System.Collections.Generic;

public class HorizontalSpawner : MonoBehaviour
{
    public GameObject horizontalPrefab;
    public float interval = 2f;
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
        if (horizontalPrefab == null) return;

        float y = Random.Range(-8f, -1f);
        bool spawnLeft = Random.value < 0.5f;
        float x = spawnLeft ? -16f : 16f;
        Vector3 pos = new Vector3(x, y, 0f);

        GameObject enemy = Instantiate(horizontalPrefab, pos, Quaternion.identity);
        activeEnemies.Add(enemy);

        StraightEnemy move = enemy.GetComponent<StraightEnemy>();
        if (move != null)
        {
            move.direction = spawnLeft ? Vector2.right : Vector2.left;
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