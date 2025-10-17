using UnityEngine;
using System.Collections.Generic;

public class BounceSpawner : MonoBehaviour
{
    public GameObject bouncePrefab;
    public float interval = 2f;
    private float timer;

    private List<GameObject> activeEnemies = new List<GameObject>();
    public static BounceSpawner Instance;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            SpawnFromTop();
            timer = 0f;
        }

        activeEnemies.RemoveAll(e => e == null);
    }

    void SpawnFromTop()
    {
        if (bouncePrefab == null) return;

        float x = Random.Range(4f, -4f);
        Vector3 pos = new Vector3(x, 4f, 0f); // âÊñ ì‡Ç…ÉXÉ|Å[Éì

        GameObject enemy = Instantiate(bouncePrefab, pos, Quaternion.identity);

        BouncingEnemy bounce = enemy.GetComponent<BouncingEnemy>();
        if (bounce != null)
        {
            bounce.velocity = new Vector2(Random.Range(-2f, 2f), -3f); // éŒÇﬂâ∫Ç…èâë¨
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