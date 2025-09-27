using UnityEngine;
using System.Collections.Generic;

public class ZigZagSpawner : MonoBehaviour
{
    public GameObject zigZagPrefab;
    public float interval = 3f;
    private float timer;

    private List<GameObject> activeEnemies = new List<GameObject>();
    public static ZigZagSpawner Instance;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            float x = Random.Range(-10f, 10f);
            Vector3 pos = new Vector3(x, 10f, 0f);
            GameObject enemy = Instantiate(zigZagPrefab, pos, Quaternion.identity);
            activeEnemies.Add(enemy);
            timer = 0f;
        }

        activeEnemies.RemoveAll(e => e == null);
    }

    public static void Unregister(GameObject enemy)
    {
        if (Instance != null && Instance.activeEnemies.Contains(enemy))
        {
            Instance.activeEnemies.Remove(enemy);
        }
    }
}